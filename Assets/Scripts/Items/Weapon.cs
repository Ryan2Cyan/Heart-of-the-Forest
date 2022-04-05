using Items;
using UnityEngine;

public class Weapon : Item
{
    protected string name;
    protected int price;
    protected ItemType type;
    public string weaponType;
    public float attackSpeed;
    public int damage;
    public AudioSource src;
    public AudioClip clip;

    private void UpgradeWeapon()
    {
        //upgrade stuff here maybe
    }

    public Weapon(string name, ItemType type, int price) : base(name, type, price)
    {
        this.name = name;
        this.type = ItemType.Weapon;
        this.price = price;
    }
}
