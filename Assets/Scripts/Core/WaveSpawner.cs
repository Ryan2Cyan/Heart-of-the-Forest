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
        private float radius;
        private float angle;
        private GameObject spawnPosition;
        [SerializeField] private int enemiesToSpawn;
        private bool spawned;
    


        private void Start()
        {
            gameStateScript = FindObjectOfType<GameState>();

            radius = 50f;
            angle = 360f / 5f;

            centreOfSpawn = GameObject.Find("CentreOfSpawn");

            spawnPosition = centreOfSpawn;
             
            enemiesToSpawn =  gameStateScript.currentWave + gameStateScript.currentWave + 1;
        }
    
        private void Update()
        {
            // Check if the time of day is night
            if(gameStateScript.isDay == false)
            {
                enemiesToSpawn = gameStateScript.currentWave + gameStateScript.currentWave + 100;

                if (!spawned)
                {
                    for(var i = 0; i < enemiesToSpawn; i++)
                    {
                        //// Get a random spawn location
                        //var spawnLocationChoice = Random.Range(0, spawnPositions.Length);

                        //// Instantiate a new enemy at the random spawn location chosen, within a range of 1-3f
                        //var newEnemy = Instantiate(enemy, new Vector3(
                        //        spawnPositions[spawnLocationChoice].position.x + Random.Range(1.0f, 3.0f),
                        //        spawnPositions[spawnLocationChoice].position.y,
                        //        spawnPositions[spawnLocationChoice].position.z + Random.Range(1.0f, 3.0f)),
                        //    Quaternion.identity);

                        spawnPosition.transform.Rotate(0,angle,0);

                        Enemy newEnemy = Instantiate(enemy, new Vector3(
                              spawnPosition.transform.position.x + radius,
                               spawnPosition.transform.position.y,
                               spawnPosition.transform.position.z + radius),
                           Quaternion.identity);

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
    }
}
