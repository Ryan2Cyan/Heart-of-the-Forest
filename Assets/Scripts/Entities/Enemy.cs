using System.Collections;
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
        public NavMeshAgent enemyNavMesh;
        public bool isAttacking;
        public bool isDead;
        public bool isDamaged;
        private float attackTimer;
        private const float damageTime = 0.15f;
        private float damageCounter;
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
            maxHealth = 50;
            currentHealth = maxHealth;
            // isSelected = false;
            isDead = false;
            isDamaged = false;
            var sphereCollider = GetComponent<SphereCollider>();
            sphereCollider.radius = 2;
            damageCounter = damageTime;
            defaultMat = model.material;

            player = GameObject.FindWithTag("Player");
            playerScript = player.GetComponent<Player>();
            Debug.Log(model.material);
        }
        
        private void Update()
        {
            if (!isDead)
            {
                //Debug.Log(isAttacking);
                enemyNavMesh.SetDestination(player.transform.position);
                
                // Enemy is damaged:
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

                // Enemy is dead:
                if (currentHealth <= 0)
                {
                    isDead = true;
                    enemyNavMesh.enabled = false;
                    model.material = deathMat;
                    gameObject.GetComponent<BoxCollider>().enabled = false;
                    gameObject.GetComponent<SphereCollider>().enabled = false;
                    gameObject.GetComponent<NavMeshAgent>().enabled = false;
                    enabled = false;
                }
            }
        }

        private void Attack()
        {
            Debug.Log(entityName + " tried to attack!");
            player.GetComponent<Player>().TakeDamage(weapon.damage);
            isAttacking = true;
        }

        // Check if the enemy has collided with the player:
        private void OnTriggerStay(Collider other)
        {
            attackTimer -= Time.deltaTime;
            if (other.gameObject.CompareTag("Player") && attackTimer <= 0.0f)
            {
                Attack();
                attackTimer = weapon.attackSpeed;
            }
        }
        

        // Checks if the player is currently looking at this shop object:
        // private bool SelectionCheck()
        // {
        //     if (player)
        //     {
        //         if (playerScript.selectedObj.transform == transform)
        //         {
        //             renderer.material = highlightMat;
        //             return true;
        //         }
        //
        //         renderer.material = defaultMat;
        //         return false;
        //     }
        //
        //     Debug.Log("Cannot find object with tag 'Player'");
        //     return false;
        // }
        
        // // Move the enemy back when hit:
        // public void Knockback(Vector3 dir)
        // {
        //     knockBackCounter = knockBackTime;
        //     var moveDirection = dir * knockBackForce;
        //     enemyNavMesh.Move(moveDirection);
        // }
    }
}
