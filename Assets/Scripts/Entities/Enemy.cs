using TDG.Entity;
using UnityEngine;
using UnityEngine.AI;

namespace Entities
{
    public class Enemy : Entity
    {
        private GameObject player;
        private Player playerScript;

        [SerializeField] private GameObject blackSkeleton;
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
        private const int goldMod = 5;
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
        private int buildingTarget;
        private GameObject core;
        private GameState gameState;
        private AudioSource src;
        public AudioClip deadSkeletonSound;
        

        private void Start()
        {
            // Fetch components:
            player = GameObject.FindWithTag("Player");
            playerScript = player.GetComponent<Player>();
            armorsmith = GameObject.Find("Armorsmith");
            alchemist = GameObject.Find("Alchemist");
            blacksmith = GameObject.Find("Blacksmith");
            core = GameObject.Find("Core");
            gameState = GameObject.Find("GameState").GetComponent<GameState>();
            src = GetComponent<AudioSource>();
            
            // Set values:
            maxHealth = 50;
            currentHealth = maxHealth;
            isDead = false;
            isDamaged = false;
            damageCounter = damageTime;
            defaultMat = model.material;
            buildingTarget = Random.Range(1, 4);
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
                        switch (buildingTarget)
                        {
                            case 1:
                                enemyNavMesh.SetDestination(alchemist.transform.position);
                                break;
                            case 2:
                                enemyNavMesh.SetDestination(armorsmith.transform.position);
                                break;
                            case 3:
                                enemyNavMesh.SetDestination(blacksmith.transform.position);
                                break;
                        }
                        break;
                }
                if (isDamaged) // If damaged
                {
                    TakeDamage();
                }

                if (currentHealth <= 0) // If dead
                {
                    OnDeath();
                    gameState.RemoveEnemy(this);
                } 
            }
        }

        // Enemy deals damage to the player:
        private void Attack(Entity target)
        {
            // If the player's armor resists an attack, the attack misses:
            var playerResist = 5 * playerScript.resistanceLvl;
            if (Random.Range(0, 100) >= playerResist)
            {
                target.TakeDamage(weapon.damage);
            }
        }

        // Activate on death:
        protected override void OnDeath()
        {
            if(enemyType == EnemyType.Bat)
                enemyNavMesh.baseOffset = 0.2f; // Move enemy to ground if bat.
            playerScript.currentGold += goldDrop + goldMod * playerScript.goldAccumulationLvl;
            isDead = true;
            src.PlayOneShot(deadSkeletonSound);
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
                        Attack(other.gameObject.GetComponent<Entity>());
                        attackTimer = weapon.attackSpeed;
                    }

                    break;
                case EnemyType.YellowSkeleton:
                    if (other.gameObject.CompareTag("Building") && attackTimer <= 0.0f)
                    {
                        Attack(other.gameObject.GetComponent<Entity>());
                        attackTimer = weapon.attackSpeed;
                        // If the shop is destroyed, convert yellow skeleton into black skeleton:
                        if (other.gameObject.GetComponent<Shopkeep>().isDead)
                        {
                            Instantiate(blackSkeleton, transform.position, Quaternion.identity);
                            Destroy(gameObject);
                        }
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
                    if (other.gameObject.CompareTag("Player"))
                    {
                        isAttacking = false;
                    }

                    break;
                case EnemyType.YellowSkeleton:
                    if (other.gameObject.CompareTag("Building"))
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
