/// <summary>
/// class which holds save game data about a particular animal in a structure in the scene
/// </summary>
[System.Serializable]
public class AnimalData
{
    // the type of the animal
    public AnimalType type;

    // the type of the animal
    public AnimalType type;
    // number of days the animal has gone unfed
    public int daysUnfed;

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