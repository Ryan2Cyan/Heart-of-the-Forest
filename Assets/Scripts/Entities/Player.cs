using System.Collections;
using Items;
using TDG.Entity;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

namespace Entities
{
    public class Player : Entity
    {
        
        [SerializeField] private SettingsMenu settingsMenu;

        // GUI:
        [SerializeField] private TextMeshProUGUI hpText;
        private Slider hpBarSlider;
        private TextMeshProUGUI currentGoldUI;
        private Image damageFX;
    
        // State variables:
        public float attackDelay;
        private float attackTimer;
        private bool useAttack0;
        public int currentGold;
        private bool settingsMenuState;
        public bool shopMenuState;
        private Animator weaponAnimator;
        private BoxCollider weaponBoxCollider;
        public int speedLvl;
        public int resistanceLvl;
        public int goldAccumulationLvl;
        private PotionInventory potionsInventory;

        public AudioSource src;
        public AudioClip takeDamageSound;
        public AudioClip deathSound;
        private bool locked = false;

        private int numb = 1;

        // Indexes:
        private static readonly int AttackWithSword = Animator.StringToHash("AttackWithSword");
        private static readonly int AttackWithSword0 = Animator.StringToHash("AttackWithSword0");

        // Start is called before the first frame update
        private void Start()
        {
            // Fetch components:
            weapon = transform.GetChild(0).transform.GetChild(0).GetComponent<Weapon>();
            weaponAnimator = weapon.gameObject.GetComponent<Animator>();
            weaponBoxCollider = weapon.gameObject.GetComponent<BoxCollider>();
            hpBarSlider = GameObject.Find("Health bar").GetComponent<Slider>();
            currentGoldUI = GameObject.Find("Player-Gold").GetComponentInChildren<TextMeshProUGUI>();
            damageFX = GameObject.Find("GetHitIndicator").GetComponent<Image>();
            potionsInventory = GetComponent<PotionInventory>();

            // Assign values:
            entityName = "Jargleblarg The Great";
            maxHealth = 50;
            currentHealth = maxHealth;
            hpBarSlider.maxValue = maxHealth;
            hpBarSlider.value = currentHealth;
            currentGold = 0;
            weapon.attackSpeed = 0.2f;
            attackTimer = attackDelay;
            weaponBoxCollider.enabled = false;
            attackDelay = 0.8f;
            settingsMenuState = false;
        }

   
        private void Update()
        {
            FallCheck();
            ProcessInput();
            AssignGold();
            hpBarSlider.value = currentHealth;
            hpText.text = currentHealth + " / " + maxHealth;
            if (currentHealth <= 0)
                OnDeath();
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;
            
            damageFX.color = Color.Lerp(damageFX.color, new Color(1, 0, 0, 0), 2 * Time.deltaTime);
        }

        // Reduces current HP:
        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
            StartCoroutine(ChangeHpBarColor());
            hpBarSlider.value = currentHealth;
            // Play sound when unlocked
            if (!locked)
            {
                src.PlayOneShot(takeDamageSound);
                // Locked for time in Invoke
                locked = true;
                StartCoroutine(SetBoolBack());
            }

            damageFX.color = new Color(1, 0, 0, 0.4f);
        }
        
        // Change color of HP bar when hit:
        private IEnumerator ChangeHpBarColor()
        {
            var fill = hpBarSlider.transform.GetChild(0).GetComponent<Image>();
            var originalColor = fill.color;
            fill.color = Color.white;
            yield return new WaitForSeconds(0.05f);
            fill.color = originalColor;
        }

        // Set the bool back to play taking hit sounds
        IEnumerator SetBoolBack()
        {
            yield return new WaitForSeconds(0.5f);
            locked = false;
        }

        // Reset the scene:
        protected override void OnDeath() 
        {
            src.PlayOneShot(deathSound);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        

        // Processes activated when attacking (including collider and animation):
        private IEnumerator AttackCooldown()
        {
            useAttack0 = !useAttack0;
            // Animator speed is inverse of whatever weapon's attack speed is
            weaponAnimator.speed = Mathf.Abs(weapon.attackSpeed - 1);
            // Debug.Log("Weapon anim speed = " + weaponAnimator.speed);

            weaponAnimator.SetBool(!useAttack0 ? AttackWithSword0 : AttackWithSword, true);

            yield return new WaitForSeconds(weapon.attackSpeed);
        
            weaponAnimator.SetBool(AttackWithSword0, false);
            weaponAnimator.SetBool(!useAttack0 ? AttackWithSword0 : AttackWithSword, false);

        }

        // Check if the player has fallen off the world:
        private void FallCheck()
        {
            if(transform.position.y <= -100.0f) OnDeath();
        }

        // Process all player input relating to the player:
        private void ProcessInput()
        {
            // Toggle settings menu:
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if(settingsMenuState == false && shopMenuState == false)
                {
                    // Unlock cursor:
                    GetComponent<FirstPersonController>().enabled = false;
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
                    GetComponent<FirstPersonController>().enabled = true;
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
           
            }
            if (Input.GetKeyDown(KeyCode.G))
			{
                Debug.Log(currentHealth);
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

        // Displays (to UI) the players gold:
        private void AssignGold()
        {
            currentGoldUI.text = currentGold.ToString();
        }

        private void UsePotion(int potionSlot)
        {
            if (GameObject.Find("Slot-" + potionSlot).GetComponent<Image>().isActiveAndEnabled && 
                !potionsInventory.isPotionActive)
            {
                potionsInventory.UsePotion(potionsInventory.potions[potionSlot]);
                potionsInventory.SetPotionIcons();
            }
        }

        private enum PlayerClass
        {
            Warrior
        }

    }
}
