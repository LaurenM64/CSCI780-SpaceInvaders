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
    [SerializeField] private float baseSpeed = 0.025f;
    [SerializeField] private float maxSpeedMultiplier = 2.5f; // How much faster they get at the very end
    private int startingAlienCount;

    //player reference for game over check
    [SerializeField] private Transform playerShip;
    [SerializeField] private float gameOverBuffer = .5f;
    
    //LEVEL LAYOUT VARIABLES
    // The Level 1 Layout (8 columns wide to give them room to move)
    private int[,] currentLevelMap = {
        { 1, 1, 1, 1, 1, 1, 1, 1 },
        { 0, 1, 1, 1, 1, 1, 1, 0 },
        { 0, 0, 1, 1, 1, 1, 0, 0 }
    };



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        // Find the edges of the screen dynamically based on the camera to not get bogged down on hardcoding coords
        // 1. Get the depth distance from the camera to the game plane
        float zDistance = Mathf.Abs(Camera.main.transform.position.z);
        
        // 2. Find the absolute corners of the screen dynamically
        Vector3 screenBottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, zDistance));
        Vector3 screenTopRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, zDistance));

        // 3. Set padded boundaries for movement and failsafes
        leftBound = screenBottomLeft.x + 1f; 
        rightBound = screenTopRight.x - 1f;
        lowerBound = screenBottomLeft.y + 1f;
        
        upperBound = screenTopRight.y ; // 1 unit below the top edge to start spawning


        // 4. Spawn the grid using the dynamic boundaries instead of hardcoded numbers

        // Read the dimensions of the level map
        int rows = currentLevelMap.GetLength(0);
        int cols = currentLevelMap.GetLength(1);
        float rspacing = 1f;
        float cspacing = 2f;

        // Start spawning slightly inward from the absolute edge so we don't spawn offscreen
        float startX = leftBound + 3f; 
        float startY = upperBound - 1f;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                // Only spawn an alien if the matrix has a 1
                if (currentLevelMap[r, c] == 1)
                {
                    GameObject alien = Instantiate<GameObject>(alienPrefab);
                    
                    // Calculate position based on the current row (r) and column (c)
                    alien.transform.position = new Vector3(startX + (c * cspacing), startY - (r * rspacing), 0);
                    
                    aliens.Add(alien);
                    alien.GetComponent<Alien>().fleet = this;
                }
            }
        }
    //record current starting alien count for later speed scaling
        startingAlienCount = aliens.Count;

    }

    // Update is called once per frame
    void Update()
    {
        //alienCounter.score = aliens.Count;
    }

    void FixedUpdate()
    {
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
            Time.timeScale = 0; // Freezes the game engine
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