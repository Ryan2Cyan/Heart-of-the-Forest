using System.Collections;
using System.Collections.Generic;
using Items;
using UnityEngine;
using TDG.Entity;

public class Shopkeep : NPC
{
    [SerializeField] private Material highlightMat;
    private Material defaultMat;
    private Renderer renderer;
    private bool isAccessable;
    public Player player;

    [SerializeField] private GameState gameState;

    private void Start()
    {
        gameState = FindObjectOfType<GameState>();
        defaultMat = transform.GetComponent<Renderer>().material;
        renderer = transform.GetComponent<Renderer>();
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    private void Update()
    {
        isAccessable = SelectionCheck();
    }

    // Checks if the player is currently looking at this shop object:
    private bool SelectionCheck()
    {
        if (player)
        {
            if (player.selectedObj.transform == transform)
            {
                renderer.material = highlightMat;
                return true;
            }

            renderer.material = defaultMat;
            return false;
        }

        Debug.Log("Cannot find object with tag 'Player'");
        return false;
    }

}
