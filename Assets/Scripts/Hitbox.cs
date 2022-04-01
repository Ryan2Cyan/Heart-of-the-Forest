using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] Weapon weapon;
    private Collider FirstCollider;
    private int CollisionCount;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            if(FirstCollider == null)
            {
                FirstCollider = other;

                FirstCollider.GetComponent<Enemy>().TakeDamage(weapon.damage);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CollisionCount--;

        if (CollisionCount == 0)
            FirstCollider = null;
    }
}
