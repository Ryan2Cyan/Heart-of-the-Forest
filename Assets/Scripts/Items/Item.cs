using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDG.Items
{
    public class Item : MonoBehaviour
    {
        private int size;
        private int price;
        private string effect;

        public virtual bool EquipItem(Item chosenItem)
        {
            return chosenItem;
        }

        public virtual void DropItem()
        {

        }
    }
}
