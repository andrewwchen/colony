using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public ItemType[] itemTypes;
    public Item[] items;
    public Dictionary<ItemType, Item> catalog;
    public List<Item> inventory;

    // Start is called before the first frame update
    void Start()
    {
        ItemData[] itemData = DataManager.Instance.gameData.inventory;

        catalog = Utils.Zip(itemTypes, items);

        // inventory = Utils.Decode<ItemType, Item>(itemData, catalog);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
