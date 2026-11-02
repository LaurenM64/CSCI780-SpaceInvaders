using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class AlienFleet : MonoBehaviour
{
    [Header("Inscribed")]

    [SerializeField] private GameObject alienPrefab;

    public AlienCounter alienCounter;

    public List<GameObject> aliens = new List<GameObject> { };

    [SerializeField] private float speed = .025f;
    [SerializeField] private float leftBound;
    [SerializeField] private float rightBound;
    [SerializeField] private float lowerBound;
    [SerializeField] private float upperBound;

    [SerializeField] private float downTime = .1f;
    [SerializeField] private bool movingDown = false;

    //For the varying speed of the fleet:
    [SerializeField] private float baseSpeed = 0.02f;
    [SerializeField] private float maxSpeedMultiplier = 3f; // How much faster they get at the very end
    private int startingAlienCount;

    //player reference for game over check
    [SerializeField] private Transform playerShip;
    [SerializeField] private float gameOverBuffer = .5f;
    



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
//moved spawn code to its own LoadLevel function so it can call new levels without restarting the game
    }
    public void LoadLevel(int level)
    {
        // 1. WIPE THE BOARD
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
        aliens.Clear(); 
        startingAlienCount = 0; 

        // 2. PICK THE LEVEL MATRIX
        int[,] currentLevelMap;
        switch (level)
        {
            case 4: 
                currentLevelMap = new int[,] {
                    { 0, 0, 1, 1, 1, 1, 0, 0 },
                    { 0, 1, 1, 1, 1, 1, 1, 0 },
                    { 1, 1, 1, 1, 1, 1, 1, 1 },
                    { 0, 1, 1, 1, 1, 1, 1, 0 },
                    { 0, 0, 1, 1, 1, 1, 0, 0 },
                    
                };
                break;
            case 3: 

                currentLevelMap = new int[,] {
                    { 1, 1, 1, 1, 1, 1, 1, 1 },
                    { 0, 1, 1, 1, 1, 1, 1, 0 },
                    { 0, 0, 1, 1, 1, 1, 0, 0 },
                    { 0, 0, 0, 1, 1, 0, 0, 0 }
                };
                break;
            case 2:
                currentLevelMap = new int[,] {
                    { 0, 0, 1, 0, 0 },
                    { 1, 1, 1, 1, 1},
                    { 1, 1, 0, 1, 1 },
                    { 1, 0, 0, 0, 1 }
                };
                break;
            default: 
                currentLevelMap = new int[,] {
                    { 1, 0, 0, 0, 0 ,0,1},
                    { 0, 1, 0, 0, 0,1,0 },
                    { 0, 0, 1, 0, 1,0,0 },
                    { 0, 0, 0, 1, 0,0,0 }
                };
                break;
        }

        // 3. CALCULATE BOUNDARIES EXACTLY WHEN NEEDED
        float zDistance = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 screenBottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, zDistance));
        Vector3 screenTopRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, zDistance));

        leftBound = screenBottomLeft.x + 1f; 
        rightBound = screenTopRight.x - 1f;
        lowerBound = screenBottomLeft.y + 1f;
        upperBound = screenTopRight.y;

        // 4. CALCULATE SPAWN POINTS
        int rows = currentLevelMap.GetLength(0);
        int cols = currentLevelMap.GetLength(1);
        float rspacing = 1f;
        float cspacing = 2f;
        float startX = leftBound + 3f; 
        float startY = upperBound - 1f;

        // 5. SPAWN THE ALIENS
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (currentLevelMap[r, c] == 1)
                {
                    GameObject alien = Instantiate<GameObject>(alienPrefab);
                    alien.transform.position = new Vector3(startX + (c * cspacing), startY - (r * rspacing), 0);
                    alien.transform.SetParent(this.transform); 
                    
                    aliens.Add(alien);
                    alien.GetComponent<Alien>().fleet = this;
                }
            }
        }

        // 6. RESET FLEET VARIABLES
        startingAlienCount = aliens.Count;
        speed = baseSpeed; 
        movingDown = false;
    }


   

//END LOADING LEVEL CODE



    // Update is called once per frame
    void Update()
    { //VICTORY CHECK
    //note: if we dont check startingAlienCount > 0, it will trigger victory on the first frame of the game before any aliens spawn
        if (GameManager.Instance.IsPlaying && startingAlienCount > 0 && aliens.Count == 0)        {
        GameManager.Instance.TriggerVictory();
        }
    }

    void FixedUpdate()
    {

        // SAFETY LOCK: Do absolutely nothing if there are no aliens
        if (aliens.Count == 0) return;


        //SPEED: the math to calculate the speed multiplier based on the number of remaining aliens
        if (startingAlienCount > 0 && aliens.Count > 0)
        {
            float percentRemaining = (float)aliens.Count / startingAlienCount;
            
            // Smoothly blend between max speed and base speed based on survival percentage
            float currentMagnitude = Mathf.Lerp(baseSpeed * maxSpeedMultiplier, baseSpeed, percentRemaining);
            
            // Apply the new speed magnitude while preserving the left/right direction (1 or -1)
            speed = currentMagnitude * Mathf.Sign(speed);
        }        



        //move all aliens
        foreach (GameObject alien in aliens)
        {
            Vector3 position = alien.transform.position;
            if (!movingDown)
            {
                position.x += speed;
            }
            else
            {
                position.y += -Mathf.Abs(speed);
                /*if (position.y <= lowerBound)
                {
                    movingDown = false;
                    lowerBound = position.y - 1;
                }*/
            }
            alien.transform.position = position;
        }

        // Grab the first alien's position as a starting baseline
        float left = aliens[0].transform.position.x;
        float right = aliens[0].transform.position.x;
        float bottom = aliens[0].transform.position.y;

        foreach (GameObject alien in aliens)
        {
            Vector3 position = alien.transform.position;
            
            if (position.x > right) right = position.x;
            if (position.x < left) left = position.x;
            if (position.y < bottom) bottom = position.y; // Find the lowest alien
        }

    //GAMEOVER CHECK:

        if (playerShip != null && bottom <= (playerShip.position.y+gameOverBuffer))
        {
            Debug.Log("GAME OVER: The fleet reached the ship!");
            GameManager.Instance.TriggerGameOver();
            return; // Stops running the rest of the movement code this frame
        }

    //MOVING DOWN CHECK
        if (speed > 0 && right >= rightBound || speed < 0 && left <= leftBound)
        {
            speed = -speed;
            movingDown = true;
            Invoke("startSidewaysMotion", downTime);
        }
    }

    void startSidewaysMotion()
    {
        movingDown = false;
    }
}