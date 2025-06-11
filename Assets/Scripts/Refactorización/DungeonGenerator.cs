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
    [SerializeField] private GameObject[] enemyPrefab;

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
                    Vector3 position = new Vector3(i * _offset.x, 0f, -j * _offset.y);
                    GameObject newRoom = _roomFactory.CreateRoom(position, Quaternion.identity, currentCell);
                    newRoom.name = "Room " + i + "-" + j;

                    // Ítems obligatorios en el primer room
                    if (roomIndex == _startPos)
                    {
                        foreach (GameObject item in firstRoomCollectibles)
                        {
                            Instantiate(item, position + RandomOffset(), Quaternion.identity, newRoom.transform);
                        }
                    }
                    else
                    {
                        float chance = UnityEngine.Random.value;

                        // Enemigo aleatorio
                        if (chance < 0.3f && enemyPrefab.Length > 0)
                        {
                            GameObject randomEnemy = enemyPrefab[UnityEngine.Random.Range(0, enemyPrefab.Length)];
                            Vector3 spawnPos = position + RandomOffset();

                            if (NavMesh.SamplePosition(spawnPos, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
                            {
                                Instantiate(randomEnemy, hit.position, Quaternion.identity, newRoom.transform);
                            }
                        }
                        // Ítem aleatorio
                        else if (chance < 0.6f && randomCollectibles.Length > 0)
                        {
                            GameObject item = randomCollectibles[UnityEngine.Random.Range(0, randomCollectibles.Length)];
                            Instantiate(item, position + RandomOffset(), Quaternion.identity, newRoom.transform);
                        }
                    }

                    roomIndex++;
                }
            }
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
