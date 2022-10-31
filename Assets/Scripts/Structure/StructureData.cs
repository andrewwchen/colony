/// <summary>
/// class which holds save game data about a particular structure in the scene
/// </summary>
[System.Serializable]
public class StructureData
{
    // the direction the structure is facing
    public StructureDirection direction = StructureDirection.North;
    // the type of the structure
    public StructureType type = StructureType.None;
    public int row;
    public int col;

    public StructureData(StructureDirection direction, StructureType type, int row, int col)
    {
        this.type = type;
        this.direction = direction;
        this.row = row;
        this.col = col;
    }

    public override string ToString()
    {
        string s = @"direction={0}
type={1}";

        return string.Format(s, direction, type);
    }
}