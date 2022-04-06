using TDG.Entity;
using UnityEngine;
using UnityEngine.AI;

namespace Entities
{
    public class Enemy : Entity
    {
        public NavMeshAgent enemy;
        public GameObject player;

        // Start is called before the first frame update
        private void Start()
        {
            entityName = "Goblin";
            maxHealth = 10;
            currentHealth = maxHealth;
            movementSpeed = 5.0f;
            weapon.attackSpeed = 0.3f;

            var sphereCollider = this.GetComponent<SphereCollider>();

            sphereCollider.radius = 2;

            player = GameObject.Find("Player(Clone)");
        }

        // Update is called once per frame
        void Update()
        {
            enemy.SetDestination(player.transform.position);

            if(currentHealth <= 0)
            {
                OnDeath();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Attack();
        }

        public override void Attack()
        {
            Debug.Log(entityName + " tried to attack!");
            player.GetComponent<Player>().TakeDamage(weapon.damage);
        }



    }
}
