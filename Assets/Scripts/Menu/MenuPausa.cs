#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPausa : MonoBehaviour
{
    [SerializeField] private Button continueButton, restartButton, controlsButton, mainMenuButton, quitButton;
    [SerializeField] private GameObject ObjetoMenuPausa;
    [SerializeField] private GameObject pantallaControles;
    public bool pausa;

    // Start is called before the first frame update
    void Start()
    {
        continueButton.onClick.AddListener(OnContinueButtonPressed);
        restartButton.onClick.AddListener(OnRestartButtonPressed);
        controlsButton.onClick.AddListener(OnControlsButtonPressed);
        mainMenuButton.onClick.AddListener(OnMainMenuButtonPressed);
        quitButton.onClick.AddListener(OnQuitButtonPressed);
    }

    // Update is called once per frame
    public void OnContinueButtonPressed()
    {
        Debug.Log("Continuando juego desde el boton");
        ObjetoMenuPausa.SetActive(false);
        pausa = false;
        //Camera.main.GetComponent<CameraOrbit>().enabled = true;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        ResetEventSystem();
    }

    public void OnControlsButtonPressed()
    {
        Debug.Log("Controls");
        pantallaControles.SetActive(true);
    }

    private void ResetEventSystem()
    {
        EventSystem eventSystem = EventSystem.current;
        if (eventSystem != null)
        {
            // Borra cualquier selección previa
            eventSystem.SetSelectedGameObject(null); 
        }
    }

    public void OnRestartButtonPressed()
    {
        Debug.Log("Restart");

        //Application.Quit(); //Solo funciona en la build, no en el editor
        // Restaurar el tiempo y recargar la escena actual
        Time.timeScale = 1f;
        //GameManager.instance.SetCurrentHealth(100);
        //GameManager.instance.SetCurrentBatery(100);

        // Asegúrate de ocultar el menú de derrota antes de recargar
        ObjetoMenuPausa.SetActive(false);

        // Obtener y recargar la escena actual
        string currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"Recargando la escena: {currentSceneName}");

        //GameManager.instance.SetCurrentHealth(100);
        //GameManager.instance.SetCurrentBatery(100);
        SceneManager.LoadScene(currentSceneName);


    }

    private void OnMainMenuButtonPressed()
    {
        Debug.Log("MainMenu");
        SceneManager.LoadScene("MenuInicio");
    }

    private void OnQuitButtonPressed()
    {
        Debug.Log("Quit");
        Application.Quit();

#if UNITY_EDITOR 
        EditorApplication.ExitPlaymode(); //Solo editor, no funciona en la build!!!
#endif
    }




}

