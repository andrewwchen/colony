/// <summary>
/// class which holds save game data about a particular plant in a structure in the scene
/// </summary>
[System.Serializable]
public class PlantData
{
    // the type of the plant
    public PlantType type;

    // number of days the plant has lived
    public int age;

    public PlantData(PlantType type, int age)
    {
        this.type = type;
        this.age = age;
    }

    public override string ToString()
    {
        string s = @"type={0}
age={1}";

        return string.Format(s, type, age);
    }
}