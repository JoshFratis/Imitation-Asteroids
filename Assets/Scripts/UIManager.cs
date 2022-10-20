using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameManager gameManager;

    [SerializeField] Text levelText;
    [SerializeField] Text scoreText;
    [SerializeField] Text livesText;
    [SerializeField] GameObject gameTextObject;
    private Text gameText;
 
    void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        gameText = gameTextObject.GetComponent<Text>();
    }

    void Start()
    {
        levelText.text = "Level: ";
        scoreText.text = "Score: ";
        livesText.text = "Lives: ";

        StartCoroutine(Flash());
    }

    public void UpdateLevel(int level)
    {
        levelText.text = "Level: " + level;
    }

    public void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score;
    }

    public void UpdateLives(int lives)
    {
        livesText.text = "Lives: " + lives;
    }

    public void StartGame()
    {
        gameText.text = "";
    }

    public void EndLevel()
    {
        gameText.text = "Press Space to Start";
    }

    IEnumerator Flash()
    {
        while(true)
        {
            if (gameTextObject.activeSelf == true)
            {
                gameTextObject.SetActive(false);
            }
            else if (gameTextObject.activeSelf == false)
            {
                gameTextObject.SetActive(true);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}
