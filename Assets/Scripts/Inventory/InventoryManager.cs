using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    // singleton instance
    public static InventoryManager Instance;
    // list of all items available in the game
    [SerializeField] private Item[] items;
    // maps item types to items
    private Dictionary<ItemType, Item> typeToItem;
    // maps items to item types
    private Dictionary<Item, ItemType> itemToType;
    // maps items to the amount of that item a player has
    [HideInInspector] public Dictionary<Item, int> inventory;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Start is called before the first frame update
    private void Start()
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

    // converts inventory into a serializable format for data saving
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
