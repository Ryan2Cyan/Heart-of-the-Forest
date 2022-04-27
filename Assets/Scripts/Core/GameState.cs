using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Core;
using Entities;
using TMPro;

public class GameState : MonoBehaviour
{

    private WaveSpawner waveSpawner;
    public bool isDay { get; private set; }
    public int currentWave;
    public int currentTime;
    public int currentDay;
    private TMP_Text waveCountUI;
    private TMP_Text dayCountUI;

    [SerializeField] private Light directionalLight;
    [SerializeField, Range(0, 24)] private float timeOfDay;
    [SerializeField] private Player player;

    public AudioSource src;
    public AudioSource daySrc;
    public AudioSource nightSrc;
    public AudioClip nightTransitionSound;
    public AudioClip dayTransitionSound;



    private void Start()
    {
        isDay = true;
        currentWave = 0;
        currentDay = 1;
        waveCountUI = GameObject.Find("WaveCount").GetComponent<TMP_Text>();
        waveCountUI.text = "WaveCount: " + currentWave;
        waveSpawner = transform.GetChild(0).GetComponent<WaveSpawner>();
        // LoadPlayers(GameObject.Find("PlayerSpawn").transform);

        daySrc.Play();
    }
    
    private void Update()
    {
        if (waveSpawner)
        {
            if (Application.isPlaying)
                UpdateLighting(isDay, ref directionalLight);

            if (waveSpawner.aliveEnemies.Count <= 0)
            {
                if (!isDay)
                {
                    ToggleDay();
                    currentDay += 1;
                }
            }
        }
    }
    
    public void ToggleDay()
    {
        isDay = !isDay;
        if (isDay)
		{
            src.PlayOneShot(dayTransitionSound);
            nightSrc.Pause();
            daySrc.Play();
		}
        else
		{
            src.PlayOneShot(nightTransitionSound);
            daySrc.Stop();
            nightSrc.PlayDelayed(2);
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
        float timeArg = (isDayArg ? 13.0f : 0.0f) / 24f;
        
        // Rotate the directional light:
        lightArg.transform.localRotation = Quaternion.Euler
            (new Vector3((timeArg * 360f) - 90f, 170f, 0));
    }

    public void UpdateWaveCount()
    {
        currentWave += 1;
        waveCountUI.text = "WaveCount: " + currentWave;
    }
}