using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item")]
public class Item : ScriptableObject
{
    // type of the item
    public ItemType type;
    // name of the item
    public string displayName;
    // description for the item
    public string description;
    // thumbnail image for the item
    public Texture2D thumbnail;
    // the way this item is used
    public ItemUseType useType;
    // the associated plant or structure that can be placed depending on this item's useType
    public Structure placeable;
}
