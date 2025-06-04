// ICellFactory.cs
public interface ICellFactory
{
    ICell CreateCell();
}

// BasicCellFactory.cs
public class BasicCellFactory : ICellFactory
{
    public ICell CreateCell()
    {
        return new Cell();
    }
}