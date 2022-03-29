using UnityEngine;

public class GameState : MonoBehaviour
{
    [SerializeField] private List<Enemy> listOfEnemies;
    [SerializeField] private List<Vector3> enemyPositions;
    [SerializeField] private List<Vector3> shopPositions;
    [SerializeField] private int currentWave;
    [SerializeField] private int currentTime;

    [SerializeField] private Light directionalLight;

    [SerializeField, Range(0, 24)] private float timeOfDay;

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
        // Instantiate a new npc gameobject at the transform-position of the playerspawn gameobject
        // Instantiate(npc.gameObject, new Vector3(playerSpawn.position.x, playerSpawn.position.y, playerSpawn.position.z), Quaternion.identity);
    }

    public void SpawnEnemies()
    {
        // Spawn three enemies for a test
        for (int i = 0; i < 3; i++)
        {
            // Create a new enemy as a gameobject and instantiate it at a random position using the playerSpawn gameobject
            Enemy newEnemy = Instantiate(enemy, new Vector3(playerSpawn.position.x + Random.Range(2.0f, 10.0f), playerSpawn.position.y, playerSpawn.position.z + Random.Range(2.0f, 10.0f)), Quaternion.identity);
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


    // Currently redundant: just an example button for instantiating new enemies in random positions
    void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 50, 100, 50), "Instantiate!"))
        {
            Instantiate(enemy.gameObject, new Vector3(playerSpawn.position.x + Random.Range(2.0f, 10.0f), playerSpawn.position.y, playerSpawn.position.z + Random.Range(2.0f, 10.0f)), Quaternion.identity);
        }
    }

    // This function just checks to see if there's a direcitonal light
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
                // Rotate the directional light to simulate the sun revolving
                directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
            }
        }
    }
}