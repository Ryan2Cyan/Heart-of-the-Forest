using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDG.Items
{
    public class Item : MonoBehaviour
    {
        private int price;
        private string effect;

        
    }

    public enum ItemType
    {
        Empty,
        Potion,
        Weapon,
        Armour,
        Misc,
    }
}
