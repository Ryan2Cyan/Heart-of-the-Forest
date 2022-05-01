using Items;
using UnityEngine;

namespace TDG.Entity
{
    public abstract class Entity : MonoBehaviour
    {
        protected string entityName;
        public float currentHealth;
        public float maxHealth;
        protected int level;
        [SerializeField] public Weapon weapon;

        public virtual void OnDeath()
        {
            //Destroy(gameObject);
        }

        public virtual void TakeDamage(int damage)
        {
            currentHealth -= damage;
        }
    }
}


