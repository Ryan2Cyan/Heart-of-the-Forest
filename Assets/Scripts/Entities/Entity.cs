using Items;
using UnityEngine;

namespace Entities
{
    public abstract class Entity : MonoBehaviour
    {
        public float currentHealth;
        public float maxHealth;
        [SerializeField] public Weapon weapon;

        public virtual void OnDeath() {}

        public virtual void TakeDamage(int damage)
        {
            currentHealth -= damage;
        }
    }
}


