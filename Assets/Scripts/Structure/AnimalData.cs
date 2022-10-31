/// <summary>
/// class which holds save game data about a particular animal in a structure in the scene
/// </summary>
[System.Serializable]
public class AnimalData
{
    // the type of the animal
    public AnimalType type;

    // the name of the animal
    public string displayName;

    // number of days the animal has gone unfed
    public int daysUnfed;

    public AnimalData(AnimalType type, string displayName, int daysUnfed)
    {
        this.type = type;
        this.displayName = displayName;
        this.daysUnfed = daysUnfed;
    }

    public override string ToString()
    {
        string s = @"type={0}
displayName={1}
daysUnfed={2}";

        return string.Format(s, type, displayName, daysUnfed);
    }
}