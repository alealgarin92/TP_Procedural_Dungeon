// BoardManager.cs
using System.Collections.Generic;
using UnityEngine;

public class BoardManager
{
    private readonly List<ICell> _board;
    private readonly Vector2 _dungeonSize;
    private readonly ICellFactory _cellFactory;

    public BoardManager(Vector2 dungeonSize, ICellFactory cellFactory)
    {
        _dungeonSize = dungeonSize;
        _cellFactory = cellFactory;
        _board = new List<ICell>();
        InitializeBoard();
    }

    private void InitializeBoard()
    {
        float boardLength = _dungeonSize.x * _dungeonSize.y;
        for (int i = 0; i < boardLength; i++)
        {
            _board.Add(_cellFactory.CreateCell());
        }
    }

    public List<ICell> GetBoard() => _board;
    public Vector2 GetDungeonSize() => _dungeonSize;
}