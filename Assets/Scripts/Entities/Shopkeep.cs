using System.Collections;
using System.Collections.Generic;
using Items;
using UnityEngine;
using TDG.Entity;

public class Shopkeep : NPC
{
    private Inventory shopInventory;
    private bool accessable;

    [SerializeField] GameState gameState;

    private void Start()
    {
        accessable = true;
        gameState = FindObjectOfType<GameState>();
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
    }

    public ItemType BuyItem(ItemType choice, int amount)
    {


        return choice;
    }

}
