using System;
using UnityEngine;

namespace Items
{
    public class Item : MonoBehaviour
    {
        private string name;
        private int price;
        private ItemType type;
        
        public Item(string name, ItemType type, int price)
        {
            this.name = name;
            this.price = price;
            this.type = type;
        }
        
        public string GetName()
        {
            return name;
        }
    }
    
    public enum ItemType
    {
        Empty,
        Potion,
        Armour,
        Weapon,
        Misc
    }
    
}
