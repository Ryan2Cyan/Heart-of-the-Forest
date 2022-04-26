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
    }

    public enum WeaponType
    {
        Empty,
        Sword,
        Bow
    }
}
