using System;
using System.Collections;
using Core;
using Items;
using TDG.Entity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

namespace Entities
{
    public class Shopkeep : Entity
    {
        private bool playerCollision;
        private bool menuOpen;
        private GameObject lvl1Model;
        private GameObject lvl2Model;
        private GameObject lvl3Model;
        private ParticleSystem smokeModel;
        private GameObject player;
        private Player playerScript;
        private PlayerWeapon playerWeaponScript;
        private PotionInventory potionInventory;
        private GameState gameState;
        private double corePassiveRegenDelay;
        private double corePassiveRegenTimer;
        public bool isDead;
    
        [SerializeField] private Slider hpBarSlider;
        [SerializeField] private Slider nightHpBarSlider;
        [SerializeField] private TextMeshProUGUI upgradePrice;
        [SerializeField] private AudioClip repairSfx;
        [SerializeField] private AudioClip destroySfx;
        [SerializeField] private AudioClip nullSfx;
        [SerializeField] private AudioClip purchaseSfx;
        [SerializeField] private AudioClip UpgradeBuildingSfx;
        [SerializeField] private AudioClip buyPotionSfx;
        [SerializeField] private AudioSource src;
        [SerializeField] private ShopType shopType;
        [SerializeField] private GameObject interactButton;
        [SerializeField] private GameObject menu;
        [SerializeField] private GameObject sign;
        [SerializeField] private ParticleSystem damagePfx;
        [SerializeField] private GameObject healthPotion;
        [SerializeField] private GameObject speedPotion;
        [SerializeField] private GameObject damagePotion;
    

    
    

        private void Start()
        {   
            // Get components:
            player = GameObject.FindWithTag("Player");
            playerScript = player.GetComponent<Player>();
            if (shopType != ShopType.Core)
            {
                lvl1Model = transform.GetChild(0).gameObject;
                lvl2Model = transform.GetChild(1).gameObject;
                lvl3Model = transform.GetChild(2).gameObject;
                smokeModel = transform.GetChild(3).GetComponent<ParticleSystem>();
                lvl1Model.SetActive(true);
            }
            gameState = GameObject.Find("GameState").GetComponent<GameState>();
            playerWeaponScript = player.transform.GetChild(0).GetChild(0).GetComponent<PlayerWeapon>();
            potionInventory = player.GetComponent<PotionInventory>();

            // Set values:
            menuOpen = false;
            playerCollision = false;
            maxHealth = 100;
            corePassiveRegenDelay = 4f;
            corePassiveRegenTimer = corePassiveRegenDelay;
            currentHealth = maxHealth;
            if (hpBarSlider)
            {
                hpBarSlider.maxValue = maxHealth;
                hpBarSlider.value = maxHealth;
                hpBarSlider.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = currentHealth.ToString();
            }
        }

        private void Update()
        {
            // Core Passive Health Regen:
            if(shopType == ShopType.Core)
            {
                if(currentHealth < maxHealth)
                {
                    if (!gameState.isDay)
                    {
                        corePassiveRegenTimer -= 1 * Time.deltaTime;
                        if(corePassiveRegenTimer <= 0)
                        {
                            currentHealth += 1;
                            corePassiveRegenTimer = corePassiveRegenDelay;
                            UpdateHPBars();
                        } 
                    }
                }
            }

            // Display potions depending on shop level:
            if (shopType == ShopType.Alchemist)
            {
                if (lvl1Model.activeInHierarchy)
                {
                    healthPotion.SetActive(true);
                    speedPotion.SetActive(false);
                    damagePotion.SetActive(false);
                }
                else if (lvl2Model.activeInHierarchy)
                {
                    healthPotion.SetActive(true);
                    speedPotion.SetActive(true);
                    damagePotion.SetActive(false);
                
                }
                else if (lvl3Model.activeInHierarchy)
                {
                    healthPotion.SetActive(true);
                    speedPotion.SetActive(true);
                    damagePotion.SetActive(true);
                    maxHealth = 1000;
                }
                else
                {
                    healthPotion.SetActive(false);
                    speedPotion.SetActive(false);
                    damagePotion.SetActive(false);
                }
            }
        
            // It's daytime - menu interaction is active:
            // Check if the player is in range of the shop. Toggle menu by pressing "E":
            if (Input.GetKeyDown(KeyCode.E) && playerCollision && !menuOpen) // Open menu
            {
                // Unlock cursor:
                player.GetComponent<FirstPersonController>().enabled = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                menu.SetActive(true);
                interactButton.SetActive(false);
                menuOpen = true;
                player.GetComponent<Player>().shopMenuState = true;

            }
            else if (Input.GetKeyDown(KeyCode.E) && menuOpen ||
                     Input.GetKeyDown(KeyCode.Escape) && menuOpen) // Close menu
            {
                player.GetComponent<FirstPersonController>().enabled = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                menu.SetActive(false);
                menuOpen = false;
                interactButton.SetActive(true);
                player.GetComponent<Player>().shopMenuState = false;
            }

            // Display night HP bars only at night:
            if(nightHpBarSlider != null)
                nightHpBarSlider.gameObject.SetActive(!gameState.isDay);

            if (currentHealth <= 0 && !isDead)
            {
                OnDeath();
            }
        }

        // Updates buildings HP bars:
        public void UpdateHPBars()
        {
            if (currentHealth >= 0)
            {
                // Core sliders:
                hpBarSlider.maxValue = maxHealth;
                hpBarSlider.value = currentHealth;
                hpBarSlider.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = currentHealth.ToString();
                
                // Night sliders:
                nightHpBarSlider.maxValue = maxHealth;
                nightHpBarSlider.value = currentHealth;
            }
        }
    
        // Switch to next level of shop model:
        public void UpgradeShop()
        {
            // Costs to upgrade the shops:
            const int cost0 = 250;
            const int cost1 = 500;
        
            if (!lvl3Model.activeInHierarchy)
            {
                if (lvl1Model.activeInHierarchy && playerScript.currentGold >= cost0)
                {
                    upgradePrice.text = cost1.ToString();
                    playerScript.currentGold -= cost0;
                    src.PlayOneShot(UpgradeBuildingSfx);
                    lvl1Model.SetActive(false);
                    lvl2Model.SetActive(true);
                
                    // Increase building HP:
                    maxHealth = 250;
                    currentHealth = maxHealth;
                    UpdateHPBars();
                
                    // Change Icon:
                    SetIcon(1);
                }
                else if (lvl2Model.activeInHierarchy&& playerScript.currentGold >= cost1)
                {
                    upgradePrice.text = "Max";
                    playerScript.currentGold -= cost1;
                    src.PlayOneShot(UpgradeBuildingSfx);
                    lvl2Model.SetActive(false);
                    lvl3Model.SetActive(true);
                
                    // Increase building HP:
                    maxHealth = 500;
                    currentHealth = maxHealth;
                    UpdateHPBars();
                
                    // Change Icon:
                    SetIcon(2);
                }
                else
                    src.PlayOneShot(nullSfx);
            }
            else
                src.PlayOneShot(nullSfx);
        }
    
        // Set a shop's icon depending on its level:
        private void SetIcon(int level)
        {
            switch (shopType)
            {
                case ShopType.Blacksmith:
                    if (level == 0)
                    {
                        GameObject.Find("Blacksmith-Icon").GetComponent<Image>().sprite =
                            Resources.Load<Sprite>("Sprites/blacksmith-icon-tier0");
                        nightHpBarSlider.transform.GetChild(2).GetComponent<Image>().sprite =
                            Resources.Load<Sprite>("Sprites/blacksmith-icon-tier0");
                        sign.GetComponent<Image>().sprite =
                            Resources.Load<Sprite>("Sprites/blacksmith-icon-tier0");
                    }
                
                    if (level == 1)
                    {
                        GameObject.Find("Blacksmith-Icon").GetComponent<Image>().sprite =
                            Resources.Load<Sprite>("Sprites/blacksmith-icon-tier1");
                        nightHpBarSlider.transform.GetChild(2).GetComponent<Image>().sprite =
                            Resources.Load<Sprite>("Sprites/blacksmith-icon-tier1");
                        sign.GetComponent<Image>().sprite =
                            Resources.Load<Sprite>("Sprites/blacksmith-icon-tier1");
                    }

                    if (level == 2)
                    {
                        GameObject.Find("Blacksmith-Icon").GetComponent<Image>().sprite =
                            Resources.Load<Sprite>("Sprites/blacksmith-icon-tier2");
                        nightHpBarSlider.transform.GetChild(2).GetComponent<Image>().sprite =
                            Resources.Load<Sprite>("Sprites/blacksmith-icon-tier2");
                        sign.GetComponent<Image>().sprite =
                            Resources.Load<Sprite>("Sprites/blacksmith-icon-tier2");
                    }

                    break;
                case ShopType.Armorsmith:
                    if (level == 0)
                    {
                        GameObject.Find("Armorsmith-Icon").GetComponent<Image>().sprite =
                            Resources.Load<Sprite>("Sprites/armorsmith-icon-tier0");
                        nightHpBarSlider.transform.GetChild(2).GetComponent<Image>().sprite =
                            Resources.Load<Sprite>("Sprites/armorsmith-icon-tier0");
                        sign.GetComponent<Image>().sprite =
                            Resources.Load<Sprite>("Sprites/armorsmith-icon-tier0");
                    }
                
                    if (level == 1)
                    {
                        GameObject.Find("Armorsmith-Icon").GetComponent<Image>().sprite =
                            Resources.Load<Sprite>("Sprites/armorsmith-icon-tier1");
                        nightHpBarSlider.transform.GetChild(2).GetComponent<Image>().sprite =
                            Resources.Load<Sprite>("Sprites/armorsmith-icon-tier1");
                        sign.GetComponent<Image>().sprite =
                            Resources.Load<Sprite>("Sprites/armorsmith-icon-tier1");
                    }

                    if (level == 2)
                    {
                        GameObject.Find("Armorsmith-Icon").GetComponent<Image>().sprite =
                            Resources.Load<Sprite>("Sprites/armorsmith-icon-tier2");
                        nightHpBarSlider.transform.GetChild(2).GetComponent<Image>().sprite =
                            Resources.Load<Sprite>("Sprites/armorsmith-icon-tier2");
                        sign.GetComponent<Image>().sprite =
                            Resources.Load<Sprite>("Sprites/armorsmith-icon-tier2");
                    }

                    break;
                case ShopType.Alchemist:
                    if (level == 0)
                    {
                        GameObject.Find("Alchemist-Icon").GetComponent<Image>().sprite =
                            Resources.Load<Sprite>("Sprites/alchemist-icon-tier0");
                        nightHpBarSlider.transform.GetChild(2).GetComponent<Image>().sprite =
                            Resources.Load<Sprite>("Sprites/alchemist-icon-tier0");
                        sign.GetComponent<Image>().sprite =
                            Resources.Load<Sprite>("Sprites/alchemist-icon-tier0");
                    }
                
                    if (level == 1)
                    {
                        GameObject.Find("Alchemist-Icon").GetComponent<Image>().sprite =
                            Resources.Load<Sprite>("Sprites/alchemist-icon-tier1");
                        nightHpBarSlider.transform.GetChild(2).GetComponent<Image>().sprite =
                            Resources.Load<Sprite>("Sprites/alchemist-icon-tier1");
                        sign.GetComponent<Image>().sprite =
                            Resources.Load<Sprite>("Sprites/alchemist-icon-tier1");
                    }

                    if (level == 2)
                    {
                        GameObject.Find("Alchemist-Icon").GetComponent<Image>().sprite =
                            Resources.Load<Sprite>("Sprites/alchemist-icon-tier2");
                        nightHpBarSlider.transform.GetChild(2).GetComponent<Image>().sprite =
                            Resources.Load<Sprite>("Sprites/alchemist-icon-tier2");
                        sign.GetComponent<Image>().sprite =
                            Resources.Load<Sprite>("Sprites/alchemist-icon-tier2");
                    }

                    break;
                default:
                    Debug.Log("Invalid shop type");
                    throw new ArgumentOutOfRangeException();
            }
        }

        // Repairs the shop, making it accessible for the player once again:
        public void Repair()
        {
            const int cost0 = 100;
            if (playerScript.currentGold >= cost0 && currentHealth != maxHealth)
            {
                src.PlayOneShot(repairSfx);
                switch (isDead)
                {
                    case true:
                        isDead = false;
                        lvl1Model.SetActive(true);
                        smokeModel.Stop(true);
                        playerScript.currentGold -= cost0;
                        SetIcon(0);
                
                        // Reset building HP:
                        maxHealth = 100;
                        currentHealth = maxHealth;
                        UpdateHPBars();
                        break;
                    case false:
                        // Set building HP to max:
                        playerScript.currentGold -= cost0;
                        currentHealth = maxHealth;
                        UpdateHPBars();
                        break;
                }
            }
            else
                src.PlayOneShot(nullSfx);
        }
    
        // Upgrades a weapon's range:
        public void UpgradeRange()
        {
            UpgradeIncSprites(
                100, 
                25, 
                ref playerWeaponScript.rangeLvl, 
                "Range",
                Resources.Load<Sprite>("Sprites/rangeblade-tier1"),
                Resources.Load<Sprite>("Sprites/rangeblade-tier2"));
        }
    
        // Start next wave:
        public void StartNextWave()
        {
            // Toggle night and day:
            if(gameState.isDay)
            {
                gameState.ToggleDay();
                gameState.UpdateWaveCount();
                gameState.dayTimerIcon.SetActive(false);
                player.GetComponent<FirstPersonController>().enabled = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                menu.SetActive(false);
                menuOpen = false;
                interactButton.SetActive(true);
                player.GetComponent<Player>().shopMenuState = false;
            
                // Disable and close core menu:
                interactButton.SetActive(false);
                playerCollision = false;
                menu.SetActive(false);
                menuOpen = false;
            } 
        }
    
        // Upgrades a weapon's attack speed:
        public void UpgradeAttackSpeed()
        {
            UpgradeIncSprites(
                100, 
                50, 
                ref playerWeaponScript.attackSpeedLvl, 
                "AttackSpeed",
                Resources.Load<Sprite>("Sprites/hilt-tier1"),
                Resources.Load<Sprite>("Sprites/hilt-tier2")
            );
        }
    
        // Upgrades a weapon's damage:
        public void UpgradeDamage()
        {
            UpgradeIncSprites(
                100, 
                50, 
                ref playerWeaponScript.damageLvl, 
                "Damage",
                Resources.Load<Sprite>("Sprites/blade-tier1"),
                Resources.Load<Sprite>("Sprites/blade-tier2")
            );
        }
    
        // Upgrades user's jump height:
        public void UpgradePotionSlot()
        {
            UpgradeIncSprites(
                125,
                125,
                ref potionInventory.maxSlots,
                "PotionSlot",
                Resources.Load<Sprite>("Sprites/leggings-tier1"),
                Resources.Load<Sprite>("Sprites/leggings-tier2")
            );
        }
    
        // Upgrades how much gold the user gets from enemies:
        public void UpgradeGoldAccumulation()
        {
            UpgradeIncSprites(
                100,
                50,
                ref playerScript.goldAccumulationLvl,
                "GoldAccumulation",
                Resources.Load<Sprite>("Sprites/helmet-tier1"),
                Resources.Load<Sprite>("Sprites/helmet-tier2")
            );
        }
    
        // Upgrades how much damage a player can resist per hit:
        public void UpgradeResistance()
        {
            UpgradeIncSprites(
                100,
                50,
                ref playerScript.resistanceLvl,
                "Resistance",
                Resources.Load<Sprite>("Sprites/chestpiece-tier1"),
                Resources.Load<Sprite>("Sprites/chestpiece-tier2")
            );
        }
    
        // Upgrades user's running and walking speed:
        public void UpgradeSpeed()
        {
            UpgradeIncSprites(
                100,
                50,
                ref playerScript.speedLvl,
                "Speed",
                Resources.Load<Sprite>("Sprites/boots-tier1"),
                Resources.Load<Sprite>("Sprites/boots-tier2")
            );
        }

        // Add health potion to player inventory:
        public void AddHealthPotion()
        {
            // Check if there is space in inventory:
            if (potionInventory.potions.Count < potionInventory.maxSlots && 
                playerScript.currentGold >= potionInventory.healthPrice)
            {
                src.PlayOneShot(buyPotionSfx);
                var newPotion = new Potion(PotionType.Health);
                potionInventory.potions.Add(newPotion);
                potionInventory.SetPotionIcons();
                playerScript.currentGold -= potionInventory.healthPrice;
            }
            else
                src.PlayOneShot(nullSfx);
        }
        
        // Add speed potion to player inventory:
        public void AddSpeedPotion()
        {
            // Check if there is space in inventory:
            if (potionInventory.potions.Count < potionInventory.maxSlots && 
                playerScript.currentGold >= potionInventory.speedPrice)
            {
                src.PlayOneShot(buyPotionSfx);
                var newPotion = new Potion(PotionType.Speed);
                potionInventory.potions.Add(newPotion);
                potionInventory.SetPotionIcons();
                playerScript.currentGold -= potionInventory.speedPrice;
            }
            else
                src.PlayOneShot(nullSfx);
        }
        
        // Add damage potion to player inventory:
        public void AddDamagePotion()
        {
            // Check if there is space in inventory:
            if (potionInventory.potions.Count < potionInventory.maxSlots && 
                playerScript.currentGold >= potionInventory.damagePrice)
            {
                src.PlayOneShot(buyPotionSfx);
                var newPotion = new Potion(PotionType.Damage);
                potionInventory.potions.Add(newPotion);
                potionInventory.SetPotionIcons();
                playerScript.currentGold -= potionInventory.damagePrice;
            }
            else
                src.PlayOneShot(nullSfx);
        }

        // Template for upgrades and stores details of each upgrade:
        private void GeneralUpgrade(int startCost, int costIncrement, ref int levelToIncrement, string upgradeName)
        {
            // Calculate current cost:
            var cost0 = CalcCost(startCost, costIncrement, levelToIncrement);
            // Display cost on UI:
            var cost = GameObject.Find("Upgrade-" + upgradeName + "-Price");
        
            // Check player can afford upgrade and isn't max level already:
            if(playerScript.currentGold >= cost0)
            {
                if (lvl1Model.activeInHierarchy && levelToIncrement < 3 ||
                    lvl2Model.activeInHierarchy && levelToIncrement < 6 ||
                    lvl3Model.activeInHierarchy && levelToIncrement < 9)
                {
                    if (upgradeName != "PotionSlot")
                    {
                        src.PlayOneShot(purchaseSfx);
                        levelToIncrement++;
                        playerScript.currentGold -= cost0;
                    }

                    playerWeaponScript.CalcTotalLevel();

                    switch (upgradeName) // Apply upgrade
                    {
                        case "Damage": // Damage upgrade
                            playerScript.weapon.damage += 5;
                            break;
                        case "AttackSpeed": // Attack speed upgrade
                            playerScript.attackDelay -= 0.07f;
                            playerScript.GetComponent<Entity>().weapon.attackSpeed -= 0.07f;
                            break;
                        case "Range": // Range upgrade
                            // Elongate sword collider:
                            var playerSword = player.transform.GetChild(0).GetChild(0);
                            playerSword.GetComponent<BoxCollider>().size += new Vector3(1.2f, 1.2f, 1.2f);
                            playerSword.GetComponent<BoxCollider>().center += new Vector3(0f, 0f, 0.8f);

                            // Elongate sword model:
                            playerSword.GetChild(0).GetChild(1).localScale += new Vector3(0f, 0.5f, 0f);
                            if (playerWeaponScript.rangeLvl > 4)
                                playerSword.GetChild(0).GetChild(1).localPosition +=
                                    new Vector3(0.0f, 0.028f, 0.0f);
                            break;
                        case "PotionSlot":
                            // Upgrade potion slots (only 3 times):
                            if (lvl1Model.activeInHierarchy && levelToIncrement == 0 ||
                                lvl2Model.activeInHierarchy && levelToIncrement == 0 ||
                                lvl3Model.activeInHierarchy && levelToIncrement == 0)
                            {
                                levelToIncrement++;
                                playerScript.currentGold -= cost0;
                                src.PlayOneShot(purchaseSfx);
                            }
                            else if (lvl2Model.activeInHierarchy && levelToIncrement == 1 ||
                                     lvl3Model.activeInHierarchy && levelToIncrement == 1)
                            {
                                levelToIncrement++;
                                playerScript.currentGold -= cost0;
                                src.PlayOneShot(purchaseSfx);
                            }
                            else if (lvl3Model.activeInHierarchy && levelToIncrement == 2)
                            {
                                levelToIncrement++;
                                playerScript.currentGold -= cost0;
                                src.PlayOneShot(purchaseSfx);
                            }
                            else
                                src.PlayOneShot(nullSfx);
                            break;
                        case "Speed":
                            player.GetComponent<FirstPersonController>().m_RunSpeed += 0.4f;
                            player.GetComponent<FirstPersonController>().m_WalkSpeed += 0.4f;
                            break;
                    }

                    // Recalculate and display new cost:
                    if (upgradeName != "PotionSlot")
                    {
                        if (levelToIncrement < 9)
                        {
                            cost0 = CalcCost(startCost, costIncrement, levelToIncrement);
                            cost.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + cost0;
                        }
                        else
                            cost.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "MAX";
                    }
                    else
                    {
                        if (levelToIncrement < 3)
                        {
                            cost0 = CalcCost(startCost, costIncrement, levelToIncrement);
                            cost.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + cost0;
                        }
                        else
                            cost.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "MAX";
                    }
                }
            }
            else
                src.PlayOneShot(nullSfx);
        }

        // Template for upgrades of armor:
        private void UpgradeIncSprites(int startCost, int costIncrement, ref int levelToIncrement, string upgradeName,
            Sprite upgrade1, Sprite upgrade2)
        {
            GeneralUpgrade(startCost, costIncrement, ref levelToIncrement, upgradeName);
        
            // Increment level indicator:
            GameObject.Find(upgradeName + "-Level-Indicator").transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                "+" + levelToIncrement;

            // Change sprite depending on player level:
            var button = GameObject.Find("Upgrade-" + upgradeName).GetComponent<Button>();
            if (upgradeName != "PotionSlot")
            {
                if (levelToIncrement > 3)
                {
                    button.GetComponent<Image>().sprite = upgrade1;
                }

                if (levelToIncrement > 6)
                {
                    button.GetComponent<Image>().sprite = upgrade2;
                }
            }
            else
            {
                if (levelToIncrement > 1)
                {
                    button.GetComponent<Image>().sprite = upgrade1;
                }

                if (levelToIncrement > 2)
                {
                    button.GetComponent<Image>().sprite = upgrade2;
                }
            }
        }
    
        // Quick method to calculate the rise in cost of an upgrade:
        private int CalcCost(int startVal, int increment, int lvl)
        {
            var result = startVal;
            switch (lvl)
            {
                case 0:
                    result = startVal;
                    break;
                case 1:
                    result += increment * lvl;
                    break;
                case 2:
                    result += increment * lvl;
                    break;
                case 3:
                    result += increment * lvl;
                    break;
                case 4:
                    result += increment * lvl;
                    break;
                case 5:
                    result += increment * lvl;
                    break;
                case 6:
                    result += increment * lvl;
                    break;
                case 7:
                    result += increment * lvl;
                    break;
                case 8:
                    result += increment * lvl;
                    break;
            }
            return result;
        }
    
        // Called when shop is destroyed:
        public override void OnDeath()
        {
            currentHealth = 0;
            UpdateHPBars();
            isDead = true;
            if (gameObject.name != "Core")
            {
                smokeModel.Play(true);
                lvl3Model.SetActive(false);
                lvl2Model.SetActive(false);
                lvl1Model.SetActive(false);
            }
            src.PlayOneShot(destroySfx);
            interactButton.SetActive(false);
            menu.SetActive(false);
        }
    
        // Reduces current HP:
        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
            if (hpBarSlider)
            {
                hpBarSlider.value = currentHealth;
            }
            if (damagePfx != null)
            {
                damagePfx.Play();
            }
            if (nightHpBarSlider)
            {
                if(nightHpBarSlider.transform.GetChild(0).GetComponent<Image>().color != Color.red)
                    StartCoroutine(ChangeHpBarColor());
            }
        }
        
        // Change color of HP bar when hit:
        private IEnumerator ChangeHpBarColor()
        {
            var fill = nightHpBarSlider.transform.GetChild(0).GetComponent<Image>();
            var originalColor = fill.color;
            fill.color = Color.red;
            yield return new WaitForSeconds(0.05f);
            fill.color = originalColor;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isDead)
            {
                // Display interaction text when close to the shop:
                if (other.gameObject.CompareTag("Player"))
                {
                    if (gameState.isDay)
                    {
                        interactButton.SetActive(true);
                        playerCollision = true;
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                interactButton.SetActive(false);
                playerCollision = false;
                menu.SetActive(false);
                menuOpen = false;
            }
        }

        private enum ShopType
        {
            Blacksmith,
            Armorsmith,
            Alchemist,
            Core,
            None
        }
    }
}
