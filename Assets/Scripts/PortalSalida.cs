using UnityEngine;
using UnityEngine.SceneManagement;

namespace AdvancedRogueLikeandPuzzleSystem
{
    public class PortalSalida : MonoBehaviour
    {
        [Header("Configuración")]
        public int RequiredKeyID = 0;
        public string nextSceneName = "Nivel2";

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var hero = HeroController.instance;
                if (hero != null && hero.Keys_Grabbed.Contains(RequiredKeyID))
                {
                    Debug.Log("✅ Pasás de nivel con la llave " + RequiredKeyID);
                    SceneManager.LoadScene(nextSceneName);
                }
                else
                {
                    Debug.Log("❌ Te falta la llave ID " + RequiredKeyID);
                    GameCanvas_Controller.instance.Show_Grabbed_Text("🔒 Te falta la llave");
                    //AudioManager.instance.Play_Error();
                }
            }
        }
    }
}

