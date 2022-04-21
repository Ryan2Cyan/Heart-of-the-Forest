using TDG.Entity;
using TMPro;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;


public class Shopkeep : Entity
{
    // [SerializeField] private Material highlightMat;
    // private Material defaultMat;
    // private Renderer renderer;
    private Collider currentCollider;
    private bool isSelected;
    private bool playerCollision;
    private bool menuOpen;
    private GameObject currentModel;
    private GameObject lvl1Model;
    private GameObject lvl2Model;
    private GameObject lvl3Model;
    private GameObject player;
    private Player playerScript;
    
    [SerializeField] private TextMeshProUGUI upgradePrice;
    [SerializeField] private AudioClip nullSfx;
    [SerializeField] private AudioClip purchaseSfx;
    [SerializeField] private AudioSource src;
    [SerializeField] private ShopType shopType;
    [SerializeField] private GameObject text;
    [SerializeField] private GameObject menu;
    
    

    private void Start()
    {
        // defaultMat = transform.GetComponent<Renderer>().material;
        // renderer = transform.GetComponent<Renderer>();
        
        // Get components:
        currentCollider = transform.GetComponent<SphereCollider>();
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<Player>();
        if (shopType != ShopType.Core)
        {
            lvl1Model = transform.GetChild(0).gameObject;
            lvl2Model = transform.GetChild(1).gameObject;
            lvl3Model = transform.GetChild(2).gameObject;
            lvl1Model.SetActive(true);
        }

        // Set values:
        menuOpen = false;
        playerCollision = false;
        maxHealth = 1000;
        currentHealth = maxHealth;
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

    // Switch to next level of shop model:
    public void Upgrade()
    {
        // Costs to upgrade the shops:
        var cost1 = 250;
        var cost2 = 500;
        
        if (!lvl3Model.activeInHierarchy)
        {
            if (lvl1Model.activeInHierarchy && playerScript.currentGold >= cost1)
            {
                upgradePrice.text = cost2.ToString();
                playerScript.currentGold -= cost1;
                src.PlayOneShot(purchaseSfx);
                lvl1Model.SetActive(false);
                lvl2Model.SetActive(true);
            }
            else if (lvl2Model.activeInHierarchy&& playerScript.currentGold >= cost2)
            {
                upgradePrice.text = "";
                playerScript.currentGold -= cost2;
                src.PlayOneShot(purchaseSfx);
                lvl2Model.SetActive(false);
                lvl3Model.SetActive(true);
            }
            else
                src.PlayOneShot(nullSfx);
        }
        else
            src.PlayOneShot(nullSfx);
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

    private enum ShopType
    {
        Blacksmith,
        Armorsmith,
        Alchemist,
        Core,
        None
    }
}
