// ICell.cs
public interface ICell
{
    bool Visited { get; set; }
    bool[] Status { get; }
}