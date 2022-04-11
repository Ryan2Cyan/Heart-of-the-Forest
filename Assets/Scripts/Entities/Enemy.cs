using TDG.Entity;
using UnityEngine;
using UnityEngine.AI;

namespace Entities
{
    public class Enemy : Entity
    {
        [SerializeField] private Material highlightMat;
        public NavMeshAgent enemy;
        private GameObject player;
        private Player playerScript;
        public bool isSelected { get; private set; }
        private  Renderer renderer;
        private Material defaultMat;
        
        
        // Start is called before the first frame update
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

        // Update is called once per frame
        private void Update()
        {
            enemy.SetDestination(player.transform.position);

            if(currentHealth <= 0)
            {
                OnDeath();
            }

            isSelected = SelectionCheck();
        }

        private void OnTriggerEnter(Collider other)
        {
            Attack();
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
