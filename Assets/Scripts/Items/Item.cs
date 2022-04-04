using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDG.Items
{
    public class Item : MonoBehaviour
    {
        private int price;
        private string effect;
        private string itemType;

        public virtual bool EquipItem(Item chosenItem)
        {
            return chosenItem;
        }

        public virtual void DropItem()
        {

        }

        public virtual string GetItemType()
        {
            return itemType;
        }
    }
}
