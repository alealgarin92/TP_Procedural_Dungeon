#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuInicial : MonoBehaviour
{
    public GameObject panelBotones;
    public Image cutsceneImage;
    public Sprite[] cutsceneSprites; // Asignar 6 imágenes en el inspector
    public float imageDuration = 5f;
    public float fadeDuration = 1f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        cutsceneImage.gameObject.SetActive(false);
    }

    public void Jugar()
    {
        panelBotones.SetActive(false);
        StartCoroutine(PlayCutscene());
    }

    public void Salir()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
#if UNITY_EDITOR 
        EditorApplication.ExitPlaymode();
#endif
    }

    private IEnumerator PlayCutscene()
    {
        cutsceneImage.gameObject.SetActive(true);
        Color color = cutsceneImage.color;

        for (int i = 0; i < cutsceneSprites.Length; i++)
        {
            cutsceneImage.sprite = cutsceneSprites[i];

            // Fade in
            float t = 0;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                color.a = Mathf.Lerp(0, 1, t / fadeDuration);
                cutsceneImage.color = color;
                yield return null;
            }

            yield return new WaitForSeconds(imageDuration);

            // Fade out
            t = 0;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                color.a = Mathf.Lerp(1, 0, t / fadeDuration);
                cutsceneImage.color = color;
                yield return null;
            }
        }

        // Cargar escena
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
