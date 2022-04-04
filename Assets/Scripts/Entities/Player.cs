using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TDG.Entity;
using TDG.Items;
using System;

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
    void Start()
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

        inventory.AddItem(ItemType.Potion);

        // This doesn't work
        // Debug.Log("Player inv 1" + inventory.GetItemType(0));
        
    }

    // Update is called once per frame
    void Update()
    {
       
        // Toggle night and day:
        if (Input.GetKeyDown(KeyCode.L))
        {
            gameState.ToggleDay();
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
