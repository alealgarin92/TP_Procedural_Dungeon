using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class FinalCutsceneController : MonoBehaviour
{
    [Header("Imágenes con CanvasGroup (en orden de aparición)")]
    public CanvasGroup[] images;

    [Header("Duración por imagen (segundos)")]
    public float displayTime = 8f;

    [Header("Duración del fundido (segundos)")]
    public float fadeDuration = 1f;

    [Header("Audio de la cutscene")]
    public AudioSource cutsceneMusic;

    private void OnEnable()
    {
        if (cutsceneMusic != null)
        {
            cutsceneMusic.Play(); // Reproduce la música de la cutscene
        }

        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        foreach (CanvasGroup img in images)
        {
            yield return StartCoroutine(FadeIn(img));
            yield return new WaitForSecondsRealtime(displayTime);
            yield return StartCoroutine(FadeOut(img));
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuInicio");
    }

    private IEnumerator FadeIn(CanvasGroup canvas)
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            canvas.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
        canvas.alpha = 1f;
    }

    private IEnumerator FadeOut(CanvasGroup canvas)
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            canvas.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }
        canvas.alpha = 0f;
    }
}
