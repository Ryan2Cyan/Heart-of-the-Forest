using System;
using Entities;
using UnityEngine;

namespace Items
{
    public class WeaponHitBox : MonoBehaviour
    {
        [SerializeField] private Weapon weapon;
        private GameObject player;

        private void Start()
        {
            player = GameObject.FindWithTag("Player");
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Enemy") && other.GetComponent<Enemy>().isSelected)
            {
                other.GetComponent<Enemy>().TakeDamage(weapon.damage);
                
                // // Calculate the direction between the player and enemy, then knock enemy back:
                // var hitDir = -(player.transform.position = other.transform.position).normalized;
                // other.GetComponent<Enemy>().Knockback(hitDir);
            }
        }
    }
}
