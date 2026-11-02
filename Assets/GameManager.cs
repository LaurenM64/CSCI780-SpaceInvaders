using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } 
    public bool IsPlaying { get; private set; } = false;

    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private GameObject player;
    [SerializeField] private AlienFleet alienFleet;


//LIVES AND LEVEL TRACKING
    public int currentLives = 3;
    public int currentLevel = 1;
    private bool isVictory = false;
    public int totalLevels = 4; //total levels for now

//AWAKE STARTS INSTANTLY UPON LOADING BEFORE START
    void Awake()
    {
        if (Instance == null) Instance = this; 
        else Destroy(gameObject);
    }

    void Start()
    {
        SetScreenEmpty(true);
        messageText.text = "SPACE INVADERS\nPRESS SPACE TO START";
        messageText.gameObject.SetActive(true);
    }

   void Update()
{
    if (!IsPlaying && Input.GetKeyDown(KeyCode.Space))
    {
        IsPlaying = true;
        Time.timeScale = 1; 
        messageText.gameObject.SetActive(false); 
        SetScreenEmpty(false);

        if (isVictory)
        {
            isVictory = false;

            if (currentLevel < totalLevels)
            {
                // Progress to the next level
                currentLevel++;
                alienFleet.LoadLevel(currentLevel);
            }
            else
            {
                // They beat the whole game, so start fresh at Level 1
                currentLives = 3;
                livesText.text = "LIVES: " + currentLives;
                currentLevel = 1;
                player.transform.position = new Vector3(0, player.transform.position.y, 0); 
                
                alienFleet.LoadLevel(currentLevel);
            }
        }
        else 
        {
            // FIRST LAUNCH OR RESTART AFTER GAME OVER
            currentLives = 3;
            livesText.text = "LIVES: " + currentLives;
            currentLevel = 1;
            
            // Reset player position to center
            if (player != null)
            {
                player.transform.position = new Vector3(0, player.transform.position.y, 0); 
            }
            
            alienFleet.LoadLevel(currentLevel);
        }
    }
}

    public void LoseLife()
    {
        currentLives--;
        livesText.text = "LIVES: " + currentLives;

        if (currentLives <= 0) TriggerGameOver();
    }

    public void TriggerGameOver()
    {
        IsPlaying = false;
        isVictory = false;
        Time.timeScale = 0; 
        SetScreenEmpty(true);
        
        messageText.text = "GAME OVER\nPRESS SPACE TO RESTART";
        messageText.gameObject.SetActive(true);
    }

    public void TriggerVictory()
    {
        IsPlaying = false;
        isVictory = true;
        Time.timeScale = 0; 
        SetScreenEmpty(true);

        if (currentLevel < totalLevels) //prevents the game looping forever
            {
                messageText.text = "VICTORY!\nPRESS SPACE FOR LEVEL " + (currentLevel + 1);
            } 
            else 
            {
                messageText.text = "GALAXY SAVED!\nPRESS SPACE TO RESTART GAME";
            }
        messageText.gameObject.SetActive(true);
    }

    private void SetScreenEmpty(bool isEmpty)
    {
        // Toggles the GameObjects off when isEmpty is true, and on when false
        if (player != null) player.SetActive(!isEmpty);
        if (livesText != null) livesText.gameObject.SetActive(!isEmpty);
        if (alienFleet != null) alienFleet.gameObject.SetActive(!isEmpty);
    }
}