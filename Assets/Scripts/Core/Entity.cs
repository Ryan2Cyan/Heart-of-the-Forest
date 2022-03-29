using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] protected string classType;
        [SerializeField] protected Weapon weapon;
        [SerializeField] protected Inventory inventory;

        public virtual void Attack()
        {
            Debug.Log(entityName + " tried to attack!");
        }
        public virtual void Move()
        {

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


