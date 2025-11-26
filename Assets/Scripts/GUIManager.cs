using JetBrains.Annotations;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // 1. IMPORTANTE: Necesario para reiniciar la escena

public class GUIManager: MonoBehaviour
{
    public Text movesText, scoreText,multiScoreText;
    private int movesCounter;
    private int score;
    public int multiScore = 1;

    [Header("Game Over UI")] // Ayuda a organizar el inspector
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    public Button reiniciarButton;
    public Button menuButton;



    public void AddScore(int amount)
    {
        
        
        score += amount * multiScore;
        scoreText.text = "Score: " + score;
    }

    // función para subir el combo
    public void IncreaseMultiplier()
    {
        MultiScore++;
    }

    public void ResetMultiplier()
    {
        MultiScore = 1;
    }

    public int MultiScore
    {
        get { return multiScore; }

        set
        {
            multiScore = value;
            multiScoreText.text = "Combos: " + multiScore;
        }
    }

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


        // Asegurarse de que el panel esté oculto al inicio
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // Asignar funciones a los botones si están asignados en el inspector
        if (reiniciarButton != null)
            reiniciarButton.onClick.AddListener(RestartGame);

        if (menuButton != null)
            menuButton.onClick.AddListener(ReturnToMenu);

    }



    private IEnumerator GameOver()
    {
        yield return new WaitUntil(() => BoardManager.sharedInstance.isShifting);
        yield return new WaitForSeconds(0.25f);

        // Activar el panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            // Mostrar puntaje final
            if (gameOverText != null)
            {
                gameOverText.text = "¡Juego Terminado!\nPuntaje: " + score;
            }
        }

    }

    // Función para reiniciar el nivel actual
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Función para volver al menú
    public void ReturnToMenu()
    {
        // Asegúrate de cambiar "MenuPrincipal" por el nombre real de tu escena de menú
        SceneManager.LoadScene("MenuPrincipal");
    }


    private void Update()
    {
        
    }

}
