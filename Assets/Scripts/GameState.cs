using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // this is used for finding the last element of a List

public class GameState : MonoBehaviour
{
    [SerializeField] private List<Enemy> listOfEnemies;
    [SerializeField] private List<Vector3> enemyPositions;
    [SerializeField] private Vector3[] shopPositions;
    [SerializeField] private int currentWave;
    [SerializeField] private int currentTime;

    [SerializeField] private Light directionalLight;
    [SerializeField, Range(0, 24)] private float timeOfDay;

    [SerializeField] private Player player;
    [SerializeField] private Transform playerSpawn;
    [SerializeField] private Enemy enemy;
    [SerializeField] private NPC npc;


    // Start is called before the first frame update
    void Start()
    {

        // Very first thing is to load the players in
        LoadPlayers();

        // Second spawn NPCs
        SpawnNPCs();

        // Start game
        StartGame();

        timeOfDay = 10f;
    }

    // Update is called once per frame
    void Update()
    {

        if(Application.isPlaying)
        {
            
            timeOfDay += Time.deltaTime;
            timeOfDay %= 24; // Clamp between 0-24
            UpdateLighting(timeOfDay/24f);
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

    public void StartGame()
    {
        // Test to display amount of enemies and their positions
        Debug.Log("Enemy position count: " + listOfEnemies.Count);
        for (int i = 0; i < listOfEnemies.Count; i++)
        {
            Debug.Log("Enemy position 1: " + enemyPositions[i]);
        }
    }

    // This function just checks to see if there's a directional light
    // If there is no light, it will find the first
    // directional light and use that
    private void OnValidate()
    {
        if (directionalLight != null)
        {
            return;
        }

        if(RenderSettings.sun!=null)
        {
            directionalLight = RenderSettings.sun;
        }
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach(Light light in lights)
            {
                if(light.type == LightType.Directional)
                {
                    directionalLight = light;
                    return;
                }
            }
        }
    }

    private void UpdateLighting(float timePercent)
    {
        if (directionalLight != null)
        {
            {
                // Rotate light at the % of the current time of day
                directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
            }
        }
    }

    public float GetTimeOfDay()
    {
        return timeOfDay;
    }

    public void AddEnemy(Enemy newEnemy)
    {
        // Add new enemy to the list of enemies
        listOfEnemies.Add(newEnemy);
        // Using the list of enemies, add their position to the enemy position list
        enemyPositions.Add(newEnemy.transform.position);

        //Debug.Log(enemyPositions.Last());
    }

    public void RemoveEnemy(Enemy newEnemy)
    {

    }

}