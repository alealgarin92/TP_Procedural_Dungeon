#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuDerrota : MonoBehaviour
{
    [SerializeField] private GameObject pantallaMenuDerrota;
    [SerializeField] private Button restartButton, mainMenuButton, quitButton;

    void Start()
    {
        restartButton.onClick.AddListener(OnRestartButtonPressed);
        mainMenuButton.onClick.AddListener(OnMainMenuButtonPressed);
        quitButton.onClick.AddListener(OnQuitButtonPressed);
    }

    public void OnRestartButtonPressed()
    {
        // Restaurar el tiempo y recargar la escena actual
        Time.timeScale = 1f;

        // Asegúrate de ocultar el menú de derrota antes de recargar
        pantallaMenuDerrota.SetActive(false);

        // Obtener y recargar la escena actual
        string currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"Recargando la escena: {currentSceneName}");

        //GameManager.instance.SetCurrentHealth(100);
        //GameManager.instance.SetCurrentBatery(100);
        SceneManager.LoadScene(currentSceneName);
    }

    public void OnMainMenuButtonPressed()
    {
        // Restaurar el tiempo y recargar la escena actual
        Time.timeScale = 1f;

        pantallaMenuDerrota.SetActive(false);
        // Restaurar el tiempo y cargar el menú principal
        Debug.Log("MainMenu");

        //GameManager.instance.SetCurrentHealth(100);
        //GameManager.instance.SetCurrentBatery(100);
        SceneManager.LoadScene("MenuInicio");

    }

    public void OnQuitButtonPressed()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();

#if UNITY_EDITOR
        EditorApplication.ExitPlaymode(); //Solo editor, no funciona en la build!!!
#endif
    }
}