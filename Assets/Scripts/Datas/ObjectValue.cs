using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectValue : MonoBehaviour
{
    [SerializeField] float _value;

    public float Value
    {
        get { return _value; }
        private set { _value = value; }
    }

    public float ModifiedValue { get; internal set; } // <- Renombrado para evitar conflicto con el nombre de la clase
}