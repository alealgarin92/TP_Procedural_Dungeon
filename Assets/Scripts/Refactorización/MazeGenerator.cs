// MazeGeneratorService.cs
using System.Collections.Generic;
using UnityEngine;

public class MazeGeneratorService
{
    private readonly BoardManager _boardManager;
    private readonly int _startPos;

    public MazeGeneratorService(BoardManager boardManager, int startPos)
    {
        _boardManager = boardManager;
        _startPos = startPos;
    }

    public void GenerateMaze()
    {
        var board = _boardManager.GetBoard();
        int currentCell = _startPos;
        Stack<int> path = new Stack<int>();
        int k = 0;

        while (k < 1000)
        {
            k++;
            board[currentCell].Visited = true;

            if (currentCell == board.Count - 1)
                break;

            List<int> neighbors = CheckNeighbors(currentCell);

            if (neighbors.Count == 0)
            {
                if (path.Count == 0)
                    break;
                currentCell = path.Pop();
            }
            else
            {
                path.Push(currentCell);
                int newCell = neighbors[Random.Range(0, neighbors.Count)];

                UpdateCellConnections(board, currentCell, newCell);
                currentCell = newCell;
            }
        }
    }

    private void UpdateCellConnections(List<ICell> board, int currentCell, int newCell)
    {
        var dungeonSize = _boardManager.GetDungeonSize();

        if (newCell > currentCell)
        {
            // Down or Right
            if (newCell - 1 == currentCell)
            {
                board[currentCell].Status[2] = true;
                board[newCell].Status[3] = true;
            }
            else
            {
                board[currentCell].Status[1] = true;
                board[newCell].Status[0] = true;
            }
        }
        else
        {
            // Up or Left
            if (newCell + 1 == currentCell)
            {
                board[currentCell].Status[3] = true;
                board[newCell].Status[2] = true;
            }
            else
            {
                board[currentCell].Status[0] = true;
                board[newCell].Status[1] = true;
            }
        }
    }

    private List<int> CheckNeighbors(int cell)
    {
        List<int> neighbors = new List<int>();
        var board = _boardManager.GetBoard();
        var dungeonSize = _boardManager.GetDungeonSize();

        // Check Up
        if (cell - dungeonSize.x >= 0 && !board[Mathf.FloorToInt(cell - dungeonSize.x)].Visited)
            neighbors.Add(Mathf.FloorToInt(cell - dungeonSize.x));

        // Check Down
        if (cell + dungeonSize.x < board.Count && !board[Mathf.FloorToInt(cell + dungeonSize.x)].Visited)
            neighbors.Add(Mathf.FloorToInt(cell + dungeonSize.x));

        // Check Right
        if ((cell + 1) % dungeonSize.x != 0 && !board[Mathf.FloorToInt(cell + 1)].Visited)
            neighbors.Add(Mathf.FloorToInt(cell + 1));

        // Check Left
        if (cell % dungeonSize.x != 0 && !board[Mathf.FloorToInt(cell - 1)].Visited)
            neighbors.Add(Mathf.FloorToInt(cell - 1));

        return neighbors;
    }
}