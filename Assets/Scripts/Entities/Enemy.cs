using System;
using Core;
using TDG.Entity;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Entities
{
    public class Enemy : Entity
    {
        private GameObject player;
        private Player playerScript;
        
        
        // States
        public EnemyType enemyType;
        public NavMeshAgent enemyNavMesh;
        private WaveSpawner waveSpawner;
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
        // Buildings:
        private GameObject armorsmith;
        private GameObject alchemist;
        private GameObject blacksmith;
        private int buildingTarget;
        private GameObject core;
        private GameState gameState;
        private AudioSource src;
        public AudioClip deadSound;
        public AudioClip hitBuildingSound;


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
            waveSpawner = GameObject.Find("WaveSpawner").GetComponent<WaveSpawner>();
            src = GetComponent<AudioSource>();
            
            // Set values:
            switch (enemyType)
            {
                case EnemyType.BlackSkeleton:
                    maxHealth = 50;
                    break;
                case EnemyType.LargeSkeleton:
                    maxHealth = 150;
                    break;
                case EnemyType.YellowSkeleton:
                    maxHealth = 40;
                    break;
                case EnemyType.Bat:
                    maxHealth = 20;
                    break;
                case EnemyType.LargeBat:
                    maxHealth = 100;
                    break;
                case EnemyType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
                    case EnemyType.BlackSkeleton: // Attacks player
                        enemyNavMesh.SetDestination(player.transform.position);
                        break;
                    case EnemyType.YellowSkeleton: // Attacks 1 of 3 buildings
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
                    case EnemyType.Bat: // Attacks player
                        enemyNavMesh.SetDestination(player.transform.position);
                        break;
                    case EnemyType.LargeSkeleton: // Attacks core
                        enemyNavMesh.SetDestination(core.transform.position);
                        break;
                    case EnemyType.LargeBat: // Attacks 1 of 3 buildings
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
                    gameState.transform.GetChild(0).GetComponent<WaveSpawner>().aliveEnemies.Remove(gameObject);
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
            waveSpawner.aliveEnemies.Remove(gameObject);
            isDead = true;
            if(enemyType == EnemyType.Bat || enemyType == EnemyType.LargeBat)
                enemyNavMesh.baseOffset = 0.2f; // Move enemy to ground if bat.
            playerScript.currentGold += goldDrop + goldMod * playerScript.goldAccumulationLvl;
            src.PlayOneShot(deadSound);
            enemyNavMesh.enabled = false;
            model.material = deathMat;
            gameObject.GetComponent<Enemy>().enabled = false;
            gameObject.GetComponent<SphereCollider>().enabled = false;
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
            if (!isDead)
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
                    
                    case EnemyType.Bat:
                        if (other.gameObject.CompareTag("Player") && attackTimer <= 0.0f)
                        {
                            Attack(other.gameObject.GetComponent<Entity>());
                            attackTimer = weapon.attackSpeed;
                        }
                        break;
                    
                    case EnemyType.YellowSkeleton:
                        if (other.gameObject.CompareTag("Building") && attackTimer <= 0.0f)
                        {
                            if (!isDead)
                            {
                                Attack(other.gameObject.GetComponent<Entity>());
                                other.GetComponent<Shopkeep>().UpdateHPBars();
                                src.PlayOneShot(hitBuildingSound);
                                attackTimer = weapon.attackSpeed;
                                
                                // If the shop is destroyed, convert yellow skeleton into black skeleton:
                                if (other.gameObject.GetComponent<Shopkeep>().isDead)
                                {
                                    var newEnemy = Instantiate(
                                        waveSpawner.blackSkeleton, transform.position, Quaternion.identity
                                        );
                                    waveSpawner.enemies.Add(newEnemy);
                                    waveSpawner.aliveEnemies.Add(newEnemy);
                                    OnDeath();
                                }
                            }
                        }
                        break;
                    
                    case EnemyType.LargeSkeleton:
                        if (other.gameObject.CompareTag("Core") && attackTimer <= 0.0f)
                        {
                            Attack(other.gameObject.GetComponent<Entity>());
                            other.GetComponent<Shopkeep>().UpdateHPBars();
                            src.PlayOneShot(hitBuildingSound);
                            attackTimer = weapon.attackSpeed;

                            if (other.gameObject.GetComponent<Shopkeep>().isDead)
                            {
                                // Game over:
                                Destroy(gameObject);
                            }
                        }
                        break;
                    
                    case EnemyType.LargeBat:
                        if (other.gameObject.CompareTag("Building") && attackTimer <= 0.0f)
                        {
                            Attack(other.gameObject.GetComponent<Entity>());
                            other.GetComponent<Shopkeep>().UpdateHPBars();
                            src.PlayOneShot(hitBuildingSound);
                            attackTimer = weapon.attackSpeed;
                            
                            // If building is destroyed, spawn 3 bats:
                            if (other.gameObject.GetComponent<Shopkeep>().isDead)
                            {
                                OnDeath();
                                for (var i = 0; i < 3; i++)
                                {
                                    var newEnemy = Instantiate(
                                        waveSpawner.bat, transform.position, Quaternion.identity
                                        );
                                    waveSpawner.enemies.Add(newEnemy);
                                    waveSpawner.aliveEnemies.Add(newEnemy);
                                }
                            }
                        }
                        break;
                }
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
                case EnemyType.Bat:
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
                case EnemyType.LargeSkeleton:
                    if (other.gameObject.CompareTag("Core") && attackTimer <= 0.0f)
                    {
                        isAttacking = true;
                    }
                    break;
                case EnemyType.LargeBat:
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
                case EnemyType.Bat:
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
                case EnemyType.LargeSkeleton:
                    break;
                case EnemyType.LargeBat:
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
