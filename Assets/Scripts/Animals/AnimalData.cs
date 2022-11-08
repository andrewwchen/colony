/// <summary>
/// class which holds save game data about a particular animal in a structure in the scene
/// </summary>
[System.Serializable]
public class AnimalData
{
    // the type of the animal
    public AnimalType type;

    // number of days the animal has gone unfed
    public int daysUnfed;

    public AnimalData(AnimalType type, int daysUnfed)
    {
        this.type = type;
        this.daysUnfed = daysUnfed;
    }

    public override string ToString()
    {
        string s = @"type={0}
daysUnfed={1}";

        return string.Format(s, type, daysUnfed);
    }
}