using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text scoreText;

    [SerializeField] private Text gameOverText;
    [SerializeField] private Text restartText;

    [SerializeField] private Sprite[] liveSprites;
    [SerializeField] private Image livesImg;


    // Start is called before the first frame update
    void Start()
    {
        UpdateScore(0);;
    }

    public void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score;
    }

    public void UpdateLives(int lifePoints)
    {
        if (lifePoints <= 0)
        {
            lifePoints = 0;
            GameOverSequence();
        }
            
        livesImg.sprite = liveSprites[lifePoints];
    }

    void GameOverSequence()
    {
        GameManager.instance.GameOver();
        gameOverText.gameObject.SetActive(true);
        restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());
    }

    IEnumerator GameOverFlickerRoutine()
    {
        while(true)
        {
            gameOverText.text = "GAME OVER";
            yield return new WaitForSeconds(0.5f);
            gameOverText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }
}
