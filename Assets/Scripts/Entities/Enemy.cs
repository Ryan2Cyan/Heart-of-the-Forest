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
        

        private void Start()
        {
            // Fetch components:
            player = GameObject.FindWithTag("Player");
            playerScript = player.GetComponent<Player>();
            
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
                enemyNavMesh.SetDestination(player.transform.position);
                if (isDamaged) // If damaged
                    TakeDamage();
                if (currentHealth <= 0) // If dead
                    OnDeath();
            }
        }

        // Enemy deals damage to the player:
        private void Attack()
        {
            Debug.Log(entityName + " tried to attack!");
            playerScript.TakeDamage(weapon.damage);
        }
        

        // Activate on death:
        protected override void OnDeath()
        {
            if(enemyType == EnemyType.Bat)
                enemyNavMesh.baseOffset = 0.2f; // Move enemy to ground if bat.
            playerScript.currentGold += goldDrop;
            Debug.Log(playerScript.currentGold);
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
            if (other.gameObject.CompareTag("Player") && attackTimer <= 0.0f)
            {
                Attack();
                attackTimer = weapon.attackSpeed;
            }
        }

        // Check if player collides to attack:
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                isAttacking = true;
            }
        } 
        
        // Check if player is not colliding to halt attacking:
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                isAttacking = false;
            }
        }
    }

    public enum EnemyType
    {
        Skeleton,
        Slime,
        Bat, 
        Dragon,
        None
    }
}
