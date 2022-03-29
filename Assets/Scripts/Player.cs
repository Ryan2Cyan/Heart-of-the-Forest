using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TDG.Entity;
using System;

public class Player : Entity
{
    private float experience;
    [SerializeField] private Material highlightMaterial;

    // Start is called before the first frame update
    void Start()
    {
        entityName = "Jargleblarg The Great";
        health = 100;
        movementSpeed = 5.0f;
        level = 0;
        classType = "Warrior";
        weapon = new Weapon();
        inventory = new Inventory();

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
    }

    // Currently test code to highlight shopkeeps later
    private void HighLightInteractables()
    {
        Ray ray = GetComponentInChildren<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(transform.position, ray.direction, out hit))
        {
            var selection = hit.transform;
            var selectionRenderer = selection.GetComponent<Renderer>();
            if( selectionRenderer != null)
            {
                selectionRenderer.material = highlightMaterial;
            }
        }

        Debug.DrawRay(transform.position, ray.direction * hit.distance, Color.yellow);
    }
}
