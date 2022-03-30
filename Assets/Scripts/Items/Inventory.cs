using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TDG.Items;

public class Inventory : MonoBehaviour
{
    private Item[] items;
    private string itemType;

    public Item GetItem(Item item)
    {
        return item;
    }

    public Item PutItem(Item item)
    {
        return item;
    }
}
