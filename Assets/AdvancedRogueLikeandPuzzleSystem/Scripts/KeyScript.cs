using UnityEngine;

namespace AdvancedRogueLikeandPuzzleSystem
{
    public class KeyScript : MonoBehaviour
    {
        [SerializeField] private int keyID = 0;
        [SerializeField] private string keyName = "Llave";

        public int Key_ID
        {
            get => keyID;
            set => keyID = value;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                GrabIt();
            }
        }

        public void GrabIt()
        {
            if (HeroController.instance != null)
            {
                HeroController.instance.Keys_Grabbed.Add(keyID);
                Debug.Log($"✅ Llave {keyName} (ID: {keyID}) recogida.");
            }
            else
            {
                Debug.LogError("⚠️ HeroController.instance no está inicializado.");
                return;
            }

            if (GameCanvas_Controller.instance != null)
            {
                GameCanvas_Controller.instance.Show_Grabbed_Text(keyName);
            }
            else
            {
                Debug.LogWarning("⚠️ GameCanvas_Controller.instance no está inicializado.");
            }

            if (AudioManager.instance != null)
            {
                AudioManager.instance.Play_Grab();
            }
            else
            {
                Debug.LogWarning("⚠️ AudioManager.instance no está inicializado.");
            }

            Destroy(gameObject);
        }
    }
}