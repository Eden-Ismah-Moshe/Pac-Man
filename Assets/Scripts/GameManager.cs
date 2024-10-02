using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Ghost[] ghosts;// Array to store all ghosts in the game
    [SerializeField] private Pacman pacman;// Reference to the Pacman player
    [SerializeField] private Transform pellets;// Container for all pellet objects
    [SerializeField] private TextMeshProUGUI _gameOverText;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _livesText;
    public int score { get; private set; } = 0;

    public int lives { get; private set; } = 3; 
    
    private int ghostMultiplier = 1;
    private bool isNewGame = true; // To check if it is a new game or a new level
    
    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Keep the GameManager across scenes
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) {
            Instance = null;
        }
    }

    private void Start()
    {
        NewGame();
    }

    private void Update()
    {
        if (lives <= 0 && Input.anyKeyDown) {
            NewGame();
        }
    }

    private void NewGame()
    {

        if (isNewGame)
        {
            SetScore(0);  // Only reset score for new games, not for level transitions
        }
        SetLives(3);
        NewRound();
        isNewGame = false; // Once the game starts, it's no longer a new game until it's reset
    }

    private void NewRound()
    {
        _gameOverText.enabled = false;

        foreach (Transform pellet in pellets)
        {
            pellet.gameObject.SetActive(true); // Reactivate all pellets
        }

        ResetState(); // Reset the state for ghosts and Pacman
    }

    private void ResetState()
    {
        for (int i = 0; i < ghosts.Length; i++) {
            ghosts[i].ResetState();
        }

        pacman.ResetState();
      // pacman.gameObject.SetActive(true);
    }

    private void GameOver()
    {
        _gameOverText.enabled = true;
        
        if(score > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
        
        isNewGame = true;  // The next game will be a new game
        SceneManager.LoadScene(0);
    }

    private void SetLives(int lives)
    {
        this.lives = lives;
        _livesText.text = "Lives: " + lives.ToString();
        
        if (lives <= 0)
        {
            GameOver(); 
        }
    }

    private void SetScore(int score)
    {
        this.score = score;
        _scoreText.text = "Score: " + score.ToString().PadLeft(2, '0');
    }

    public void PacmanEaten()
    {
       pacman.DeathSequence();
       //pacman.gameObject.SetActive(false);

        SetLives(lives - 1);

        if (lives > 0) {
            Invoke(nameof(ResetState), 3f);
        } else {
            GameOver();
        }
    }

    public void GhostEaten(Ghost ghost)
    {
        int points = ghost.points * ghostMultiplier;
        SetScore(score + points);

        ghostMultiplier++;
    }

    public void PelletEaten(Pellet pellet)
    {
        pellet.gameObject.SetActive(false);

        SetScore(score + pellet.points);

        if (!HasRemainingPellets())
        {
          //  pacman.gameObject.SetActive(false);
          Invoke(nameof(NextLevel), 3f); // Wait 3 seconds before loading the next level

        }
    }
    
    public void PowerPelletEaten(PowerPellet pellet)
    {
        for (int i = 0; i < ghosts.Length; i++) {
            ghosts[i].frightened.Enable(pellet.duration);
        }

        PelletEaten(pellet);
        CancelInvoke(nameof(ResetGhostMultiplier));
        Invoke(nameof(ResetGhostMultiplier), pellet.duration);
    }

    private bool HasRemainingPellets()
    {
        if (pellets == null)
        {
            return false;
        }
    
        bool foundPellets = false;

        foreach (Transform pellet in pellets)
        {
            if (pellet.gameObject.activeSelf)
            {
                foundPellets = true;
            }
        }

        return foundPellets;
    }

    private void ResetGhostMultiplier()
    {
        ghostMultiplier = 1;
    }
    
    private void NextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (currentSceneIndex == 1)  
        {
            SceneManager.LoadScene(2); 
            StartCoroutine(AssignLevelObjects()); // Wait for the scene to load

        }
        else if (currentSceneIndex == 2)  
        {
            if (!HasRemainingPellets() || lives <= 0)  
            {
                GameOver();
            }
            else
            {
                return;
            }
        }
      
    }

    private IEnumerator AssignLevelObjects()
    {
        yield return new WaitForSeconds(0.5f); // Short delay to allow scene load

        pacman = FindObjectOfType<Pacman>();
        ghosts = FindObjectsOfType<Ghost>();
        pellets = GameObject.Find("Pellets")?.transform;
        
        // Reassign the UI references after a scene loads
        _gameOverText = GameObject.Find("GameOverText").GetComponent<TextMeshProUGUI>();
        _gameOverText.enabled = false;
        
        _scoreText = GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        _scoreText.enabled = true;
        
        _livesText = GameObject.Find("LivesText").GetComponent<TextMeshProUGUI>();
        this.lives = lives;
        _livesText.text = "Lives: " + lives.ToString();
        _livesText.enabled = true;

        ResetState();
    }

}
