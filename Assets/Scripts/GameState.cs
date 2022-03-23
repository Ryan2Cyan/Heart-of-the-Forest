using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TDG.Entity;

public class GameState : MonoBehaviour
{
    [SerializeField] private List<Enemy> amountOfEnemies;
    [SerializeField] private Vector3[] enemyPositions;
    [SerializeField] private Vector3[] shopPositions;
    [SerializeField] private int currentWave;
    [SerializeField] private int currentTime;


    [SerializeField] private GameObject directionalLight;
    [SerializeField] private GameObject player;
    [SerializeField] private Enemy enemy;
    [SerializeField] private Transform playerSpawn;

    // Start is called before the first frame update
    void Start()
    {
        // Very first thing is to load the players in
        LoadPlayers();

        // Spawn enemies - probably to be changed later
        SpawnEnemies();

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
        // Temporary Day/Night cycle thing, update later
        directionalLight.transform.Rotate(new Vector3(0.1f, 0.0f, 0.0f));
    }

    public void LoadPlayers()
    {
        // Instantiate a new player gameobject at the transform-position of the playerspawn gameobject
        Instantiate(player, new Vector3(playerSpawn.position.x, playerSpawn.position.y, playerSpawn.position.z), Quaternion.identity);
    }

    public void SpawnEnemies()
    {
        for (int i = 0; i < 3; i++)
        {
            Instantiate(enemy.gameObject, new Vector3(playerSpawn.position.x + Random.Range(2.0f, 10.0f), playerSpawn.position.y, playerSpawn.position.z + Random.Range(2.0f, 10.0f)), Quaternion.identity);
            amountOfEnemies.Add(enemy);
        }
    }

    public void StartGame()
    {

    }


    void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 50, 100, 50), "Instantiate!"))
        {
            Instantiate(enemy, new Vector3(playerSpawn.position.x + Random.Range(2.0f, 10.0f), playerSpawn.position.y, playerSpawn.position.z + Random.Range(2.0f, 10.0f)), Quaternion.identity);
        }
    }
}