using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TDG.Entity;
using Items;

public class Player : Entity
{
    [SerializeField] private GameState gameState;

    [SerializeField] private float experience;
    [SerializeField] private float nextLevelExp;

    private Camera fpsCamera;
    [SerializeField] private PlayerClass playerClass;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Slider hpBarSlider;
    [SerializeField] private Animator animator;
    [SerializeField] private BoxCollider boxCollider;

    // Start is called before the first frame update
    private void Start()
    {
        entityName = "Jargleblarg The Great";
        maxHealth = 100;
        currentHealth = maxHealth;
        movementSpeed = 5.0f;
        experience = 0.0f;
        level = 0;
        nextLevelExp = 20.0f;
        fpsCamera = GetComponentInChildren<Camera>();
        playerClass = PlayerClass.Warrior;
        weapon.attackSpeed = 0.3f;
        animator = gameObject.GetComponentInChildren<Animator>();

        // Fetch Hp bar [this will need to be changed if we implement multiplayer]:
        hpBarSlider = GameObject.Find("Health bar").GetComponent<Slider>();
        hpBarSlider.value = maxHealth;
        
        gameState = FindObjectOfType<GameState>();

        // Make sure inventory is referenced
        inventory = gameObject.GetComponentInChildren<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {
        // Toggle night and day:
        if (Input.GetKeyDown(KeyCode.L))
        {
            gameState.ToggleDay();
        }
        
        // TEST: Add item to inventory:
        if (Input.GetKeyDown(KeyCode.P))
        {
            var potion = new Item("Stinky Potion", ItemType.Potion, 20);
            inventory.AddItem(potion);
            
            if (inventory.items.Count == inventory.maxItems)
            {
                Debug.Log("MAX ITEMS: " + inventory.items.Count);
            }
            else
            {
                Debug.Log("NOT MAX: " + inventory.items.Count);
            }

            for (var i = 0; i < inventory.items.Count; i++)
            {
                Debug.Log("Item: [" + i + "] " + potion.name + ", " + potion.price + ", " + potion.type);
            }

        }
        
        HighlightInteractable();

        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }

        if (experience > nextLevelExp)
        {
            LevelUp();
        }

        if (currentHealth <= 0)
        {
            OnDeath();
        }

    }
    
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        hpBarSlider.value = currentHealth;
    }

    // Cast ray from player camera's direction, interactable objects hit by the ray are highlighted:
    private void HighlightInteractable()
    {
        var ray = fpsCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(transform.position + new Vector3(0, 1, 0), ray.direction, out var hit))
        {
            // Check if the object is a shop:
            if (hit.transform.CompareTag("Selectable"))
            {
                var renderer = hit.transform.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = highlightMaterial;
                    hit.transform.GetComponent<Shopkeep>().SetIsSelected(true);
                }
            }
        }
    }

    private void LevelUp()
    {
        level += 1;
        nextLevelExp += nextLevelExp + 20.0f;
    }

    public override void Attack()
    {
        Debug.Log(entityName + " tried to attack!");
        StartCoroutine(AttackCooldown());
    }

    IEnumerator AttackCooldown()
    {
        animator.SetBool("AttackWithSword", true);

        boxCollider.enabled = true;

        weapon.src.PlayOneShot(weapon.clip);
        yield return new WaitForSeconds(weapon.attackSpeed);
        animator.SetBool("AttackWithSword", false);

        boxCollider.enabled = false;
    }

    public override void OnDeath() 
    {
        // Find death canvas and activate it - probably do some extra death stuff here later
        gameObject.transform.GetChild(1).gameObject.SetActive(true);
    }

    private enum PlayerClass
    {
        Warrior
    }
}
