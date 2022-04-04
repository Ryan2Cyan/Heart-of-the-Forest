using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TDG.Items;

public class Inventory : MonoBehaviour
{
    public ItemType itemType;
    public int maxStack = 3;
    public int currentStack = 0;
    public bool isFull

    {
        get
        {
            return currentStack >= maxStack;
        }
    }
    public Inventory[] slots;

    public void AddItem(ItemType thisType)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].itemType == ItemType.Empty)
            {
                slots[i].itemType = thisType;
                slots[i].currentStack = 1;
                return;
            }
            if (slots[i].itemType == thisType && !slots[i].isFull)
            {
                slots[i].currentStack++;
                return;
            }
        }
    }

    public ItemType GetItemType(int slot)
    {
        return slots[slot].itemType;
    }
}