using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    // Control
    public PlayerInput controls;
    private InputAction confirm;
    bool gameStarted = false;
    bool gameTransitioning = false;

    // Game Manager Variables
    private int level;
    public int score;
    private int asteroidCount;

    // Objects
    public List<GameObject> asteroids;
    public List<SaucerSpawner> saucerSpawners;
    
    // Scripts
    private UIManager uiManager;
    private Ship playerShipScript;

    // Prefabs
    [SerializeField] GameObject oAsteroid;
    [SerializeField] GameObject playerShip;

    // Screen Coordinates
    private float xExtent;
    private float yExtent;
    private float leftBound;
    private float rightBound;
    private float upperBound;
    private float lowerBound;

    // START UP
    void Awake()
    {
        // Initialize Controls
        controls = new PlayerInput();
        
        // Get Scripts 
        uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
        playerShipScript = playerShip.GetComponent<Ship>();
    }
    
    void Start()
    {
        // Initialize Lists
        List<GameObject> asteroids = new List<GameObject>();
        List<SaucerSpawner> saucerSpawners = new List<SaucerSpawner>();

        foreach(SaucerSpawner saucerSpawner in saucerSpawners)
        {
            saucerSpawner.enabled = false;
        }

        // Screen Coordinates
        yExtent = Camera.main.GetComponent<Camera>().orthographicSize; 
        xExtent = yExtent / Screen.height * Screen.width;
        leftBound = -xExtent;
        rightBound = xExtent;
        lowerBound = -yExtent;
        upperBound = yExtent;
    }

    void OnEnable()
    {
        // Enable 'Press Space to Start'
        confirm = controls.Game.Confirm;
        confirm.Enable();
        confirm.performed += Confirm;
    }

    void OnDisable()
    {
        // Disable 'Press Space to Start'
        confirm.Disable();
        confirm.performed -= Confirm;
    }

    // NEW GAME
    // 'Press Space to Start'
    void Confirm(InputAction.CallbackContext context)
    {
        StartGame();
    }

    void StartGame()
    {
        // Initialize Variables
        gameStarted = true;
        score = 0;
        level = 0;

        // Activate / Update UI
        uiManager.StartGame();
        uiManager.UpdateScore(score);

        // Activate Player Ship
        playerShip.SetActive(true);

        // Deactivate Saucer Spawners
        // if any are left active from the last game
        foreach (SaucerSpawner saucerSpawner in saucerSpawners)
        {
            saucerSpawner.enabled = false;
        }

        // Disable 'Press Space to Start'
        confirm.Disable();
        confirm.performed -= Confirm;

        StartLevel();
    }

    // NEW LEVEL
    void StartLevel()
    {
        gameTransitioning = false;

        // Update Level
        level++;
        uiManager.UpdateLevel(level);

        // Re-Initialize Asteroids (for game restart)
        foreach(GameObject asteroid in asteroids)
        {
            Destroy(asteroid);
        }
        asteroids.Clear();

        // Spawn Asteroids
        if (score < 60000)
        {
            asteroidCount = Mathf.Min(2 + (level * 2), 11);
        }

        float posX;
        float posY;

        for (var i = 0; i < asteroidCount; i++)
        {
            if (FlipCoin()) // Spawn on the left or right
            { 
                // Position Y
                posY = Random.Range(lowerBound, upperBound);

                // Position X
                float side = Mathf.Sign(Random.Range(-1, 1));
                posX = side * (xExtent);// - Random.Range(0f, 25f));

                if (side == -1) Debug.Log("Spawning asteroid on left");
                else Debug.Log("Spawning asteroid on right");
            }
            else // Spawn on the top or bottom
            {
                // Position X
                posX = Random.Range(lowerBound, upperBound);

                // Position Y
                float side = Mathf.Sign(Random.Range(-1, 1));
                posY = side * (yExtent);// - Random.Range(0f, 25f));

                if (side == -1) Debug.Log("Spawning asteroid on bottom");
                else Debug.Log("Spawning asteroid on top");
            }

            GameObject asteroid = Instantiate(oAsteroid) as GameObject;
            asteroid.GetComponent<Transform>().position = new Vector3(posX, posY, 0);
            Debug.Log("Spawning asteroid at " + asteroid.GetComponent<Transform>().position);
            //asteroid.GetComponent<Transform>().position = new Vector3(Random.Range(-40,40), Random.Range(-20,20), 0);
            asteroids.Add(asteroid);
        }
    }

    // GAMEPLAY
    public void AddScore(int newScore)
    {
        foreach (SaucerSpawner saucerSpawner in saucerSpawners)
        {
            // Activate on entering score threshold
            if (score < saucerSpawner.scoreThresholdLower)
            {
                if (score + newScore >= saucerSpawner.scoreThresholdLower)
                {
                    Debug.Log("Enabling saucer spawner on entering lower score threshold: " + saucerSpawner.scoreThresholdLower);
                    saucerSpawner.enabled = true;
                }
            }
            
            // Deactivate on exiting score threshold
            if (score < saucerSpawner.scoreThresholdUpper)
            {
                if (score + newScore >= saucerSpawner.scoreThresholdUpper)
                {
                    Debug.Log("Disabling saucer spawner on exiting upper score threshold: " + saucerSpawner.scoreThresholdUpper);
                    saucerSpawner.enabled = false;
                }
            }
        }

        // 1UP for every 10,000 points
        if ((score / 10000) != ((score + newScore) / 10000))
        {
            playerShipScript.AddLives(1);
        }

        // Update Score
        score += newScore;
        score %= 99990;
        uiManager.UpdateScore(score);
    }

    public void AddAsteroid(GameObject asteroid)
    {
        asteroids.Add(asteroid);
    }

    void Update()
    {
        if (gameStarted)
        {
            // Check for asteroids
            bool noAsteroids = true;
            foreach(GameObject asteroid in asteroids)
            {
                if (asteroid.activeSelf == true)
                {
                    noAsteroids = false;
                }
            }

            // Check for saucers
            bool noSaucers = true;
            foreach (SaucerSpawner saucerSpawner in saucerSpawners)
            {
                if (saucerSpawner.saucerObject.activeSelf == true)
                {
                    noSaucers = false;
                }
            }

            // Next Level
            if ((noAsteroids == true) && (noSaucers == true) && (gameTransitioning == false))
            {
                gameTransitioning = true;
                StartCoroutine(NextLevel());
            } 

            // Game Over
            if (playerShip.activeSelf == false)
            {
                EndLevel();
            }
        }
    }

    // GAME OVER
    void EndLevel()
    {
        gameStarted = false;
        uiManager.EndLevel();
        StartCoroutine(RestartGame());
    }

    public bool FlipCoin()
    {
        if (Random.value < 0.5f)
        {
            Debug.Log("Heads!");
            return true;
        }
        else 
        {
            Debug.Log("Tails!");
            return false;
        }
    }

    IEnumerator NextLevel()
    {
        yield return new WaitForSeconds(3);
        StartLevel();
    }

    IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(3);

        // Enable 'Press Space to Start'
        confirm.Enable();
        confirm.performed += Confirm;
    }
}
