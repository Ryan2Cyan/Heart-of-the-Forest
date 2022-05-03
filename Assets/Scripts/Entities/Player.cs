using System.Collections;
using Core;
using Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

namespace Entities
{
    public class Player : Entity
    {
        // Other:
        private FirstPersonController firstPersonController;

        // GUI:
        private Slider hpBarSlider;
        private Image hpBarFill;
        private TextMeshProUGUI hpText;
        private TextMeshProUGUI currentGoldText;
        private Image damageFX;
        [SerializeField] private SettingsMenu settingsMenu;
        private CanvasGroup playerDeathScreen;
        private bool FinishedFading;

        // State variables:
        public float attackDelay { get; set; }
        public int currentGold { get; set; }
        public int globalGold { get; set; }
        public bool shopMenuState { get; set; }
        private float attackTimer;
        private bool useAttack0;
        private bool settingsMenuState;
        private Animator weaponAnimator;
        public int speedLvl;
        public int resistanceLvl;
        public int goldAccumulationLvl;
        private PotionInventory potionsInventory;

        // Audio:
        public AudioSource src { get; private set; }
        private AudioClip takeDamageSound;
        private AudioClip deathSound;
        private bool locked;

        // Animator indexes:
        private static readonly int AttackWithSword = Animator.StringToHash("AttackWithSword");
        private static readonly int AttackWithSword0 = Animator.StringToHash("AttackWithSword0");
        
        private void Start()
        {
            // Fetch components:
            weapon = transform.GetChild(0).transform.GetChild(0).GetComponent<Weapon>();
            weaponAnimator = weapon.gameObject.GetComponent<Animator>();
            hpBarSlider = GameObject.Find("HealthBar").GetComponent<Slider>();
            hpBarFill = hpBarSlider.transform.GetChild(0).GetComponent<Image>();
            hpText = GameObject.Find("HPText").GetComponent<TextMeshProUGUI>();
            currentGoldText = GameObject.Find("Player-Gold").GetComponentInChildren<TextMeshProUGUI>();
            damageFX = GameObject.Find("GetHitIndicator").GetComponent<Image>();
            potionsInventory = GetComponent<PotionInventory>();
            playerDeathScreen = GameObject.Find("PlayerDeathScreen").GetComponent<CanvasGroup>();
            firstPersonController = GetComponent<FirstPersonController>();
            src = GetComponent<AudioSource>();
            takeDamageSound = Resources.Load<AudioClip>("Sounds/SFX/AlexSFX/player-getting-hit");
            deathSound = Resources.Load<AudioClip>("Sounds/SFX/AlexSFX/player-die");

            // Assign values:
            maxHealth = 50;
            currentHealth = maxHealth;
            hpBarSlider.maxValue = maxHealth;
            hpBarSlider.value = currentHealth;
            currentGold = 0;
            globalGold = 0;
            weapon.attackSpeed = 0.2f;
            attackTimer = attackDelay;
            attackDelay = 0.8f;
            settingsMenuState = false;
            FinishedFading = false;
            damageFX.gameObject.SetActive(false);
        }
        
        private void Update()
        {
            if (currentHealth <= 0)
                Death();
            else
            {
                FallCheck();
                ProcessInput();
                AssignGold(ref currentGoldText, currentGold);
                UpdateHPBar(ref hpBarSlider, ref hpText, ref currentHealth, maxHealth);
                FadeOutDamageVFX(ref damageFX);
            }
        }
        
        private void Death()
        {
            // Display how many waves the player survived:
            playerDeathScreen.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                "Waves Survived:    " + GameObject.Find("GameState").GetComponent<GameState>().currentWave;
            playerDeathScreen.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                "Total Gold:    " + globalGold;
            
            if (!FinishedFading)
            {
                if (playerDeathScreen.alpha <= 1)
                {
                    firstPersonController.enabled = false;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    playerDeathScreen.alpha += Time.deltaTime;
                    damageFX.gameObject.SetActive(false);
                }

                if (playerDeathScreen.alpha >= 1)
                {
                    Time.timeScale = 0;
                    src.PlayOneShot(deathSound);
                    playerDeathScreen.interactable = true;
                    playerDeathScreen.blocksRaycasts = true;
                    FinishedFading = true;
                }
            }
        }

        // Change color of HP bar when hit:
        private IEnumerator ChangeHpBarColor()
        {
            var originalColor = hpBarFill.color;
            hpBarFill.color = Color.white;
            yield return new WaitForSeconds(0.05f);
            hpBarFill.color = originalColor;
        }

        // Processes activated when attacking (including collider and animation):
        private IEnumerator AttackCooldown()
        {
            useAttack0 = !useAttack0;
            // Animator speed is inverse of whatever weapon's attack speed is
            weaponAnimator.speed = Mathf.Abs(weapon.attackSpeed - 1);
            weaponAnimator.SetBool(!useAttack0 ? AttackWithSword0 : AttackWithSword, true);
            yield return new WaitForSeconds(weapon.attackSpeed);
            weaponAnimator.SetBool(AttackWithSword0, false);
            weaponAnimator.SetBool(!useAttack0 ? AttackWithSword0 : AttackWithSword, false);
        }

        // Check if the player has fallen off the world:
        private void FallCheck()
        {
            if (transform.position.y <= -100.0f) OnDeath();
        }

        // Process all player input relating to the player:
        private void ProcessInput()
        {
            // Toggle settings menu:
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (settingsMenuState == false && shopMenuState == false)
                {
                    // Unlock cursor:
                    firstPersonController.enabled = false;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;

                    // Open settings menu and pause game
                    settingsMenu.SwitchSetting(0);
                    settingsMenuState = true;
                    Time.timeScale = 0;
                }
                else
                {
                    // Close all settings menu and unpause game
                    settingsMenu.SwitchSetting(2);
                    settingsMenuState = false;
                    Time.timeScale = 1;

                    // Give cursor control back to FPS controller:
                    firstPersonController.enabled = true;
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }

            }

            // Using potions:
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                UsePotion(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                UsePotion(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                UsePotion(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                UsePotion(3);
            }

            // Attack:
            attackTimer -= Time.deltaTime;
            if (!Cursor.visible) // Don't process mouse input while in UI Menu
            {
                if (Input.GetMouseButtonDown(0) && attackTimer <= 0.0f)
                {
                    StartCoroutine(AttackCooldown());
                    attackTimer = attackDelay;
                }
            }


        }

        // Activate potion in specified potion slot:
        private void UsePotion(int potionSlot)
        {
            if (GameObject.Find("Slot-" + potionSlot).GetComponent<Image>().isActiveAndEnabled &&
                !potionsInventory.isPotionActive)
            {
                potionsInventory.UsePotion(potionsInventory.potions[potionSlot]);
                potionsInventory.SetPotionIcons();
            }
        }
        
        // Reduce alpha of damage VFX and deactivate once non-visible:
        private void FadeOutDamageVFX(ref Image damageVfxArg)
        {
            // Once active, reduce the alpha:
            if(damageVfxArg.color.a > 0f && damageVfxArg.gameObject.activeInHierarchy)
                damageFX.color = Color.Lerp(damageFX.color, new Color(1, 0, 0, 0), 2 * Time.deltaTime);
            
            // Once alpha is 0.001 or less, deactivate:
            if(damageVfxArg.color.a <= 0.001f)
                damageFX.gameObject.SetActive(false);
        }
        
        // Displays (to UI) the players gold:
        private static void AssignGold(ref TextMeshProUGUI goldText, int gold)
        {
            goldText.text = gold.ToString();
        }
        
        // Updates the player's HP bar with their current HP:
        private static void UpdateHPBar(ref Slider hpBarArg, ref TextMeshProUGUI hpTextArg, ref float currentHP, 
            float maxHP)
        {
            hpBarArg.value = currentHP;
            hpTextArg.text = Mathf.Floor(currentHP) + " / " + maxHP;
            if (currentHP > maxHP)
            {
                currentHP = maxHP;
            }
        }

        // Reduces current HP:
        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
            if (hpBarFill.color != Color.white)
                StartCoroutine(ChangeHpBarColor());
            hpBarSlider.value = currentHealth;
            src.PlayOneShot(takeDamageSound);
            damageFX.gameObject.SetActive(true);
            damageFX.color = new Color(1, 0, 0, 0.4f);
        }

    }
}
