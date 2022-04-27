using System;
using Entities;
using UnityEngine;

namespace Items
{
    public class PlayerWeapon : MonoBehaviour
    {
        private Player playerScript;
        [SerializeField] private Material bladeMat0;
        [SerializeField] private Material bladeMat1;
        [SerializeField] private Material bladeMat2;
        private Renderer bladeRenderer;
        public int rangeLvl;
        public int damageLvl;
        public int attackSpeedLvl;
        private int totalLevel;
        
        private void Start()
        {
            // Get components:
            playerScript = GameObject.FindWithTag("Player").GetComponent<Player>();
            bladeRenderer = transform.GetChild(0).GetChild(1).GetComponent<Renderer>();
            
            // Set values:
            rangeLvl = 0;
            damageLvl = 0;
            attackSpeedLvl = 0;
            totalLevel = 0;

            // Set starting blade material:
            if (bladeMat0)
                bladeRenderer.material = bladeMat0;
        }

        // Change color of sword depending on its total power:
        public void CalcTotalLevel()
        {
            totalLevel = (rangeLvl + damageLvl + attackSpeedLvl) / 3;
            if (totalLevel >= 5)
                bladeRenderer.material = bladeMat1;
            if(totalLevel >= 9)
                bladeRenderer.material = bladeMat2;
        }
    }
}
