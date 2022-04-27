using UnityEngine;

namespace Items
{
    public class Weapon : MonoBehaviour
    {
        protected string name;
        public WeaponType weaponType;
        public float attackSpeed;
        public int damage;
        public AudioSource src;
        public AudioClip sfx;

        public void PlaySwingSound()
        {
            src.PlayOneShot(sfx);
        }

        public void EnableHitbox()
        {
            GetComponent<BoxCollider>().enabled = true;
        }
        public void DisableHitbox()
        {
            GetComponent<BoxCollider>().enabled = false;
        }
    }

    public enum WeaponType
    {
        Empty,
        Sword,
        Bow
    }
}
