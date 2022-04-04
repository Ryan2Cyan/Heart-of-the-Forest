using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TDG.Items;

public class Weapon : Item
{
    public string weaponType;
    public float attackSpeed;
    public int damage;
    public AudioSource src;
    public AudioClip clip;

    private void UpgradeWeapon()
    {
        //upgrade stuff here maybe
    }
}
