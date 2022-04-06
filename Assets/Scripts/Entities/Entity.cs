using Items;
using UnityEngine;

namespace TDG.Entity
{
    public abstract class Entity : MonoBehaviour
    {
        [SerializeField] protected string entityName;
        [SerializeField] protected int currentHealth;
        [SerializeField] protected int maxHealth;
        [SerializeField] protected float movementSpeed;
        [SerializeField] protected int level;
        [SerializeField] protected Weapon weapon;

        public virtual void Attack()
        {
            Debug.Log(entityName + " tried to attack!");
        }
        public virtual void OnDeath()
        {
            Destroy(gameObject);
        }

        public virtual void TakeDamage(int damage)
        {
            currentHealth -= damage;
        }
    }
}


