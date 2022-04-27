using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Items
{
    public class PotionInventory : MonoBehaviour
    {
        private Sprite healthSprite;
        private Sprite speedSprite;
        private Sprite damageSprite;
        public int maxSlots = 3;
        public readonly List<Potion> potions = new List<Potion>();

        private void Start()
        {
            healthSprite = Resources.Load<Sprite>("Sprites/health-potion");
            speedSprite = Resources.Load<Sprite>("Sprites/speed-potion");
            damageSprite = Resources.Load<Sprite>("Sprites/damage-potion");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                Debug.Log("Potion0" + potions[0].type);
                Debug.Log("Potion1" + potions[1].type);
                Debug.Log("Potion2" + potions[2].type);
                Debug.Log("Potion3" + potions[3].type);
            }
        }

        // Add health potion to player inventory:
        public void AddHealthPotion()
        {
            Debug.Log(potions.Count);
            // Check if there is space in inventory:
            if (potions.Count < 3)
            {
                Debug.Log("Add HP Potion");
                var newPotion = new Potion(PotionType.Health);
                potions.Add(newPotion);
                SetPotionIcons();
            }
            else
                Debug.Log("Slots Full");
        }
        
        // Add speed potion to player inventory:
        public void AddSpeedPotion()
        {
            Debug.Log(potions.Count);
            // Check if there is space in inventory:
            if (potions.Count < 3)
            {
                Debug.Log("Add HP Potion");
                var newPotion = new Potion(PotionType.Speed);
                potions.Add(newPotion);
                SetPotionIcons();
            }
            else
                Debug.Log("Slots Full");
        }
        
        // Add damage potion to player inventory:
        public void AddDamagePotion()
        {
            Debug.Log(potions.Count);
            // Check if there is space in inventory:
            if (potions.Count < 3)
            {
                Debug.Log("Add HP Potion");
                var newPotion = new Potion(PotionType.Damage);
                potions.Add(newPotion);
                SetPotionIcons();
            }
            else
                Debug.Log("Slots Full");
        }
        
        
        // Changes the GUI to display what potions the player contains in their inventory:
        private void SetPotionIcons()
        {
            for (var i = 0; i < potions.Count; i++)
            {
                if (potions[i] != null)
                {
                    switch (potions[i].type)
                    {
                        case PotionType.None:
                            Debug.Log("Invalid potion type");
                            break;
                        case PotionType.Health:
                            GameObject.Find("Slot-" + i).GetComponent<Image>().enabled = true;
                            GameObject.Find("Slot-" + i).GetComponent<Image>().sprite = healthSprite;
                            break;
                        case PotionType.Speed:
                            GameObject.Find("Slot-" + i).GetComponent<Image>().sprite = speedSprite;
                            break;
                        case PotionType.Damage:
                            GameObject.Find("Slot-" + i).GetComponent<Image>().sprite = damageSprite;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            
        }
    }
}