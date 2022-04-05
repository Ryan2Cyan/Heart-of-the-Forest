using System.Collections.Generic;
using Items;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int maxItems = 3;
    public List<Item> items = new List<Item>();

    public void AddItem(Item arg)
    {
        // Check if there is space in inventory:
        if (items.Count != maxItems)
        {
            Debug.Log("Items.Count" + items.Count);
            items.Add(arg);
        }
    }
}