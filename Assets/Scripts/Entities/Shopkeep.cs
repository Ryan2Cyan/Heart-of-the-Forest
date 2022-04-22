using TDG.Entity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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
    private int rangeLvl;
    private int damageLvl;
    private int attackSpeedLvl;
    public bool isDead { get; private set; }
    
    [SerializeField] private Slider hpBarSlider;
    [SerializeField] private TextMeshProUGUI upgradePrice;
    [SerializeField] private AudioClip repairSfx;
    [SerializeField] private AudioClip destroySfx;
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
        maxHealth = 100;
        currentHealth = maxHealth;
        if(hpBarSlider)
            hpBarSlider.value = maxHealth;
        rangeLvl = 0;
        damageLvl = 0;
        attackSpeedLvl = 0;
    }

    private void Update()
    {

        if (Cursor.visible)
        {
            Debug.Log("Visible");
        }
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
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            menu.SetActive(false);
            menuOpen = false;
            text.SetActive(true);
        }

        if (currentHealth <= 0 && !isDead)
        {
            OnDeath();
        }
    }

    // Switch to next level of shop model:
    public void Upgrade()
    {
        // Costs to upgrade the shops:
        const int cost0 = 250;
        const int cost1 = 500;
        
        if (!lvl3Model.activeInHierarchy)
        {
            if (lvl1Model.activeInHierarchy && playerScript.currentGold >= cost0)
            {
                upgradePrice.text = cost1.ToString();
                playerScript.currentGold -= cost0;
                src.PlayOneShot(purchaseSfx);
                lvl1Model.SetActive(false);
                lvl2Model.SetActive(true);
            }
            else if (lvl2Model.activeInHierarchy&& playerScript.currentGold >= cost1)
            {
                upgradePrice.text = "";
                playerScript.currentGold -= cost1;
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

    // Repairs the shop, making it accessible for the player once again:
    public void Repair()
    {
        const int cost0 = 100;
        if (playerScript.currentGold >= cost0 && currentHealth != maxHealth)
        {
            src.PlayOneShot(repairSfx);
            isDead = false;
            lvl1Model.SetActive(true);
            playerScript.currentGold -= cost0;
            currentHealth = maxHealth;
            hpBarSlider.value = maxHealth;
        }
        else
            src.PlayOneShot(nullSfx);
    }
    
    // Upgrades a weapon's range:
    public void UpgradeRange()
    {
        var cost0 = CalcCost(100, 25, rangeLvl);
        var cost = GameObject.Find("Upgrade-Range-Price");
        cost.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + cost0;
        if (playerScript.currentGold >= cost0)
        {
            src.PlayOneShot(purchaseSfx);
            rangeLvl += 1;
            playerScript.currentGold -= cost0;
            var playerSword = player.transform.GetChild(0).GetChild(0);
            // Elongate sword collider:
            playerSword.GetComponent<BoxCollider>().size += new Vector3(1.2f, 1.2f, 1.2f);
            playerSword.GetComponent<BoxCollider>().center += new Vector3(0f, 0f, 0.8f);

            // Elongate sword model:
            playerSword.GetChild(0).GetChild(1).localScale += new Vector3(0f, 0.5f, 0f);
            if(rangeLvl > 4)
                playerSword.GetChild(0).GetChild(1).localPosition += new Vector3(0.0f, 0.015f, 0.0f);
            cost0 = CalcCost(100, 25, rangeLvl);
            cost.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + cost0;
        }
        else
            src.PlayOneShot(nullSfx);
    }
    
    // Upgrades a weapon's attack speed:
    public void UpgradeAttackSpeed()
    {
        var cost0 = CalcCost(100, 25, attackSpeedLvl);
        var cost = GameObject.Find("Upgrade-AttackSpeed-Price");
        cost.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + cost0;
        if (playerScript.currentGold >= cost0)
        {
            src.PlayOneShot(purchaseSfx);
            attackSpeedLvl += 1;
            playerScript.currentGold -= cost0;
            playerScript.attackDelay -= 0.07f;
            playerScript.GetComponent<Entity>().weapon.attackSpeed -= 0.07f;
            cost0 = CalcCost(100, 25, attackSpeedLvl);
            cost.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + cost0;
        }
        else
            src.PlayOneShot(nullSfx);
    }
    
    // Upgrades a weapon's damage:
    public void UpgradeDamage()
    {
        var cost0 = CalcCost(100, 25, damageLvl);
        var cost = GameObject.Find("Upgrade-Damage-Price");
        cost.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + cost0;
        if (playerScript.currentGold >= cost0)
        {
            src.PlayOneShot(purchaseSfx);
            damageLvl += 1;
            playerScript.currentGold -= cost0;
            playerScript.weapon.damage += 20;
            cost0 = CalcCost(100, 25, damageLvl);
            cost.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + cost0;
        }
        else
            src.PlayOneShot(nullSfx);
    }

    // Quick method to calculate the rise in cost of an upgrade:
    private int CalcCost(int startVal, int increment, int lvl)
    {
        var result = startVal;
        switch (lvl)
        {
            case 0:
                result = startVal;
                break;
            case 1:
                result += increment * lvl;
                break;
            case 2:
                result += increment * lvl;
                break;
            case 3:
                result += increment * lvl;
                break;
            case 4:
                result += increment * lvl;
                break;
            case 5:
                result += increment * lvl;
                break;
            case 6:
                result += increment * lvl;
                break;
            case 7:
                result += increment * lvl;
                break;
            case 8:
                result += increment * lvl;
                break;
        }

        return result;
    }
    
    // Called when shop is destroyed:
    protected override void OnDeath()
    {
        src.PlayOneShot(destroySfx);
        lvl3Model.SetActive(false);
        lvl2Model.SetActive(false);
        lvl1Model.SetActive(false);
        text.SetActive(false);
        menu.SetActive(false);
        isDead = true;
    }
    
    // Reduces current HP:
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if(hpBarSlider)
            hpBarSlider.value = currentHealth;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isDead)
        {
            // Display interaction text when close to the shop:
            if (other.gameObject.CompareTag("Player"))
            {
                text.SetActive(true);
                playerCollision = true;
            }
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
