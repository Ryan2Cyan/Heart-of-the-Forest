using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TDG.Entity;

public class Player : Entity
{
    private float experience;

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
    }
}
