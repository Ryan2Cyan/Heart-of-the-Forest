using System.Collections;
using System.Collections.Generic;
using Items;
using UnityEngine;
using TDG.Entity;

public class Shopkeep : NPC
{
    private Inventory shopInventory;
    private Material defaultMat;
    private bool accessable;
    private bool isSelected;
    private float selectedTime = 0.1f;

    [SerializeField] GameState gameState;

    private void Start()
    {
        accessable = true;
        gameState = FindObjectOfType<GameState>();
        defaultMat = transform.GetComponent<Renderer>().material;
    }

    private void Update()
    {
        if (gameState.isDay)
        {
            accessable = true;
        }
        else
        {
            accessable = false;
        }

        // Return to original material when unselected:
        if (isSelected)
        {
            selectedTime -= Time.deltaTime;
            if (selectedTime <= 0.0f)
            {
                isSelected = false;
                transform.GetComponent<Renderer>().material = defaultMat;
                selectedTime = 0.1f;
            }
        }
    }

    public ItemType BuyItem(ItemType choice, int amount)
    {


        return choice;
    }

    public void SetIsSelected(bool arg)
    {
        isSelected = arg;
    }
    
    public bool GetIsSelected()
    {
        return isSelected;
    }

}
