using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Entities;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameState : MonoBehaviour
{
    
    public bool isDay { get; private set; }
    
    [SerializeField] private List<Enemy> listOfEnemies;
    [SerializeField] private int currentWave;
    [SerializeField] private int currentTime;
    
    [SerializeField] private Light directionalLight;
    [SerializeField, Range(0, 24)] private float timeOfDay;

    [SerializeField] private Player player;
    



    private void Start()
    {
        isDay = true;

        LoadPlayers(player.transform);
    }


    private void Update()
    {
        if (Application.isPlaying)
        {
            UpdateLighting(isDay, ref directionalLight);
        }
    }

    public void AddEnemy(Enemy newEnemy)
    {
        listOfEnemies.Add(newEnemy);
    }
    
    public void ToggleDay()
    {
        isDay = !isDay;
    }
    
    private static void LoadPlayers(Transform arg)
    {
        var position = arg.position;
        Debug.Log("Player Spawned.");
        Instantiate(arg.gameObject, new Vector3(position.x, position.y, position.z), Quaternion.identity);
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