using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _highestScoreText;
    public void Start()
    {
        int highestScore = PlayerPrefs.GetInt("HighScore", 0);
        _highestScoreText.text = "Highest Score: " + highestScore;
    }
    
    public void OnPlayButton()
    {
       SceneManager.LoadScene(1);
    }
    
    public void OnQuitButton()
    {
       Application.Quit();
    }
}
