using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TDG.Entity;
using System;

public class Player : Entity
{
    private float experience;
    private float expRequiredForLevelUp;

    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Slider slider;
    [SerializeField] private Animator animator;

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

        animator = gameObject.GetComponentInChildren<Animator>();

        slider = GameObject.Find("Health bar").GetComponent<Slider>();
        slider.value = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Skip day");
            // SkipDay()
        }

        HighLightInteractables();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(10);
        }

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
            if( selectionRenderer != null && hit.collider.gameObject.tag == "Selectable")
            {
                selectionRenderer.material = highlightMaterial;
            }
        }

        Debug.DrawRay(transform.position + new Vector3(0, 1, 0), ray.direction * hit.distance, Color.yellow);
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
        weapon.src.PlayOneShot(weapon.clip);
        yield return new WaitForSeconds(0.2f);
        animator.SetBool("AttackWithSword", false);
    }

    public override void OnDeath() 
    {
        // Find deathcanvas and activate it
        gameObject.transform.GetChild(1).gameObject.SetActive(true);
    }
}
