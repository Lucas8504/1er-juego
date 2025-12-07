using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cargar escenas

public class MenuManager : MonoBehaviour
{
    // Define aquí el nombre de la escena de tu juego principal (donde está BoardManager)
    // Por defecto, asumiremos que se llama "GameScene" o "Level1" si no se ha renombrado.
    public string nombreEscenaDeJuego = "GameScene";

    // Función que se llamará al hacer clic en "INICIAR JUEGO"
    public void IniciarJuego()
    {
        // Carga la escena de juego.
        SceneManager.LoadScene(nombreEscenaDeJuego);
    }

    // Función que se llamará al hacer clic en "SALIR"
    public void SalirDelJuego()
    {
        // Sale de la aplicación (solo funciona en builds, no en el editor)
        Application.Quit();

        // Código para detener el juego en el editor de Unity (opcional, para testeo)
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}