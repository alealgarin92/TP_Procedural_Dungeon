using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class pantallaControles : MonoBehaviour
{

    [SerializeField] private GameObject ObjetoControles;
    [SerializeField] private Button controlesButton;

    void Start()
    {
        controlesButton.onClick.AddListener(OnControlsButtonPressed);
    }

    public void OnControlsButtonPressed()
    {
        // Asegúrate de ocultar el menú de derrota antes de recargar
        ObjetoControles.SetActive(false);
    }
}
