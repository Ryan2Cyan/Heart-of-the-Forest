using System;
using TMPro;
using UnityEngine;


public class Shopkeep : MonoBehaviour
{
    // [SerializeField] private Material highlightMat;
    // private Material defaultMat;
    // private Renderer renderer;
    private Collider currentCollider;
    private bool isSelected;
    private bool playerCollision;
    private bool menuOpen;
    [SerializeField] private Player player;
    [SerializeField] private GameObject text;
    [SerializeField] private GameObject menu;

    private void Start()
    {
        // defaultMat = transform.GetComponent<Renderer>().material;
        // renderer = transform.GetComponent<Renderer>();

        // NOTE
        // Since shop keeps will spawn before player, this won't find the player
        // Update this later
        menuOpen = false;
        playerCollision = false;
        currentCollider = transform.GetComponent<SphereCollider>();
    }

    private void Update()
    {
        // isSelected = SelectionCheck();
        
        // Check if the player is in range of the shop. Toggle menu by pressing "E":
        if (Input.GetKeyDown(KeyCode.E) && playerCollision && !menuOpen)
        {
            menu.SetActive(true);
            text.SetActive(false);
            menuOpen = true;

            player.UpgradeWeapon();
        }
        else if (Input.GetKeyDown(KeyCode.E) && menuOpen)
        {
            menu.SetActive(false);
            menuOpen = false;
            text.SetActive(true);
        }
    }

    // Checks if the player is currently looking at this shop object:
    // private bool SelectionCheck()
    // {
    //     if (player)
    //     {
    //         if (player.selectedObj.transform == transform)
    //         {
    //             renderer.material = highlightMat;
    //             return true;
    //         }
    //
    //         renderer.material = defaultMat;
    //         return false;
    //     }
    //
    //     Debug.Log("Cannot find object with tag 'Player'");
    //     return false;
    // }

    private void OnTriggerEnter(Collider other)
    {
        // Display interaction text when close to the shop:
        if (other.gameObject.CompareTag("Player"))
        {
            text.SetActive(true);
            playerCollision = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            text.SetActive(false);
            playerCollision = false;
            menu.SetActive(false);
            menuOpen = false;
        }
    }
}
