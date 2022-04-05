using UnityEngine;

namespace Items
{
    public class Weapon : MonoBehaviour
    {
        protected string name;
        protected int price;
        protected ItemType type;
        public string weaponType;
        public float attackSpeed;
        public int damage;
        public AudioSource src;
        public AudioClip clip;

        private void UpgradeWeapon()
        {
            //upgrade stuff here maybe
        }

    }
}
