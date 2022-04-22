using Items;
using UnityEngine;

namespace TDG.Entity
{
    public abstract class Entity : MonoBehaviour
    {
        protected string entityName;
        [SerializeField] protected int currentHealth;
        protected int maxHealth;
        protected int level;
        [SerializeField] public Weapon weapon;

        protected virtual void OnDeath()
        {
            Destroy(gameObject);
        }

        public virtual void TakeDamage(int damage)
        {
            currentHealth -= damage;
        }
    }
}


