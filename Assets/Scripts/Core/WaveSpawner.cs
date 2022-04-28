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
        [SerializeField] private GameObject yellowSkeleton;
        [SerializeField] private GameObject giantSkeleton;
        [SerializeField] private GameObject bat;
        [SerializeField] private GameObject giantBat;
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

                        var numb = Random.Range(0, 100);
                        Debug.Log(numb);
                        // 40% likely to spawn black skeleton:
                        if (numb < 40)
                        {
                            Debug.Log("Spawn BlackSkeleton");
                            var newEnemy = Instantiate(blackSkeleton, pos, rot);
                            newEnemy.transform.parent = transform;
                            enemies.Add(newEnemy);
                            aliveEnemies.Add(newEnemy);
                        }
                        // 15% likely to spawn bat:
                        else if (numb > 40 && numb <= 55)
                        {
                            Debug.Log("Spawn Bat");
                            var newEnemy = Instantiate(bat, pos, rot);
                            newEnemy.transform.parent = transform;
                            enemies.Add(newEnemy);
                            aliveEnemies.Add(newEnemy);
                        }
                        // 30% likely to spawn yellow skeleton:
                        else if (numb > 55 && numb <= 85)
                        {
                            Debug.Log("Spawn YellowSkeleton");
                            var newEnemy = Instantiate(yellowSkeleton, pos, rot);
                            newEnemy.transform.parent = transform;
                            enemies.Add(newEnemy);
                            aliveEnemies.Add(newEnemy);
                        }
                        // 5% likely to spawn giant skeleton:
                        else if (numb > 85 && numb <= 90)
                        {
                            Debug.Log("Spawn Giant Skeleton");
                            var newEnemy = Instantiate(giantSkeleton, pos, rot);
                            newEnemy.transform.parent = transform;
                            enemies.Add(newEnemy);
                            aliveEnemies.Add(newEnemy);
                        }
                        // 10% likely to spawn giant bat:
                        else if (numb > 90 && numb <= 100)
                        {
                            Debug.Log("Spawn Giant Bat");
                            var newEnemy = Instantiate(giantBat, pos, rot);
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
