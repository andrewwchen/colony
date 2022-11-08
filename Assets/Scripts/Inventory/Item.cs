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
    public Sprite thumbnail;
    // whether this item can be bought in the shop
    public bool buyable;
    // the buy price of the item
    public int buyPrice;
    // the sell price of the item
    public int sellPrice;
    // the way this item is used
    public ItemUseType useType;
    // the associated structure that can be placed depending on this item's useType
    public Structure placeable;
    // the associated plant that can be placed depending on this item's useType
    public Plant plantable;
}
