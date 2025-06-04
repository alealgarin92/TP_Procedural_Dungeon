// RoomFactory.cs
using UnityEngine;

public class RoomFactory : IRoomFactory
{
    private readonly GameObject[] _roomPrefabs;

    public RoomFactory(GameObject[] roomPrefabs)
    {
        _roomPrefabs = roomPrefabs;
    }

    public GameObject CreateRoom(Vector3 position, Quaternion rotation, ICell cell)
    {
        int randomRoom = Random.Range(0, _roomPrefabs.Length);
        GameObject newRoom = Object.Instantiate(_roomPrefabs[randomRoom], position, rotation);
        RoomBehaviour rb = newRoom.GetComponent<RoomBehaviour>();
        rb.UpdateRoom(cell.Status);
        return newRoom;
    }
}