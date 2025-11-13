using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager: MonoBehaviour
{
    public Text movesText;
    private int movesCounter;
    void Start()
    {
        
        movesCounter = 30;
        movesText.text = "Moves: " + movesCounter;
    }

    private void Update()
    {
        
    }

}
