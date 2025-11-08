using UnityEngine;
using UnityEngine.UI;

public class GUIManager: MonoBehaviour
{
    public Text movesText;
    private int moveCounter = 30;

    void Start()
    {
        moveCounter = 30;
        movesText.text = "Moves: " + moveCounter;
    }

    private void Update()
    {
        
    }


}
