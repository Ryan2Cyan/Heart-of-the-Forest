using System;
using TMPro;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;


public class Shopkeep : MonoBehaviour
{
    // [SerializeField] private Material highlightMat;
    // private Material defaultMat;
    // private Renderer renderer;
    private Collider currentCollider;
    private bool isSelected;
    private bool playerCollision;
    private bool menuOpen;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject text;
    [SerializeField] private GameObject menu;

    private void Start()
    {
        // defaultMat = transform.GetComponent<Renderer>().material;
        // renderer = transform.GetComponent<Renderer>();
        // Get components:
        currentCollider = transform.GetComponent<SphereCollider>();
        player = GameObject.FindWithTag("Player");
        
        // Set values:
        menuOpen = false;
        playerCollision = false;
    }

    private void Update()
    {
        // isSelected = SelectionCheck();
        
        // Check if the player is in range of the shop. Toggle menu by pressing "E":
        if (Input.GetKeyDown(KeyCode.E) && playerCollision && !menuOpen) // Open menu
        {
            // Unlock cursor:
            player.GetComponent<FirstPersonController>().enabled = false; 
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            menu.SetActive(true);
            text.SetActive(false);
            menuOpen = true;
        }
        else if (Input.GetKeyDown(KeyCode.E) && menuOpen) // Close menu
        {
            player.GetComponent<FirstPersonController>().enabled = true; 
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
