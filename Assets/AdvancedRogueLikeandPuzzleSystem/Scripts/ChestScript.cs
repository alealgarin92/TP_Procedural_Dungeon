using System.Collections.Generic;
using UnityEngine;

namespace AdvancedRogueLikeandPuzzleSystem
{
    public class ChestScript : MonoBehaviour
    {
        public bool isInteractable = false;
        public List<GameObject> hiddenObjects;
        public GameObject keyPrefab;        // arrastr� el prefab de la llave ac�
        public Transform keySpawnPoint;

        void Update()
        {
            if (isInteractable && GameManager.Instance.controllerType == ControllerType.KeyboardMouse)
            {
                if (Input.GetKeyUp(GameManager.Instance.Keycode_Interact) || Input.GetButtonUp("Interact"))
                {
                    GetComponent<Animation>().Play("Sandik_OpeningAnimation");
                    HeroController.instance.HideInteractSprite();
                    foreach (var item in hiddenObjects)
                    {
                        item.SetActive(true);
                    }
                    AudioManager.instance.Play_Chest_Open();
                    GetComponent<Collider>().enabled = false;
                    this.enabled = false;
                }
            }
        }

        public void Open()
        {
            if (isInteractable)
            {
                GetComponent<Animation>().Play("Sandik_OpeningAnimation");
                HeroController.instance.HideInteractSprite();
                if (keyPrefab != null && keySpawnPoint != null)
                {
                    GameObject keyInstance = Instantiate(keyPrefab, keySpawnPoint.position, keySpawnPoint.rotation);
                }
                else
                {
                    Debug.LogWarning("No se asign� la llave o el punto de aparici�n.");
                }
                AudioManager.instance.Play_Chest_Open();
                GetComponent<Collider>().enabled = false;
                this.enabled = false;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isInteractable = true;
                HeroController.instance.ShowInteractSprite();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isInteractable = false;
                HeroController.instance.HideInteractSprite();
            }
        }
    }
}