using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cargar escenas

public class MenuManager : MonoBehaviour
{
   
    
    public string nombreEscenaDeJuego = "SampleScene";

    
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