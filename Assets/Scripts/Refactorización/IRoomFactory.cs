// IRoomFactory.cs
using UnityEngine;

public interface IRoomFactory
{
    GameObject CreateRoom(Vector3 position, Quaternion rotation, ICell cell);
}