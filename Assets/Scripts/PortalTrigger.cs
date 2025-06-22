using UnityEngine;

public class FinalPortalTrigger : MonoBehaviour
{
    [Header("Etiqueta del jugador")]
    public string playerTag = "Player";

    [Header("Referencia al Canvas de Fin de Juego")]
    private GameObject finalCanvas;

    private GameObject gamePlayPanel;

    private void Start()
    {
        finalCanvas = GameObject.FindWithTag("CanvasFinal");
        gamePlayPanel = GameObject.FindWithTag("GamePlayPanel");

        if (finalCanvas == null)
        {
            Debug.LogError("No se encontró ningún GameObject con el tag 'CanvasFinal'.");
        }
        else
        {
            finalCanvas.SetActive(false); // Ocultar al inicio
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (finalCanvas != null)
            {
                // Detener todos los sonidos activos
                AudioSource[] allAudioSources = Object.FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
                foreach (AudioSource source in allAudioSources)
                {
                    source.Stop(); // Detiene música y SFX
                }

                // Mostrar canvas
                Time.timeScale = 0f;
                finalCanvas.SetActive(true);
                gamePlayPanel.SetActive(false);
            }
        }
    }
}
