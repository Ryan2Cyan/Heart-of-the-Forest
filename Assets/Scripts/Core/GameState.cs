using System;
using System.Collections;
using UnityEngine;
using System.Linq;
using Core;
using Entities;
using TMPro;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class GameState : MonoBehaviour
{

    private WaveSpawner waveSpawner;
    public bool isDay { get; private set; }
    public int currentWave;
    public int currentTime;
    private TMP_Text waveCountUI;
    private Image waveIcon;
    private bool waitingForDay;
    private bool changeIcon;
    private TMP_Text dayCountUI;

    [SerializeField] private Light directionalLight;
    [SerializeField, Range(0, 24)] private float timeOfDay;
    [SerializeField] private Player player;
    [SerializeField] private GameObject coreGameOverScreen;

    public AudioSource src;
    public AudioSource daySrc;
    public AudioSource nightSrc;
    public AudioClip nightTransitionSound;
    public AudioClip dayTransitionSound;

    
    private void Start()
    {
        isDay = true;
        waitingForDay = false;
        currentWave = 0;
        waveCountUI = GameObject.Find("WaveCount").transform.GetChild(0).GetComponent<TMP_Text>();
        waveIcon = GameObject.Find("WaveCount").transform.GetChild(1).GetComponent<Image>();
        waveCountUI.text = currentWave.ToString();
        waveIcon.sprite = Resources.Load<Sprite>("Sprites/wave-icon");
        waveSpawner = transform.GetChild(0).GetComponent<WaveSpawner>();
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
                    if(!waitingForDay)
                    {
                        StartCoroutine(FinishWave());
                        waitingForDay = true; 
                    }
                }
            }
        }
    }

    IEnumerator FinishWave()
    {
        yield return new WaitForSeconds(4f);
        ToggleDay();
    }
    
    public void ToggleDay()
    {

        
        if (isDay)
		{
            isDay = !isDay;
            src.PlayOneShot(dayTransitionSound);
            nightSrc.Pause();
            daySrc.Play();
            waveIcon.sprite = Resources.Load<Sprite>("Sprites/wave-icon");
		}
        else
		{
            isDay = true;
            src.PlayOneShot(nightTransitionSound);
            daySrc.Stop();
            nightSrc.PlayDelayed(2);
            waveIcon.sprite = Resources.Load<Sprite>("Sprites/night-wave-icon");
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
        waveCountUI.text = currentWave.ToString();
    }

    public void CoreDestroyed()
    {
        coreGameOverScreen.SetActive(true);

        Time.timeScale = 0;
        player.GetComponent<FirstPersonController>().enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}