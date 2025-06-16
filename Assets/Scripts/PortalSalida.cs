using UnityEngine;
using UnityEngine.SceneManagement;

namespace AdvancedRogueLikeandPuzzleSystem
{
    public class PortalSalida : MonoBehaviour
    {
        [Header("Configuración")]
        [SerializeField] private int requiredKeyID = 0;
        [SerializeField] private string nextSceneName = "Nivel2";
        [SerializeField] private ParticleSystem successEffect;
        [SerializeField] private ParticleSystem errorEffect;
        [SerializeField] private AudioClip successSound;
        [SerializeField] private AudioClip errorSound;

        public int RequiredKeyID
        {
            get => requiredKeyID;
            set => requiredKeyID = value;
        }

        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                HeroController hero = HeroController.instance;
                if (hero == null)
                {
                    Debug.LogError("⚠️ HeroController.instance no está inicializado.");
                    return;
                }

                if (hero.Keys_Grabbed.Contains(requiredKeyID))
                {
                    Debug.Log($"✅ Pasás de nivel con la llave {requiredKeyID}");
                    PlayFeedback(successEffect, successSound);
                    if (!string.IsNullOrEmpty(nextSceneName))
                    {
                        SceneManager.LoadScene(nextSceneName);
                    }
                    else
                    {
                        Debug.LogWarning("⚠️ No se especificó el nombre de la siguiente escena.");
                    }
                }
                else
                {
                    Debug.Log($"❌ Te falta la llave ID {requiredKeyID}");
                    PlayFeedback(errorEffect, errorSound);
                    if (GameCanvas_Controller.instance != null)
                    {
                        GameCanvas_Controller.instance.Show_Grabbed_Text("🔒 Te falta la llave");
                    }
                    else
                    {
                        Debug.LogWarning("⚠️ GameCanvas_Controller.instance no está inicializado.");
                    }
                }
            }
        }

        private void PlayFeedback(ParticleSystem effect, AudioClip sound)
        {
            if (effect != null)
            {
                effect.Play();
            }
            if (sound != null && audioSource != null)
            {
                audioSource.PlayOneShot(sound);
            }
        }
    }
}