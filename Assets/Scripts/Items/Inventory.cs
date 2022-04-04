using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TDG.Items;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<Item> items;
    private string itemType;
    //Dictionary<string, Item> itemNamesToItem = inventoryx.Player.items.ToDictionary(i => i.itemName, i => i);
    public Item GetItem(string item)
    {
        Item test = GetComponent<Item>();
        return test;
    }

    public void InsertItem(Item item)
    {
        items.Add(item);
    }
}
