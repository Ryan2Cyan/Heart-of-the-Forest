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
        public GameObject blackSkeleton;
        public GameObject yellowSkeleton;
        public GameObject giantSkeleton;
        public GameObject bat;
        public GameObject giantBat;
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
                // enemiesToSpawn = gameStateScript.currentWave + 2;
                enemiesToSpawn = 1;

                if (!spawned)
                {
                    for (var i = 0; i < enemiesToSpawn; i++)
                    {
                        var position = centreOfSpawn.transform.position;
                        var pos = RandomCircle(position, 50f);
                        var rot = Quaternion.FromToRotation(Vector3.forward, position - pos);

                        
                        var numb = Random.Range(0, 100);

                        // 45% likely to spawn black skeleton:
                        if (numb <= 45)
                        {
                            var newEnemy = Instantiate(blackSkeleton, pos, rot);
                            newEnemy.transform.parent = transform;
                            enemies.Add(newEnemy);
                            aliveEnemies.Add(newEnemy);
                        }
                        // 30% likely to spawn yellow skeleton:
                        else if (numb > 45 && numb <= 75)
                        {
                            var newEnemy = Instantiate(yellowSkeleton, pos, rot);
                            newEnemy.transform.parent = transform;
                            enemies.Add(newEnemy);
                            aliveEnemies.Add(newEnemy);
                        }
                        // 15% likely to spawn bat:
                        else if (numb > 75 && numb <= 93)
                        {
                            var newEnemy = Instantiate(bat, pos, rot);
                            newEnemy.transform.parent = transform;
                            enemies.Add(newEnemy);
                            aliveEnemies.Add(newEnemy);
                        }
                        // 3% likely to spawn giant bat:
                        else if (numb > 93 && numb <= 98)
                        {
                            var newEnemy = Instantiate(giantBat, pos, rot);
                            newEnemy.transform.parent = transform;
                            enemies.Add(newEnemy);
                            aliveEnemies.Add(newEnemy);
                        }
                        // 2% likely to spawn giant skeleton:
                        else if (numb > 98 && numb <= 100)
                        {
                            var newEnemy = Instantiate(giantSkeleton, pos, rot);
                            newEnemy.transform.parent = transform;
                            enemies.Add(newEnemy);
                            aliveEnemies.Add(newEnemy);
                        }
                    }
                    spawned = true;
                }
            }

            if (gameStateScript.isDay && spawned)
            {
                spawned = false;
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
