using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TDG.Entity;
using Items;

public class Player : Entity
{
    [SerializeField] GameState gameState;

    private float experience;
    private float expRequiredForLevelUp;

    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Slider slider;
    [SerializeField] private Animator animator;

    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private bool attacking;
    private float inputDelay;

    // Start is called before the first frame update
    private void Start()
    {
        entityName = "Jargleblarg The Great";
        maxHealth = 100;
        currentHealth = maxHealth;
        movementSpeed = 5.0f;
        experience = 0.0f;
        level = 0;
        expRequiredForLevelUp = 20.0f;
        classType = "Warrior";
        weapon.attackSpeed = 0.3f;
        animator = gameObject.GetComponentInChildren<Animator>();

        slider = GameObject.Find("Health bar").GetComponent<Slider>();
        slider.value = maxHealth;

        gameState = FindObjectOfType<GameState>();

        // Make sure inventory is referenced
        inventory = gameObject.GetComponentInChildren<Inventory>();
        // var newItem = new Item("Potion", ItemType.Potion, 20);
        // inventory.items.Add(newItem);
        // if (inventory.items.Count != 0)
        // {
        //     foreach (var item in inventory.items)
        //     {
        //         Debug.Log(item.GetName());
        //     }
        // }
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
       
            foreach (var item in inventory.items)
            {
                Debug.Log(potion.GetName());
            }
        }
        
        HighLightInteractables();

        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }

        if (experience > expRequiredForLevelUp)
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
        UpdateHealthBar();
    }

    // Currently test code to highlight shopkeeps later
    private void HighLightInteractables()
    {
        Ray ray = GetComponentInChildren<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(transform.position + new Vector3(0,1,0), ray.direction, out hit))
        {
            var selection = hit.transform;
            var selectionRenderer = selection.GetComponent<Renderer>();
            if (selectionRenderer != null && hit.collider.gameObject.tag == "Selectable")
            {
                selectionRenderer.material = highlightMaterial;
            }


            // If a ray is hitting and highlighting something, and the player presses E
            // then if the interactable is a shopkeep, open the shop inventory
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (hit.collider.gameObject.tag == "Selectable")
                {
                    Shopkeep shopInteract = hit.collider.gameObject.GetComponent<Shopkeep>();
                }
            }

        }
        Debug.DrawRay(transform.position + new Vector3(0, 1, 0), ray.direction * hit.distance, Color.yellow);
    }

    IEnumerator SelectionTimer()
    {

        yield return new WaitForEndOfFrame();
    }

    private void UpdateHealthBar()
    {
        slider.value = currentHealth;
    }

    private void LevelUp()
    {
        level += 1;
        expRequiredForLevelUp += expRequiredForLevelUp + 20.0f;
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
        // Find deathcanvas and activate it - probably do some extra death stuff here later
        gameObject.transform.GetChild(1).gameObject.SetActive(true);
    }

    
}
