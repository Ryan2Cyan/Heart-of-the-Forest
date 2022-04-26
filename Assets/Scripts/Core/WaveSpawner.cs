using Entities;
using UnityEngine;

namespace Core
{
    public class WaveSpawner : MonoBehaviour
    {

        private GameState gameStateScript;
        [SerializeField] private Enemy enemy;
        [SerializeField] private Transform[] spawnPositions;
        [SerializeField] private GameObject centreOfSpawn;
        [SerializeField] private int enemiesToSpawn;
        private bool spawned;



        private void Start()
        {
            gameStateScript = FindObjectOfType<GameState>();

            centreOfSpawn = GameObject.Find("CentreOfSpawn");
        }

        private void Update()
        {
            // Check if the time of day is night
            if (gameStateScript.isDay == false)
            {
                enemiesToSpawn = gameStateScript.currentWave + gameStateScript.currentWave + 1;

                if (!spawned)
                {
                    for (var i = 0; i < enemiesToSpawn; i++)
                    {
                        // Generate enemies randomly on the perimeter of a circle
                        Vector3 pos = RandomCircle(centreOfSpawn.transform.position, 50f);
                        Quaternion rot = Quaternion.FromToRotation(Vector3.forward, centreOfSpawn.transform.position - pos);
                        Enemy newEnemy = Instantiate(enemy, pos, rot);

                        // Add the enemy to the game state so it can track the enemy and its position
                        gameStateScript.AddEnemy(newEnemy);
                    }

                    spawned = true;
                }
            }
            else
            {
                spawned = false;
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
