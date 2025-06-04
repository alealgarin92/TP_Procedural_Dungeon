// DungeonGenerator.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] private Vector2 _dungeonSize;
    [SerializeField] private int _startPos = 0;
    [SerializeField] private GameObject[] _rooms;
    [SerializeField] private Vector2 _offset;

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
    }

    private void InstantiateRooms()
    {
        var board = _boardManager.GetBoard();

        for (int i = 0; i < _dungeonSize.x; i++)
        {
            for (int j = 0; j < _dungeonSize.y; j++)
            {
                int index = Mathf.FloorToInt(i + j * _dungeonSize.x);
                ICell currentCell = board[index];
                if (currentCell.Visited)
                {
                    GameObject newRoom = _roomFactory.CreateRoom(
                        new Vector3(i * _offset.x, 0f, -j * _offset.y),
                        Quaternion.identity,
                        currentCell
                    );
                    // Ensure room naming matches original format: "Room X-Y"
                    newRoom.name = "Room " + i + "-" + j;
                }
            }
        }
    }

    private void OnGUI()
    {
        float w = Screen.width / 2;
        float h = Screen.height - 80;
        if (GUI.Button(new Rect(w, h, 250, 50), "Regenerate Dungeon"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}