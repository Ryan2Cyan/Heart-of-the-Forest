using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TDG.Entity;
using TDG.Items;

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

    public Item BuyItem(Item choice, int amount)
    {
        // fix dis
        Item item = shopInventory.GetItem(choice.ToString());

        return item;
    }

}
