using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    // list of all items available in the game
    public Item[] items;
    // maps item types to items
    [HideInInspector] public Dictionary<ItemType, Item> typeToItem;
    // maps items to item types
    [HideInInspector] public Dictionary<Item, ItemType> itemToType;
    // maps items to the amount of that item a player has
    [HideInInspector] public Dictionary<Item, int> inventory;

    // Start is called before the first frame update
    void Start()
    {
        // initializing dictionaries based on default values
        for (int i = 0; i < items.Length; i++)
        {
            Item item = items[i];
            ItemType type = item.type;
            typeToItem[type] = item;
            inventory[item] = 0;
            itemToType[item] = type;
        }

        // initializing inventory based on saved data
        ItemData[] itemData = DataManager.Instance.gameData.inventory;
        for (int i = 0; i < itemData.Length; i++)
        {
            ItemData data = itemData[i];
            inventory[typeToItem[data.type]] = data.count;
        }
    }

    public ItemData[] Serialize()
    {
        ItemData[] itemData = new ItemData[inventory.Count];
        for (int i = 0; i < inventory.Count; i++)
        {
            var entry = inventory.ElementAt(i);
            ItemType type = itemToType[entry.Key];
            int count = entry.Value;
            itemData[i] = new ItemData(type, count);
        }
        return itemData;
    }
}
