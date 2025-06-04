// Cell.cs
public class Cell : ICell
{
    public bool Visited { get; set; }
    public bool[] Status { get; private set; }

    public Cell()
    {
        Visited = false;
        Status = new bool[4]; // 0-Up, 1-Down, 2-Right, 3-Left
    }
}