using TDG.Entity;
using UnityEngine;
using UnityEngine.AI;

namespace Entities
{
    public class Enemy : Entity
    {
        private GameObject player;
        private Player playerScript;
        // States
        public EnemyType enemyType;
        public NavMeshAgent enemyNavMesh;
        public bool isAttacking;
        public bool isDead;
        public bool isDamaged;
        private float attackTimer;
        private const float damageTime = 0.15f;
        private float damageCounter;
        [SerializeField] private int goldDrop;
        // Materials:
        public SkinnedMeshRenderer model;
        private Material defaultMat;
        [SerializeField] private Material damageMat;
        [SerializeField] private Material deathMat;
        // Knockback: 
        [SerializeField] private float knockBackForce;
        [SerializeField] private float knockBackTime;
        private float knockBackCounter;
        // Buildings:
        private GameObject armorsmith;
        private GameObject alchemist;
        private GameObject blacksmith;
        private GameObject core;
        

        private void Start()
        {
            // Fetch components:
            player = GameObject.FindWithTag("Player");
            playerScript = player.GetComponent<Player>();
            armorsmith = GameObject.Find("Armorsmith");
            alchemist = GameObject.Find("Alchemist");
            blacksmith = GameObject.Find("Blacksmith");
            core = GameObject.Find("Core");
            
            // Set values:
            maxHealth = 50;
            currentHealth = maxHealth;
            isDead = false;
            isDamaged = false;
            damageCounter = damageTime;
            defaultMat = model.material;
        }
        
        private void Update()
        {
            if (!isDead) // If alive:
            {
                switch (enemyType)
                {
                    case EnemyType.BlackSkeleton:
                        enemyNavMesh.SetDestination(player.transform.position);
                        break;
                    case EnemyType.YellowSkeleton:
                        enemyNavMesh.SetDestination(alchemist.transform.position);
                        break;
                }
                if (isDamaged) // If damaged
                    TakeDamage();
                if (currentHealth <= 0) // If dead
                    OnDeath();
            }
        }

        // Enemy deals damage to the player:
        private void Attack(Entity target)
        {
            Debug.Log(entityName + " tried to attack!");
            target.TakeDamage(weapon.damage);
        }
        

        // Activate on death:
        protected override void OnDeath()
        {
            if(enemyType == EnemyType.Bat)
                enemyNavMesh.baseOffset = 0.2f; // Move enemy to ground if bat.
            playerScript.currentGold += goldDrop;
            isDead = true;
            enemyNavMesh.enabled = false;
            model.material = deathMat;
            gameObject.GetComponent<SphereCollider>().enabled = false;
            gameObject.GetComponent<NavMeshAgent>().enabled = false;
            enabled = false;
        }

        // Change material for brief time when hit:
        private void TakeDamage()
        {
            damageCounter -= Time.deltaTime;
            model.material = damageMat;
            if (damageCounter <= 0.0f)
            {
                model.material = defaultMat;
                damageCounter = damageTime;
                isDamaged = false;
            }
        }
        
        // Check if player is continuously colliding to repeatedly attack:
        private void OnTriggerStay(Collider other)
        {
            attackTimer -= Time.deltaTime;
            switch (enemyType)
            {
                case EnemyType.BlackSkeleton:
                    if (other.gameObject.CompareTag("Player") && attackTimer <= 0.0f)
                    {
                        Debug.Log("Attack" + other.gameObject.name);
                        Attack(other.gameObject.GetComponent<Entity>());
                        attackTimer = weapon.attackSpeed;
                    }

                    break;
                case EnemyType.YellowSkeleton:
                    if (other.gameObject.CompareTag("Building") && attackTimer <= 0.0f)
                    {
                        Debug.Log("Attack" + other.gameObject.name);
                        Attack(other.gameObject.GetComponent<Entity>());
                        attackTimer = weapon.attackSpeed;
                    }
                    break;
            }
        }

        // Check if player collides to attack:
        private void OnTriggerEnter(Collider other)
        {
            switch (enemyType)
            {
                case EnemyType.BlackSkeleton:
                    if (other.gameObject.CompareTag("Player") && attackTimer <= 0.0f)
                    {
                        isAttacking = true;
                    }

                    break;
                case EnemyType.YellowSkeleton:
                    if (other.gameObject.CompareTag("Building") && attackTimer <= 0.0f)
                    {
                        isAttacking = true;
                    }
                    break;
            }
        } 
        
        // Check if player is not colliding to halt attacking:
        private void OnTriggerExit(Collider other)
        {
            switch (enemyType)
            {
                case EnemyType.BlackSkeleton:
                    if (other.gameObject.CompareTag("Player") && attackTimer <= 0.0f)
                    {
                        isAttacking = false;
                    }

                    break;
                case EnemyType.YellowSkeleton:
                    if (other.gameObject.CompareTag("Building") && attackTimer <= 0.0f)
                    {
                        isAttacking = false;
                    }
                    break;
            }
        }
    }

    public enum EnemyType
    {
        BlackSkeleton,
        LargeSkeleton,
        YellowSkeleton,
        Bat, 
        LargeBat,
        None
    }
}
