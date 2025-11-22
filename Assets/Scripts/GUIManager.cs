using JetBrains.Annotations;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager: MonoBehaviour
{
    public Text movesText, scoreText;
    private int movesCounter;
    private int score;
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    public Button reiniciarButton;
    public Button menuButton;

    private bool isGameOver = false;


    public int MovesCounter
    {
        get { return movesCounter; }
        set
        {
            movesCounter = value;
            movesText.text = "Moves: " + movesCounter;

            if (movesCounter <= 0)
            {

                movesCounter = 0;
                StartCoroutine(GameOver());

            }
        }
    }
    public int Score
    {
        get { return score; }
        set 
        { 
            score = value;
            scoreText.text = "Score: " + score;
        }
    }
    public static GUIManager sharedInstance;  

    void Start()
    {
        if (sharedInstance == null)
        {
            sharedInstance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }


        score = 0;
        movesCounter = 30;
        movesText.text = "Moves: " + movesCounter;
        scoreText.text = "Score: " + score;
    }

    private IEnumerator GameOver()
    {
        yield return new WaitUntil(() => BoardManager.sharedInstance.isShifting);
        yield return new WaitForSeconds(0.25f);
    }


    private void Update()
    {
        
    }

}
