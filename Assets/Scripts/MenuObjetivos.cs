using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_Objetivos : MonoBehaviour
{
    public GameObject MenuObjetivos;
    public bool Objetivos = false;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            if (Objetivos == false)
            {
                MenuObjetivos.SetActive(true);
                Objetivos = true;

                Time.timeScale = 0;
            }
    }
}