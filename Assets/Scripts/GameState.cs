using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameState : MonoBehaviour
{
    [SerializeField] private List<Enemy> listOfEnemies;
    [SerializeField] private List<Vector3> enemyPositions;
    [SerializeField] private List<Vector3> shopPositions;
    [SerializeField] private int currentWave;
    [SerializeField] private int currentTime;
    [SerializeField] private int totalEnemies;
    
    [SerializeField] private Light directionalLight;

    [SerializeField, Range(0, 24)] private float timeOfDay;
    [SerializeField] private bool isDay;

    [SerializeField] private Player player;
    [SerializeField] private Enemy enemy;
    [SerializeField] private NPC npc;
    [SerializeField] private Transform playerSpawn;

   
    private void Start()
    {
        totalEnemies = 3;
        isDay = false;

        LoadPlayers(player.transform);
        SpawnEnemies(totalEnemies, player.transform, ref listOfEnemies);
        
        StartGame();
    }


    private void Update()
    {
        if (Application.isPlaying)
        {
            UpdateLighting(isDay, ref directionalLight);
        }
    }

    
    private static void LoadPlayers(Transform arg)
    {
        var position = arg.position;
        Instantiate(arg.gameObject, new Vector3(position.x, position.y, position.z), Quaternion.identity);
    }

    private void SpawnNpcs(Transform arg)
    {
        var position = arg.position;
        Instantiate(npc.gameObject, new Vector3(position.x, position.y, position.z), Quaternion.identity);
    }

    // Spawn specified amount of enemies, add to enemies list:
    private void SpawnEnemies(int numOfEnemies, Transform spawnPoint, ref List<Enemy> enemyList)
    {
        for (var i = 0; i < numOfEnemies; i++)
        {
            // TODO: Implement enemy spawning system (potentially at defined positions or in a circle around the map:
            var position = spawnPoint.position;
            var newEnemy = Instantiate(enemy, new Vector3(position.x + Random.Range(2.0f, 10.0f), 
                position.y, position.z + Random.Range(2.0f, 10.0f)), Quaternion.identity);
          
            enemyList.Add(newEnemy);
            enemyPositions.Add(enemyList[i].transform.position);
        }
    }
    
    private void StartGame()
    {
        Debug.Log("Enemy position count: " + listOfEnemies.Count);
        for (var i = 0; i < listOfEnemies.Count; i++)
        {
            Debug.Log("Enemy position" + i + ": " + enemyPositions[i]);
        }
    }


    // Currently redundant: just an example button for instantiating new enemies in random positions
    void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 50, 100, 50), "Instantiate!"))
        {
            var position = playerSpawn.position;
            Instantiate(enemy.gameObject, new Vector3(position.x + Random.Range(2.0f, 10.0f), 
                position.y, position.z + Random.Range(2.0f, 10.0f)), Quaternion.identity);
        }
    }
    
    
    // Check if light is valid - if true: set time based on bool:
    private static void UpdateLighting(bool isDayArg, ref Light lightArg)
    {
        if (!lightArg)
        {
            lightArg = RenderSettings.sun;
            if(!lightArg) throw new Exception("Could not instantiate directional Light");
        }
        // Change time of day based on bool:
        var timeArg = (isDayArg ? 13.0f : 0.0f) / 24f;
        
        // Rotate the directional light:
        lightArg.transform.localRotation = Quaternion.Euler
            (new Vector3((timeArg * 360f) - 90f, 170f, 0));
         
    }
}