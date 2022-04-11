using Entities;
using Items;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] Weapon weapon;
    private Collider FirstCollider;
    private int CollisionCount;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy") && other.GetComponent<Enemy>().isSelected)
        {
            if(FirstCollider == null)
            {
                FirstCollider = other;

                CollisionCount++;

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
