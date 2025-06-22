using UnityEngine;

namespace AdvancedRogueLikeandPuzzleSystem
{
    public class CoinScript : MonoBehaviour
    {
        [SerializeField] private int coinValue = 1; // Cantidad de monedas que otorga
        [SerializeField] private string coinName = "Moneda"; // Nombre para mostrar en UI

        [Header("Movimiento de la moneda")]
        [SerializeField] private float rotationSpeed = 90f; // Grados por segundo
        [SerializeField] private float floatAmplitude = 0.5f; // Altura máxima de la flotación
        [SerializeField] private float floatFrequency = 1f; // Velocidad de la flotación

        private Vector3 initialPosition; // Posición inicial para la flotación
        private float timeOffset; // Desfase aleatorio para evitar que todas las monedas floten igual

        private void Start()
        {
            // Guardar la posición inicial y agregar un desfase aleatorio
            initialPosition = transform.position;
            timeOffset = Random.Range(0f, 2f * Mathf.PI);
        }

        private void Update()
        {
            // Rotación constante alrededor del eje Y
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

            // Flotación (movimiento sinusoidal en Y)
            float yOffset = Mathf.Sin((Time.time + timeOffset) * floatFrequency) * floatAmplitude;
            transform.position = initialPosition + new Vector3(0f, yOffset, 0f);
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
                HeroController.instance.UpdateCoin(coinValue);
                Debug.Log($"✅ Moneda '{coinName}' (Valor: {coinValue}) recogida.");
            }
            else
            {
                Debug.LogError("⚠️ HeroController.instance no está inicializado.");
                return;
            }

            if (GameCanvas_Controller.instance != null)
            {
                GameCanvas_Controller.instance.Show_Grabbed_Text($"{coinName} (+{coinValue})");
            }
            else
            {
                Debug.LogWarning("⚠️ GameCanvas_Controller.instance no está inicializado.");
            }

            if (AudioManager.instance != null)
            {
                AudioManager.instance.Play_Coin();
            }
            else
            {
                Debug.LogWarning("⚠️ AudioManager.instance no está inicializado.");
            }

            Destroy(gameObject);
        }
    }
}