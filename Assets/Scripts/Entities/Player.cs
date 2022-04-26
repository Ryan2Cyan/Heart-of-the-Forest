using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TDG.Entity;
using Items;
using TMPro;
using UnityStandardAssets.Characters.FirstPerson;

public class Player : Entity
{
    // Serialized variables:
    [SerializeField] private SettingsMenu settingsMenu;
    [SerializeField] private Inventory inventory;

    // GUI:
    private Slider hpBarSlider;
    private TextMeshProUGUI currentGoldUI;
    private Image damageFX;
    
    // State variables:
    // public RaycastHit selectedObj { get; private set; }
    private Camera fpsCamera;
    private float experience;
    private float nextLevelExp;
    public float attackDelay;
    private float attackTimer;
    private bool useAttack0;
    public int currentGold;
    private bool settingsMenuState;
    public bool shopMenuState;
    private Animator weaponAnimator;
    private BoxCollider weaponBoxCollider;
    public int jumpHeightLvl;
    public int speedLvl;
    public int resistanceLvl;
    public int goldAccumulationLvl;

    public AudioSource src;
    public AudioClip takeDamageSound;
    public AudioClip deathSound;
    private bool locked = false;

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
        fpsCamera = GetComponentInChildren<Camera>();
        hpBarSlider = GameObject.Find("Health bar").GetComponent<Slider>();
        currentGoldUI = GameObject.Find("Player-Gold").GetComponentInChildren<TextMeshProUGUI>();
        damageFX = GameObject.Find("GetHitIndicator").GetComponent<Image>();

        // Assign values:
        entityName = "Jargleblarg The Great";
        maxHealth = 100;
        currentHealth = maxHealth;
        currentGold = 8000;
        nextLevelExp = 20.0f;
        experience = 0.0f;
        level = 0;
        weapon.attackSpeed = 0.6f;
        attackTimer = attackDelay;
        weaponBoxCollider.enabled = false;
        hpBarSlider.value = maxHealth;
        attackDelay = 1.9f;
        inventory = new Inventory();
        settingsMenuState = false;
        jumpHeightLvl = 0;
}

   
    private void Update()
    {
        FallCheck();
        ProcessInput();
        AssignGold();
        // HighlightInteractable();

        if (experience > nextLevelExp)
            LevelUp();
        
        if (currentHealth <= 0)
            OnDeath();

        damageFX.color = Color.Lerp(damageFX.color, new Color(1, 0, 0, 0), 2 * Time.deltaTime);
    }

// Reduces current HP:
public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
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
    

    // Increase current level and EXP to next level:
    private void LevelUp()
    {
        level += 1;
        nextLevelExp += nextLevelExp + 20.0f;
    }

    // Processes activated when attacking (including collider and animation):
    private IEnumerator AttackCooldown()
    {
        useAttack0 = !useAttack0;
        weaponAnimator.SetBool(!useAttack0 ? AttackWithSword0 : AttackWithSword, true);
        weaponBoxCollider.enabled = true;
        
        yield return new WaitForSeconds(0.05f);
        weaponBoxCollider.enabled = false;
        
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

        // TEST: Add item to inventory:
        if (Input.GetKeyDown(KeyCode.P))
        {
            var potion = new Item("Stinky Potion", ItemType.Potion, 20);
            inventory.AddItem(potion);

            for (var i = 0; i < inventory.items.Count; i++)
            {
                Debug.Log("Number of Items: " + inventory.items.Count);
                Debug.Log("Item: [" + i + "] " + potion.name + ", " + potion.price + ", " + potion.type);
            }
        }
    }

    // Displays (to UI) the players gold:
    private void AssignGold()
    {
        currentGoldUI.text = currentGold.ToString();
    }
    
    
    private enum PlayerClass
    {
        Warrior
    }

}
