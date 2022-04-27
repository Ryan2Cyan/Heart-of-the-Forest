using System.Collections.Generic;
using Entities;
using UnityEngine;

namespace Core
{
    public class WaveSpawner : MonoBehaviour
    {

        private GameState gameStateScript;
        public List<GameObject> enemies;
        public List<GameObject> aliveEnemies;
        [SerializeField] private GameObject blackSkeleton;
        [SerializeField] private Enemy enemy;
        [SerializeField] private GameObject centreOfSpawn;
        [SerializeField] private int enemiesToSpawn;
        private bool spawned;



        private void Start()
        {
            gameStateScript = FindObjectOfType<GameState>();
            centreOfSpawn = GameObject.Find("CentreOfSpawn");
            enemies = new List<GameObject>();
        }

        private void Update()
        {
            // Check if the time of day is night
            if (!gameStateScript.isDay)
            {
                enemiesToSpawn = 3;

                if (!spawned)
                {
                    for (var i = 0; i < enemiesToSpawn; i++)
                    {
                        var position = centreOfSpawn.transform.position;
                        var pos = RandomCircle(position, 50f);
                        var rot = Quaternion.FromToRotation(Vector3.forward, position - pos);
                        var newEnemy = Instantiate(blackSkeleton, pos, rot);
                        newEnemy.transform.parent = transform;
                        enemies.Add(newEnemy);
                        aliveEnemies.Add(newEnemy);
                    }
                    spawned = true;
                }
            }

            if (gameStateScript.isDay && spawned)
            {
                spawned = false;
                Debug.Log("Call");
                foreach (var enemy in enemies)
                {
                    Destroy(enemy);
                }
                enemies.Clear();
            }
                
        }

        Vector3 RandomCircle(Vector3 center, float radius)
        {
            float ang = Random.value * 360;
            Vector3 pos;
            pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
            pos.y = center.y;
            pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad); ;
            return pos;
        }
    }
}
