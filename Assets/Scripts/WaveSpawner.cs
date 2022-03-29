using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{

    // Reference to the gamestate for access to the game's state
    GameState gameStateScript;
    [SerializeField] Enemy enemy;
    [SerializeField] Transform[] spawnPositions;
    [SerializeField] private int amountOfEnemiesToSpawn;
    bool spawned = false;

    // Start is called before the first frame update
    void Start()
    {
        gameStateScript = FindObjectOfType<GameState>();

        amountOfEnemiesToSpawn = 20;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the time of day is night
        if(gameStateScript.isDay)
        {
           if(!spawned)
           {
                for(int i = 0; i < amountOfEnemiesToSpawn; i++)
                {
                    // Get a random spawn location
                    int spawnLocationChoice = Random.Range(0, spawnPositions.Length);

                    // Instantiate a new enemy at the random spawn location chosen, within a range of 1-3f
                    Enemy newEnemy = Instantiate(enemy, new Vector3(
                        spawnPositions[spawnLocationChoice].position.x + Random.Range(1.0f, 3.0f),
                        spawnPositions[spawnLocationChoice].position.y,
                        spawnPositions[spawnLocationChoice].position.z + Random.Range(1.0f, 3.0f)),
                        Quaternion.identity);
                    // Add the enemy to the gamestate so it can track the enemy and its position
                    gameStateScript.AddEnemy(newEnemy);
                }

                spawned = true;
           }
        }
    }
}
