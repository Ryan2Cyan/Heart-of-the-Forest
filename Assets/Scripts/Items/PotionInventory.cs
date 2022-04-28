using System;
using System.Collections.Generic;
using Entities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

namespace Items
{
    public class PotionInventory : MonoBehaviour
    {
        private GameObject player;
        private Player playerScript;
        private FirstPersonController playerFpsController;
        private Sprite healthSprite;
        private Sprite speedSprite;
        private Sprite damageSprite;
        public int healthPrice { get; private set; }
        public int speedPrice { get; private set; }
        public int damagePrice { get; private set; }
        public int maxSlots;
        private float currentTimer;
        private Potion activePotion;
        public bool isPotionActive;
        private bool isPotionEffect;
        public List<Potion> potions = new List<Potion>();
        [SerializeField] private TextMeshProUGUI healthPriceGUI;
        [SerializeField] private TextMeshProUGUI speedPriceGUI;
        [SerializeField] private TextMeshProUGUI damagePriceGUI;
        [SerializeField] private TextMeshProUGUI effectTimer;
        [SerializeField] private Image effectImage;
        [SerializeField] private AudioClip drinkPotion;

        private void Start()
        {
            player = GameObject.FindWithTag("Player");
            playerScript = player.GetComponent<Player>();
            playerFpsController = player.GetComponent<FirstPersonController>();
            
            // Assign sprites:
            healthSprite = Resources.Load<Sprite>("Sprites/health-potion");
            speedSprite = Resources.Load<Sprite>("Sprites/speed-potion");
            damageSprite = Resources.Load<Sprite>("Sprites/damage-potion");
            
            // Assign prices:
            healthPrice = 150;
            speedPrice = 100;
            damagePrice = 200;
            healthPriceGUI.text = healthPrice.ToString();
            speedPriceGUI.text = speedPrice.ToString();
            damagePriceGUI.text = damagePrice.ToString();
            
            // Assign max slots:
            maxSlots = 0;
            isPotionEffect = false;

        }

        private void Update()
        {
            if (isPotionActive)
            {
                currentTimer -= Time.deltaTime;
                if (currentTimer > 0.0f)
                {
                    var displayTime = (int)currentTimer;
                    effectTimer.text = displayTime.ToString();
                    effectImage.enabled = true;
                    
                    // Enable potion when used:
                    switch (activePotion.type)
                    {
                        case PotionType.Health:
                            playerScript.currentHealth += (100 * Time.deltaTime);
                            if (isPotionEffect)
                            {
                                playerScript.currentHealth += (100 * Time.deltaTime);
                                effectImage.sprite = Resources.Load<Sprite>("Sprites/health-potion-effect");
                            }
                            break;
                        case PotionType.Speed:

                            if (isPotionEffect)
                            {
                                playerScript.GetComponent<FirstPersonController>().m_WalkSpeed += 10f;
                                playerScript.GetComponent<FirstPersonController>().m_RunSpeed += 20f;
                                effectImage.sprite = Resources.Load<Sprite>("Sprites/speed-potion-effect");
                            }
                            break;
                        case PotionType.Damage:
                            if (isPotionEffect)
                            {
                                playerScript.weapon.damage += 50;
                                effectImage.sprite = Resources.Load<Sprite>("Sprites/damage-potion-effect");
                            }

                            break;
                        case PotionType.None:
                            Debug.Log("Invalid potion type.");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    
                    isPotionEffect = false;
                }

                // Disable potion when timer runs out:
                if (currentTimer <= 0f && isPotionActive)
                {
                    switch (activePotion.type)
                    {
                        case PotionType.Health:
                            break;
                        case PotionType.Speed:
                            playerScript.GetComponent<FirstPersonController>().m_WalkSpeed -= 10f;
                            playerScript.GetComponent<FirstPersonController>().m_RunSpeed -= 20f;
                            break;
                        case PotionType.Damage:
                            playerScript.weapon.damage -= 50;
                            break;
                        case PotionType.None:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    effectImage.enabled = false;
                    isPotionEffect = false;
                    isPotionActive = false;
                }

            }
            else
                effectTimer.text = "";
            
            
        }

        public void UsePotion(Potion arg)
        {
            playerScript.src.PlayOneShot(drinkPotion);
            currentTimer = arg.duration;
            activePotion = arg;
            isPotionActive = true;
            isPotionEffect = true;
            potions.Remove(arg);
        }


        // Changes the GUI to display what potions the player contains in their inventory:
        public void SetPotionIcons()
        {
            GameObject.Find("Slot-" + 0).GetComponent<Image>().enabled = false;
            GameObject.Find("Slot-" + 1).GetComponent<Image>().enabled = false;
            GameObject.Find("Slot-" + 2).GetComponent<Image>().enabled = false;
            GameObject.Find("Slot-" + 3).GetComponent<Image>().enabled = false;
            
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
                            GameObject.Find("Slot-" + i).GetComponent<Image>().enabled = true;
                            GameObject.Find("Slot-" + i).GetComponent<Image>().sprite = speedSprite;
                            break;
                        case PotionType.Damage:
                            GameObject.Find("Slot-" + i).GetComponent<Image>().enabled = true;
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