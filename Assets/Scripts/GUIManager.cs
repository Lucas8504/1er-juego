using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager: MonoBehaviour
{
    public Text movesText, scoreText;
    private int movesCounter;
    private int score;


    public int MovesCounter
    {
        get { return movesCounter; }
        set
        {
            movesCounter = value;
            movesText.text = "Moves: " + movesCounter;
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

    private void Update()
    {
        
    }

}
