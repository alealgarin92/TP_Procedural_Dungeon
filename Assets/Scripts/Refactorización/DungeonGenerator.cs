using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using Unity.AI.Navigation;
using AdvancedRogueLikeandPuzzleSystem;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Configuración del Dungeon")]
    [SerializeField] private Vector2 _dungeonSize;
    [SerializeField] private int _startPos = 0;
    [SerializeField] private GameObject[] _rooms;
    [SerializeField] private Vector2 _offset;

    [Header("Spawneo de enemigos")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Spawneo de coleccionables aleatorios")]
    [SerializeField] private GameObject[] randomCollectibles;

    [Header("Ítems obligatorios del primer room")]
    [SerializeField] private GameObject[] firstRoomCollectibles;

    [Header("NavMesh")]
    [SerializeField] private NavMeshSurface navMeshSurface;

    [Header("Portal de salida")]
    [SerializeField] private GameObject exitPortalPrefab;
    [SerializeField] private int keyID = 1;

    [SerializeField] private GameObject keyPrefab;

    [Header("Objeto exclusivo del primer room")]
    [SerializeField] private GameObject firstRoomSpecialObject;

    private BoardManager _boardManager;
    private MazeGeneratorService _mazeGenerator;
    private IRoomFactory _roomFactory;
    private Vector3 lastRoomPosition;

    void Start()
    {
        InitializeComponents();

        if (DungeonCache.Instance != null && DungeonCache.Instance.DungeonAlreadyGenerated)
        {
            _boardManager = DungeonCache.Instance.BoardManagerCache;
            _roomFactory = new RoomFactory(_rooms);
            InstantiateRooms();
            if (navMeshSurface != null) navMeshSurface.BuildNavMesh();
        }
        else
        {
            GenerateDungeon();

            if (DungeonCache.Instance != null)
            {
                DungeonCache.Instance.BoardManagerCache = _boardManager;
                DungeonCache.Instance.DungeonAlreadyGenerated = true;
            }
        }
    }

    private void InitializeComponents()
    {
        _boardManager = new BoardManager(_dungeonSize, new BasicCellFactory());
        _mazeGenerator = new MazeGeneratorService(_boardManager, _startPos);
        _roomFactory = new RoomFactory(_rooms);
    }

    private void GenerateDungeon()
    {
        _mazeGenerator.GenerateMaze();
        InstantiateRooms();

        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
        }
        else
        {
            Debug.LogWarning("⚠️ No se asignó NavMeshSurface en el inspector.");
        }
    }

    private void InstantiateRooms()
    {
        var board = _boardManager.GetBoard();
        int roomIndex = 0;
        List<Vector3> roomPositions = new List<Vector3>();

        for (int i = 0; i < _dungeonSize.x; i++)
        {
            for (int j = 0; j < _dungeonSize.y; j++)
            {
                int index = Mathf.FloorToInt(i + j * _dungeonSize.x);
                ICell currentCell = board[index];
                if (currentCell.Visited)
                {
                    Vector3 basePosition = new Vector3(i * _offset.x, transform.position.y, -j * _offset.y);
                    GameObject newRoom = _roomFactory.CreateRoom(basePosition, Quaternion.identity, currentCell);
                    newRoom.name = "Room " + i + "-" + j;

                    Vector3 roomPos = newRoom.transform.position;
                    roomPositions.Add(roomPos);
                    lastRoomPosition = roomPos;

                    if (roomIndex == _startPos)
                    {
                        // Instanciar todos los objetos del primer room, juntos
                        float spacing = 2f; // distancia entre objetos
                        Vector3 origin = roomPos + Vector3.forward * 2f; // pequeña distancia hacia adelante para empezar

                        for (int k = 0; k < firstRoomCollectibles.Length; k++)
                        {
                            Vector3 offset = new Vector3((k % 3) * spacing, 0, (k / 3) * spacing);
                            Vector3 spawnPos = origin + offset;
                            SpawnOnGround(firstRoomCollectibles[k], spawnPos, newRoom.transform);
                        }
                        // 🧱 Instanciar objeto exclusivo del primer room
                        if (firstRoomSpecialObject != null)
                        {
                            Instantiate(firstRoomSpecialObject, roomPos, Quaternion.identity, newRoom.transform);
                        }

                    }

                    else
                    {
                        float chance = UnityEngine.Random.value;

                        if (chance < 0.3f && enemyPrefabs.Length > 0)
                        {
                            GameObject randomEnemy = enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Length)];
                            Vector3 spawnPos = roomPos + RandomOffset();
                            SpawnOnGround(randomEnemy, spawnPos, newRoom.transform);
                        }
                        else if (chance < 0.6f && randomCollectibles.Length > 0)
                        {
                            GameObject item = randomCollectibles[UnityEngine.Random.Range(0, randomCollectibles.Length)];
                            Vector3 spawnPos = roomPos + RandomOffset();
                            SpawnOnGround(item, spawnPos, newRoom.transform);
                        }
                    }

                    roomIndex++;
                }
            }
        }
    

    // 🔑 Instanciar la llave
    Vector3 keyRoomPosition = GetRandomRoomPositionExceptFirstAndLast();
        if (keyRoomPosition != Vector3.zero)
        {
            GameObject llave = Instantiate(keyPrefab, keyRoomPosition + Vector3.up * 0.5f, Quaternion.identity);
            KeyScript script = llave.GetComponent<KeyScript>();
            if (script != null)
            {
                script.Key_ID = keyID;
            }
            else
            {
                Debug.LogError("⚠️ El prefab de la llave no tiene el componente KeyScript.");
            }
            SpawnOnGround(llave, keyRoomPosition + RandomOffset(), transform);
        }
        else
        {
            Debug.LogError("⚠️ No se pudo encontrar una posición válida para la llave.");
        }

        // 🚪 Instanciar el portal de salida
        if (exitPortalPrefab != null && lastRoomPosition != Vector3.zero)
        {
            GameObject portaldeSalida = Instantiate(exitPortalPrefab, lastRoomPosition + Vector3.up * 0.5f, Quaternion.identity);
            PortalSalida salida = portaldeSalida.GetComponent<PortalSalida>();
            if (salida != null)
            {
                salida.RequiredKeyID = keyID;
            }
            else
            {
                Debug.LogError("⚠️ El prefab del portal no tiene el componente PortalSalida.");
            }
        }
        else
        {
            Debug.LogError("⚠️ No se asignó el prefab del portal de salida o no hay una posición válida.");
        }
    }

    private Vector3 GetRandomRoomPositionExceptFirstAndLast()
    {
        List<Vector3> validRoomPositions = new List<Vector3>();
        var board = _boardManager.GetBoard();
        for (int i = 0; i < _dungeonSize.x; i++)
        {
            for (int j = 0; j < _dungeonSize.y; j++)
            {
                int index = Mathf.FloorToInt(i + j * _dungeonSize.x);
                ICell currentCell = board[index];
                if (currentCell.Visited && index != _startPos)
                {
                    Vector3 roomPosition = new Vector3(i * _offset.x, transform.position.y, -j * _offset.y);
                    validRoomPositions.Add(roomPosition);
                }
            }
        }

        if (validRoomPositions.Count == 0)
        {
            Debug.LogError("⚠️ No hay habitaciones válidas para colocar la llave.");
            return Vector3.zero;
        }

        return validRoomPositions[UnityEngine.Random.Range(0, validRoomPositions.Count)];
    }

    private void SpawnOnGround(GameObject prefab, Vector3 position, Transform parent)
    {
        if (Physics.Raycast(position + Vector3.up * 5f, Vector3.down, out RaycastHit hit, 10f))
        {
            Instantiate(prefab, hit.point, Quaternion.identity, parent);
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró el piso para: " + prefab.name + " en posición: " + position);
        }
    }

    private Vector3 RandomOffset()
    {
        return new Vector3(
            UnityEngine.Random.Range(-1f, 1f),
            0f,
            UnityEngine.Random.Range(-1f, 1f)
        );
    }

    public void ResetDungeon()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        GenerateDungeon();
    }
}