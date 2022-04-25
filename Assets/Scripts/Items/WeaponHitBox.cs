using Entities;
using UnityEngine;

namespace Items
{
    public class WeaponHitBox : MonoBehaviour
    {
        [SerializeField] private Weapon weapon;
        private GameObject player;

        public AudioSource src;
        public AudioClip sfx;
        bool locked = false;

        private void Start()
        {
            player = GameObject.FindWithTag("Player");
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Enemy"))
            {
                Debug.Log("Deal damage");
                var enemyScript = other.GetComponent<Enemy>();
                enemyScript.TakeDamage(weapon.damage);
                enemyScript.isDamaged = true;

                // play hit sound, don't play multiple sounds in the same swing 
                if (locked)
				{

				}
				else
				{
                    src.PlayOneShot(sfx);
                    locked = true;
                    Invoke("SetBoolBack", 0.1f);                  
                }
                            
                // // Calculate the direction between the player and enemy, then knock enemy back:
                // var hitDir = -(player.transform.position = other.transform.position).normalized;
                // other.GetComponent<Enemy>().Knockback(hitDir);
            }
        }

        private void SetBoolBack()
		{
            locked = false;
		}
    }
}
