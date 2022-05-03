using Entities;
using UnityEngine;
using System.Collections;

namespace Items
{
    public class WeaponHitBox : MonoBehaviour
    {
        [SerializeField] private Weapon weapon;

        public AudioSource src;
        public AudioClip sfx;
        [SerializeField] private ParticleSystem hitVFX;
        private bool locked = false;


        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Enemy"))
            {
                var enemyScript = other.GetComponent<Enemy>();
                enemyScript.TakeDamage(weapon.damage);
                enemyScript.isDamaged = true;

                hitVFX.Play();

                // play hit sound, don't play multiple sounds in the same swing 
                if (!locked)
				{
                    src.PlayOneShot(sfx);
                    locked = true;
                    StartCoroutine(SetBoolBack());                  
                }
            }
        }

        private IEnumerator SetBoolBack()
		{
            yield return new WaitForSeconds(0.1f);
            locked = false;
		}
    }
}
