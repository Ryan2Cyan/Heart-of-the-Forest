using System;
using UnityEngine;

namespace Items
{
    public class Item
    {
        public string name { get; }
        public int price { get; }
        public ItemType type { get; }
        
        public Item(string name, ItemType type, int price)
        {
            this.name = name;
            this.price = price;
            this.type = type;
        }
    }
    
    public enum ItemType
    {
        Empty,
        Potion,
        Armour,
        Misc
    }
    
}
