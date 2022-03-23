using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    [SerializeField] private List<GameObject> listOfEnemies;
    [SerializeField] private List<Vector3> enemyPositions;
    [SerializeField] private Vector3[] shopPositions;
    [SerializeField] private int currentWave;
    [SerializeField] private int currentTime;

    [SerializeField] private float dayTime = 20.0f;
    [SerializeField] private float nightTime = 50.0f;



    [SerializeField] private GameObject directionalLight;
    [SerializeField] private Player player;
    [SerializeField] private Enemy enemy;
    [SerializeField] private NPC npc;
    [SerializeField] private Transform playerSpawn;

    // Start is called before the first frame update
    void Start()
    {

        // Very first thing is to load the players in
        LoadPlayers();

        // Second spawn NPCs
        SpawnNPCs();

        // Spawn enemies - probably to be changed later
        SpawnEnemies();

        // Start game
        StartGame();


        // Find the directional light gameobject which we use for the sun and cache it
        directionalLight = GameObject.Find("Directional Light");
        if (!directionalLight)
        {
            Debug.Log("Couldn't find light in scene");
        }
    }

    // Update is called once per frame
    void Update()
    {

        if(directionalLight.transform.rotation.x >= 0.98f)
        {
            Debug.Log("Nighttime reached");
        }
        else
        {
            directionalLight.transform.Rotate(new Vector3(0.1f, 0.0f, 0.0f));
            //Debug.Log(directionalLight.transform.rotation.x);
        }
    }

    public void LoadPlayers()
    {

        // Instantiate a new player gameobject at the transform-position of the playerspawn gameobject
        Instantiate(player.gameObject, new Vector3(playerSpawn.position.x, playerSpawn.position.y, playerSpawn.position.z), Quaternion.identity);
    }

    public void SpawnNPCs()
    {
        // Instantiate a new player gameobject at the transform-position of the playerspawn gameobject
        //Instantiate(npc.gameObject, new Vector3(playerSpawn.position.x, playerSpawn.position.y, playerSpawn.position.z), Quaternion.identity);
    }

    public void SpawnEnemies()
    {
        // Spawn three enemies for a test
        for (int i = 0; i < 3; i++)
        {
            // Create a new enemy as a gameobject and instantiate it at a random position using the playerSpawn gameobject
            GameObject newEnemy = Instantiate(enemy.gameObject, new Vector3(playerSpawn.position.x + Random.Range(2.0f, 10.0f), playerSpawn.position.y, playerSpawn.position.z + Random.Range(2.0f, 10.0f)), Quaternion.identity);
            // Add new enemy to the list of enemies
            listOfEnemies.Add(newEnemy);
            // Using the list of enemies, add their position to the enemy position list
            enemyPositions.Add(listOfEnemies[i].transform.position);
        }
    }

    public void StartGame()
    {
        // Test to display amount of enemies and their positions
        Debug.Log("Enemy position count: " + listOfEnemies.Count);
        for (int i = 0; i < listOfEnemies.Count; i++)
        {
            Debug.Log("Enemy position 1: " + enemyPositions[i]);
        }
    }


    void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 50, 100, 50), "Instantiate!"))
        {
            Instantiate(enemy.gameObject, new Vector3(playerSpawn.position.x + Random.Range(2.0f, 10.0f), playerSpawn.position.y, playerSpawn.position.z + Random.Range(2.0f, 10.0f)), Quaternion.identity);
        }
    }
}