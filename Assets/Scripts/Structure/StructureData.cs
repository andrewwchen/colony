/// <summary>
/// class which holds save game data about a particular structure in the scene
/// </summary>
[System.Serializable]
public class StructureData
{
    // the direction the structure is facing
    public StructureDirection direction;
    // the type of the structure
    public StructureType type;
    // the row of the tile under the structure's bottom left corner 
    public int row;
    // the column of the tile under the structure's bottom left corner 
    public int col;
    // the animals living in this stable
    public AnimalData[] animals;

    public StructureData(StructureDirection direction, StructureType type, int row, int col)
    {
        this.type = type;
        this.direction = direction;
        this.row = row;
        this.col = col;
    }
    public StructureData(StructureInstance si)
    {
        type = si.config.type;
        direction = si.direction;
        row = si.row;
        col = si.col;
        animals = new AnimalData[si.animals.Count];

        int i = 0;
        foreach (AnimalInstance ai in si.animals)
        {
            animals[i] = new AnimalData(ai);
            i += 1;
        }
    }

    public override string ToString()
    {
        string s = @"direction={0}
type={1}";

        return string.Format(s, direction, type);
    }
}