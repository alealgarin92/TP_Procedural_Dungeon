using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using Unity.AI.Navigation;

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

    private BoardManager _boardManager;
    private MazeGeneratorService _mazeGenerator;
    private IRoomFactory _roomFactory;

    void Start()
    {
        InitializeComponents();
        GenerateDungeon();
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

        // 🧠 Bakear NavMesh después de colocar habitaciones
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

                    // Ítems obligatorios en el primer room
                    if (roomIndex == _startPos)
                    {
                        foreach (GameObject item in firstRoomCollectibles)
                        {
                            Vector3 spawnPos = roomPos + RandomOffset();
                            SpawnOnGround(item, spawnPos, newRoom.transform);
                        }
                    }
                    else
                    {
                        float chance = UnityEngine.Random.value;

                        // Enemigo aleatorio
                        if (chance < 0.3f && enemyPrefabs.Length > 0)
                        {
                            GameObject randomEnemy = enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Length)];
                            Vector3 spawnPos = roomPos + RandomOffset();
                            SpawnOnGround(randomEnemy, spawnPos, newRoom.transform);
                        }
                        // Ítem aleatorio
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

    /*private void OnGUI()
    {
        float w = Screen.width / 2;
        float h = Screen.height - 80;
        if (GUI.Button(new Rect(w, h, 250, 50), "Regenerate Dungeon"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }*/
}
