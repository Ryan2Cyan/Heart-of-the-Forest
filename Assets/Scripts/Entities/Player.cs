using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TDG.Entity;
using Items;

public class Player : Entity
{
    [SerializeField] private GameState gameState;

    [SerializeField] private float experience;
    [SerializeField] private float nextLevelExp;

    private Camera fpsCamera;
    private const float attackDelay = 0.65f;
    private float attackTimer;
    private bool useAttack0 = false;
    
    public RaycastHit selectedObj { get; private set; }
    [SerializeField] private PlayerClass playerClass;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Slider hpBarSlider;
    private Animator weaponAnimator;
    private BoxCollider weaponBoxCollider;

    [SerializeField] GameObject[] swordPrefab;
    
    // Indexes:
    private static readonly int AttackWithSword = Animator.StringToHash("AttackWithSword");
    private static readonly int AttackWithSword0 = Animator.StringToHash("AttackWithSword0");

    // Start is called before the first frame update
    private void Start()
    {
        // Grabbing all these components will mean we can upgrade the sword via
        // scripts instead of assigning them in the inspector
        weapon = transform.GetChild(0).transform.GetChild(0).GetComponent<Weapon>();
        weaponAnimator = weapon.gameObject.GetComponent<Animator>();
        weaponBoxCollider = weapon.gameObject.GetComponent<BoxCollider>();


        entityName = "Jargleblarg The Great";
        maxHealth = 30;
        currentHealth = maxHealth;
        experience = 0.0f;
        level = 0;
        nextLevelExp = 20.0f;
        fpsCamera = GetComponentInChildren<Camera>();
        playerClass = PlayerClass.Warrior;
        weapon.attackSpeed = 0.6f;
        weaponAnimator = gameObject.GetComponentInChildren<Animator>();
        attackTimer = attackDelay;
        weaponBoxCollider.enabled = false;

        // Fetch Hp bar [this will need to be changed if we implement multiplayer]:
        hpBarSlider = GameObject.Find("Health bar").GetComponent<Slider>();
        hpBarSlider.value = maxHealth;
        
        gameState = FindObjectOfType<GameState>();

        // Make sure inventory is referenced
        inventory = new Inventory();
    }

    // Update is called once per frame
    private void Update()
    {
        

        // Check if player has fallen off the world:
        FallCheck();
        
        // Process user input:
        ProcessInput();
        
        // Check what the player is currently looking at:
        HighlightInteractable();

        if (experience > nextLevelExp)
        {
            LevelUp();
        }

        if (currentHealth <= 0)
        {
            OnDeath();
        }
    }

    // Reduces current HP:
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        hpBarSlider.value = currentHealth;
    }
    
    // Reset the scene:
    protected override void OnDeath() 
    {
        // Find death canvas and activate it - probably do some extra death stuff here later
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    // Cast ray from player camera's direction, interactable objects hit by the ray are highlighted:
    private void HighlightInteractable()
    {
        var ray = fpsCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(transform.position + new Vector3(0, 1, 0), ray.direction, out var hit, 40.0f))
        {
            selectedObj = hit;
        }
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
        weapon.src.PlayOneShot(weapon.sfx);
        
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

    private void ProcessInput()
    {
        // Toggle night and day:
        if (Input.GetKeyDown(KeyCode.L))
        {
            gameState.ToggleDay();
        }
        
        // Attack:
        attackTimer -= Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && attackTimer <= 0.0f)
        {
            StartCoroutine(AttackCooldown());
            attackTimer = attackDelay;
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

    private enum PlayerClass
    {
        Warrior
    }

    public void UpgradeWeapon()
    {
        // Update weapon values
        weapon.attackSpeed = 100;
        weapon.name = "Bitch Please";

        // Destroy weapon model
        Destroy(weapon.transform.GetChild(0).gameObject);

        // Add new weapon model
        var child = Instantiate(swordPrefab[1], transform.position, Quaternion.identity);
        child.transform.parent = gameObject.transform.GetChild(0).gameObject.transform;


        // >> Upgrade weapon via replacement <<
        //var child = Instantiate(swordPrefab[1], new Vector3(transform.position.x, weapon.transform.position.y, weapon.transform.position.z), Quaternion.identity);
        //child.transform.parent = gameObject.transform.GetChild(0).gameObject.transform;
        //Destroy(weapon.gameObject);

        //weapon = transform.GetChild(0).transform.GetChild(0).GetComponent<Weapon>();
        //weaponAnimator = weapon.gameObject.GetComponent<Animator>();
        //weaponBoxCollider = weapon.gameObject.GetComponent<BoxCollider>();
        //weapon.src = GetComponent<AudioSource>();
    }
}
