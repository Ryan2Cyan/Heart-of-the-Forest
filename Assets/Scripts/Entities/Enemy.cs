using System;
using Core;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Entities
{
    public class Enemy : Entity
    {
        private GameObject player;
        private Player playerScript;
        private GameState gameState;
        
        
        // States
        public EnemyType enemyType;
        public NavMeshAgent enemyNavMesh;
        public bool isAttacking;
        public bool isDead;
        public bool isDamaged;
        private WaveSpawner waveSpawner;
        private float attackTimer;
        private const float damageTime = 0.15f;
        private float damageCounter;
        [SerializeField] private int goldDrop;
        private const int goldMod = 10;
        private float searchRange;
        private float baseHP;
        private float hpIncrement;
        
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

        // Audio:
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
                    baseHP = 50f;
                    hpIncrement = 20f;
                    maxHealth = CalcHpMod(5f, baseHP, hpIncrement);
                    searchRange = 30f;
                    break;
                case EnemyType.LargeSkeleton:
                    baseHP = 250f;
                    hpIncrement = 50f;
                    maxHealth = CalcHpMod(5f, baseHP, hpIncrement);
                    break;
                case EnemyType.YellowSkeleton:
                    baseHP = 40f;
                    hpIncrement = 20f;
                    maxHealth = CalcHpMod(5f, baseHP, hpIncrement);
                    searchRange = 4f;
                    break;
                case EnemyType.Bat:
                    baseHP = 20f;
                    hpIncrement = 20f;
                    maxHealth = CalcHpMod(5f, baseHP, hpIncrement);
                    break;
                case EnemyType.LargeBat:
                    baseHP = 150f;
                    hpIncrement = 50f;
                    maxHealth = CalcHpMod(5f, baseHP, hpIncrement);
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
                SetTarget(enemyType);
                MatDamage();
                if (currentHealth <= 0) // If dead:
                {
                    OnDeath();
                    DropGold();
                    gameState.transform.GetChild(0).GetComponent<WaveSpawner>().aliveEnemies.Remove(gameObject);
                }
            }
        }

        // Sets the target for the enemy depending on the type:
        private void SetTarget(EnemyType arg)
        {
            switch (arg)
                {
                    case EnemyType.BlackSkeleton: // Attacks player if in range, else core:
                        if (Math.Abs(Math.Abs(transform.position.x) - Math.Abs(player.transform.position.x)) <= searchRange && 
                            Math.Abs(Math.Abs(transform.position.z) - Math.Abs(player.transform.position.z)) <= searchRange)
                            enemyNavMesh.SetDestination(player.transform.position);
                        else
                            enemyNavMesh.SetDestination(core.transform.position);
                        break;
                    case EnemyType.YellowSkeleton: // Attacks 1 of 3 buildings (randomly selected):
                        if (Math.Abs(Math.Abs(transform.position.x) - Math.Abs(player.transform.position.x)) <= searchRange && 
                            Math.Abs(Math.Abs(transform.position.z) - Math.Abs(player.transform.position.z)) <= searchRange)
                            enemyNavMesh.SetDestination(player.transform.position);
                        else
                        {
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
                        }
                        break;
                    case EnemyType.Bat: // Attacks player:
                        enemyNavMesh.SetDestination(player.transform.position);
                        break;
                    case EnemyType.LargeSkeleton: // Attacks core:
                        enemyNavMesh.SetDestination(core.transform.position);
                        break;
                    case EnemyType.LargeBat: // Attacks 1 of 3 buildings (randomly selected):
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
                    case EnemyType.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(arg), arg, null);
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

        // Give player specified amount of gold:
        private void DropGold()
        {
            var gold = Random.Range(
                Mathf.RoundToInt(goldDrop - 5), 
                Mathf.RoundToInt(goldDrop + 10)) + goldMod * playerScript.goldAccumulationLvl;
            playerScript.currentGold += gold;
            playerScript.globalGold += gold;
        }

        // Calculates how much extra HP the enemy gets (on spawn):
        private float CalcHpMod(float waveToIncrement, float baseHp, float increment)
        {
            return baseHp + increment * Mathf.Floor(gameState.currentWave / waveToIncrement);
        }
        
        // Change material for brief time when hit:
        private void MatDamage()
        {
            if (isDamaged)
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
                        else if (other.gameObject.CompareTag("Core") && attackTimer <= 0.0f)
                        {
                            if (!isDead)
                            {
                                // If black skeleton is close to its target, attack it:
                                // NOTE: This avoids the skeleton attacking buildings while walking by:
                                if (Math.Abs(Math.Abs(other.gameObject.transform.position.x) - Math.Abs(enemyNavMesh.destination.x)) <= 5 && 
                                    Math.Abs(Math.Abs(other.gameObject.transform.position.z) - Math.Abs(enemyNavMesh.destination.z)) <= 5)
                                {
                                    Attack(other.gameObject.GetComponent<Entity>());
                                    other.GetComponent<Shopkeep>().UpdateHPBars();
                                    src.PlayOneShot(hitBuildingSound);
                                    attackTimer = weapon.attackSpeed;
                                }

                                if (other.gameObject.GetComponent<Shopkeep>().isDead)
                                {
                                    // Game over:
                                    gameState.CoreDestroyed();
                                }
                            }
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
                                // If yellow skeleton is close to its target, attack it:
                                // This avoids the skeleton attacking buildings while walking by.
                                if(Math.Abs(Math.Abs(other.gameObject.transform.position.x) - Math.Abs(enemyNavMesh.destination.x)) <= searchRange && Math.Abs(Math.Abs(other.gameObject.transform.position.z) - Math.Abs(enemyNavMesh.destination.z)) <= searchRange)
                                {
                                    Attack(other.gameObject.GetComponent<Entity>());
                                    other.GetComponent<Shopkeep>().UpdateHPBars();
                                    src.PlayOneShot(hitBuildingSound);
                                    attackTimer = weapon.attackSpeed;
                                }
                                
                                
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
                        else if (other.gameObject.CompareTag("Player") && attackTimer <= 0.0f)
                        {
                            Attack(other.gameObject.GetComponent<Entity>());
                            attackTimer = weapon.attackSpeed;
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
                                gameState.CoreDestroyed();
                            }
                        }
                        break;
                    case EnemyType.LargeBat:
                        if (other.gameObject.CompareTag("Building") && attackTimer <= 0.0f)
                        {
                            // Fetch shop scripts:
                            Shopkeep shopScript = other.gameObject.GetComponent<Shopkeep>();
                            Entity shopEntityScript = other.gameObject.GetComponent<Entity>();
                            
                            // Deal damage to the shop:
                            Attack(shopEntityScript);
                            shopScript.UpdateHPBars();
                            src.PlayOneShot(hitBuildingSound);
                            attackTimer = weapon.attackSpeed;
                            
                            // Check if building HP is 0 - if true destroy the building:
                            if (other.gameObject.GetComponent<Entity>().currentHealth <= 0)
                            {
                                shopScript.isDead = true;
                                shopEntityScript.OnDeath();
                            }

                            // If building is destroyed, spawn 3 bats:
                            if (other.gameObject.GetComponent<Shopkeep>().isDead)
                            {
                                Debug.Log("building dead");
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
                    if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Core"))
                    {
                        isAttacking = true;
                    }
                    break;
                case EnemyType.Bat:
                    if (other.gameObject.CompareTag("Player"))
                    {
                        isAttacking = true;
                    }
                    break;
                case EnemyType.YellowSkeleton:
                    if (other.gameObject.CompareTag("Building"))
                    {
                        if (Math.Abs(Math.Abs(other.gameObject.transform.position.x) - Math.Abs(enemyNavMesh.destination.x)) <= searchRange && Math.Abs(Math.Abs(other.gameObject.transform.position.z) - Math.Abs(enemyNavMesh.destination.z)) <= searchRange)
                        {
                            isAttacking = true;
                        }
                    }
                    else if(other.gameObject.CompareTag("Player"))
                    {
                        isAttacking = true;
                    }
                    break;
                case EnemyType.LargeSkeleton:
                    if (other.gameObject.CompareTag("Core"))
                    {
                        isAttacking = true;
                    }
                    break;
                case EnemyType.LargeBat:
                    if (other.gameObject.CompareTag("Building"))
                    {
                        isAttacking = true;
                    }
                    break;
                case EnemyType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        } 
        
        // Check if player is not colliding to halt attacking:
        private void OnTriggerExit(Collider other)
        {
            switch (enemyType)
            {
                case EnemyType.BlackSkeleton:
                    isAttacking = false;
                    break;
                case EnemyType.Bat:
                    isAttacking = false;
                    break;
                case EnemyType.YellowSkeleton:
                    isAttacking = false;
                    break;
                case EnemyType.LargeSkeleton:
                    isAttacking = false;
                    break;
                case EnemyType.LargeBat:
                    if (other.gameObject.CompareTag("Building"))
                    {
                        isAttacking = false;
                    }
                    break;
                case EnemyType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        // Activate on death:
        public override void OnDeath()
        {
            waveSpawner.aliveEnemies.Remove(gameObject);
            isDead = true;
            if (enemyType == EnemyType.Bat || enemyType == EnemyType.LargeBat)
            {
                enemyNavMesh.baseOffset = 0.2f; // Move enemy to ground if bat.  
            }
            src.PlayOneShot(deadSound);
            enemyNavMesh.enabled = false;
            model.material = deathMat;
            gameObject.GetComponent<Enemy>().enabled = false;
            gameObject.GetComponent<SphereCollider>().enabled = false;
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
