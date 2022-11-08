using UnityEngine;

/// <summary>
/// class which holds save game data
/// </summary>
[System.Serializable]
public class GameData
{
    // amount of money the player has
    public int money;
    // the current day number the player is on (first day is day 1)
    public int day;
    // array of tiles and what structures are placed on them
    [SerializeField] public StructureData[] structures;
    // array of items held in inventory and their amounts
    [SerializeField] public ItemData[] inventory;

    public GameData()
    {
        day = 1;
        money = 400;
        structures = new StructureData[0];
        inventory = new ItemData[0];
    }

    public override string ToString()
    {
        string s = @"money={0}
day={1}
structures=[{2}]
inventory=[{3}]";

        return string.Format(s, money, day, structures, string.Join<StructureData>(',', structures), string.Join<ItemData>(',', inventory));
    }
}