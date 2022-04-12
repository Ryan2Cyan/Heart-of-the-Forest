using Entities;
using UnityEngine;

namespace Items
{
    public class WeaponHitBox : MonoBehaviour
    {
        [SerializeField] private Weapon weapon;

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Enemy") && other.GetComponent<Enemy>().isSelected)
            {
                other.GetComponent<Enemy>().TakeDamage(weapon.damage);
            }
        }
    }
}
