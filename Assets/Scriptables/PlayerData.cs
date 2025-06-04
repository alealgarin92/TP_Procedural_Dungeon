using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "CreatePlayerData", order = 0)]
public class PlayerData : ScriptableObject
{
    public float health;
    public float score;
    public Vector3 lastPosition;

}
