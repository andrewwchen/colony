using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class InventoryManager : MonoBehaviour
{
    // singleton instance
    public static InventoryManager Instance;
    // list of all items available in the game
    public Item[] items;
    // maps item types to items
    private Dictionary<ItemType, Item> typeToItem = new Dictionary<ItemType, Item>();
    // maps items to item types
    private Dictionary<Item, ItemType> itemToType = new Dictionary<Item, ItemType>();
    // list of buyable items
    [HideInInspector] public List<Item> buyables = new List<Item>();
    // maps items to the amount of that item a player has
    [HideInInspector] public Dictionary<Item, int> inventory = new Dictionary<Item, int>();
    // the player's current amount of money
    [HideInInspector] public int money;
    [HideInInspector] public UnityEvent OnMoneyChange;
    [HideInInspector] public UnityEvent OnInventoryChange;

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
            typeToItem.Add(type, item);
            inventory[item] = 0;
            itemToType[item] = type;
            if (item.buyable)
                buyables.Add(item);
        }

        // initializing inventory based on saved data
        ItemData[] itemData = DataManager.Instance.gameData.inventory;
        for (int i = 0; i < itemData.Length; i++)
        {
            ItemData data = itemData[i];
            inventory[typeToItem[data.type]] = data.count;
        }

        // initializing money based on saved data
        money = DataManager.Instance.gameData.money;
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

    public void AddItem(Item item)
    {
        inventory[item] += 1;
        OnInventoryChange?.Invoke();
    }

    public bool RemoveItem(Item item)
    {
        if (HasItem(item))
        {
            inventory[item] -= 1;
            OnInventoryChange?.Invoke();
            return true;
        }
        return false;
    }

    public bool HasItem(Item item)
    {
        return inventory[item] > 0;
    }

    public bool UseItem(Item item)
    {
        if (!HasItem(item))
            return false;

        switch (item.useType)
        {
            case ItemUseType.Seed:
                // Set UM mode to Seeding with this item
                return true;
            case ItemUseType.Structure:
                // Set UM mode to Building with this item
                return true;
            default:
                return false;
        }
    }

    public bool SellItem(Item item)
    {
        if (RemoveItem(item))
        {
            money += item.sellPrice;
            OnMoneyChange?.Invoke();
            return true;
        }
        return false;
    }

    public bool BuyItem(Item item)
    {
        if (item.buyable && money >= item.buyPrice)
        {
            money -= item.buyPrice;
            AddItem(item);
            OnMoneyChange?.Invoke();
            return true;
        }
        return false;
    }

    public string GetBalance()
    {
        return Utils.MoneyToString(money);
    }
}
