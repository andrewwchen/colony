/// <summary>
/// class which holds save game data about a particular item in the inventory
/// </summary>
[System.Serializable]
public class ItemData
{
    // the type of item
    public ItemType type;
    // the number of this item in inventory
    public int count;

    public ItemData(ItemType type, int count)
    {
        this.type = type;
        this.count = count;
    }

    public override string ToString()
    {
        string s = @"type={0}
count={1}";

        return string.Format(s, type, count);
    }
}