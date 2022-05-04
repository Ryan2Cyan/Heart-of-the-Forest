using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class WaveSpawner : MonoBehaviour
    {
        public List<GameObject> enemies { get; set; }
        public List<GameObject> aliveEnemies { get; set; }
        public GameObject blackSkeleton;
        public GameObject yellowSkeleton;
        public GameObject giantSkeleton;
        public GameObject bat;
        public GameObject giantBat;
        private GameObject centreOfSpawn;
        private bool hasSpawned;
        
        // Other:
        private GameState gameStateScript;

        private void Start()
        {
            gameStateScript = FindObjectOfType<GameState>();
            centreOfSpawn = GameObject.Find("CentreOfSpawn");
            enemies = new List<GameObject>();
            aliveEnemies = new List<GameObject>();
        }

        private void Update()
        {
            // Check if the wave has started, if true, spawn enemies:
            SpawnEnemies(gameStateScript.currentWave + 2, gameStateScript.isDay, ref hasSpawned);
        }

        // Spawns a specified amount of enemies:
        private void SpawnEnemies(int amount, bool day, ref bool spawned)
        {
            switch (day)
            {
                case false when !spawned:
                {
                    for (var i = 0; i < amount; i++)
                    {
                        var position = centreOfSpawn.transform.position;
                        var pos = RandomCircle(position, Random.Range(50f, 80f));
                        var rot = Quaternion.FromToRotation(Vector3.forward, position - pos);
                    
                        var randVal = Random.value;
                        // Black Skeleton (45%):
                        if (randVal <= 0.45f)
                        {
                            var newEnemy = Instantiate(blackSkeleton, pos, rot);
                            newEnemy.transform.parent = transform;
                            enemies.Add(newEnemy);
                            aliveEnemies.Add(newEnemy);
                        }
                        // Yellow Skeleton (30%):
                        else if (randVal > 0.45f && randVal <= 0.75f)
                        {
                            var newEnemy = Instantiate(yellowSkeleton, pos, rot);
                            newEnemy.transform.parent = transform;
                            enemies.Add(newEnemy);
                            aliveEnemies.Add(newEnemy);
                        }
                        // Bat (15%):
                        else if (randVal > 0.75f && randVal <= 0.93f)
                        {
                            var newEnemy = Instantiate(bat, pos, rot);
                            newEnemy.transform.parent = transform;
                            enemies.Add(newEnemy);
                            aliveEnemies.Add(newEnemy);
                        }
                        // Giant Bat (3%):
                        else if (randVal > 0.93f && randVal <= 0.98f)
                        {
                            var newEnemy = Instantiate(giantBat, pos, rot);
                            newEnemy.transform.parent = transform;
                            enemies.Add(newEnemy);
                            aliveEnemies.Add(newEnemy);
                        }
                        // Giant Skeleton (2%):
                        else if (randVal > 0.98f && randVal <= 1f)
                        {
                            var newEnemy = Instantiate(giantSkeleton, pos, rot);
                            newEnemy.transform.parent = transform;
                            enemies.Add(newEnemy);
                            aliveEnemies.Add(newEnemy);
                        }
                    }
                    spawned = true;
                    break;
                }
                // Allow for spawning again when wave ends:
                case true:
                    spawned = false;
                    DestroyAllEnemies();
                    break;
            }
        }

        // Destroy all enemy game objects:
        private void DestroyAllEnemies()
        {
            foreach (var enemy in enemies)
            {
                Destroy(enemy);
            }
            enemies.Clear();
        }

        // Returns spawn position in circle radius:
        private static Vector3 RandomCircle(Vector3 center, float radius)
        {
            var ang = Random.value * 360;
            Vector3 pos;
            pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
            pos.y = center.y;
            pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad); 
            return pos;
        }
    }
}
