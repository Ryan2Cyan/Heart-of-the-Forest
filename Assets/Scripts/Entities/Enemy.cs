using TDG.Entity;
using UnityEngine;
using UnityEngine.AI;

namespace Entities
{
    public class Enemy : Entity
    {
        public NavMeshAgent enemyNavMesh;
        public bool isSelected { get; private set; }
        
        [SerializeField] private Material highlightMat;
        private GameObject player;
        private Player playerScript;
        private  Renderer renderer;
        private Material defaultMat;
        
        

        private void Start()
        {
            entityName = "Goblin";
            maxHealth = 10;
            currentHealth = maxHealth;
        
            weapon.attackSpeed = 0.3f;
            isSelected = false;
            var sphereCollider = GetComponent<SphereCollider>();

            sphereCollider.radius = 2;

            player = GameObject.FindWithTag("Player");
            playerScript = player.GetComponent<Player>();
            renderer = transform.GetComponent<Renderer>();
            defaultMat = renderer.material;
        }
        
        private void Update()
        {
            enemyNavMesh.SetDestination(player.transform.position);

            if(currentHealth <= 0)
            {
                OnDeath();
            }

            isSelected = SelectionCheck();
        }
        
        // Attack player when they enter the collider:
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Attack();
            }
        }

        private void Attack()
        {
            Debug.Log(entityName + " tried to attack!");
            player.GetComponent<Player>().TakeDamage(weapon.damage);
        }
        
        // Checks if the player is currently looking at this shop object:
        private bool SelectionCheck()
        {
            if (player)
            {
                if (playerScript.selectedObj.transform == transform)
                {
                    renderer.material = highlightMat;
                    return true;
                }
        
                renderer.material = defaultMat;
                return false;
            }
        
            Debug.Log("Cannot find object with tag 'Player'");
            return false;
        }
    }
}
