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
    private float dayLimitedTimer;
    public float maximumDayTime;
   

    [SerializeField] private Light directionalLight;
    [SerializeField, Range(0, 24)] private float timeOfDay;
    [SerializeField] private Player player;
    [SerializeField] private GameObject coreGameOverScreen;
    [SerializeField] private GameObject waveCompleteScreen;
    [SerializeField] private TMP_Text dayTimerText;
    private TMP_Text waveCompleteText;
    [SerializeField] private GameObject lightingManager;

    public AudioSource src;
    public AudioSource daySrc;
    public AudioSource nightSrc;
    public AudioClip nightTransitionSound;
    public AudioClip dayTransitionSound;

	private void Awake()
	{
        // This fixed the build error
        waveCompleteText = waveCompleteScreen.GetComponent<TMP_Text>();
    }

	private void Start()
    {
        dayLimitedTimer = 0f;
        maximumDayTime = 60f;
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
        if (isDay)
        {
            dayLimitedTimer += 1 * Time.deltaTime;
            dayTimerText.transform.GetChild(0).GetComponent<TMP_Text>().text = (maximumDayTime - Mathf.Round(dayLimitedTimer)).ToString() + "s";
        }

        if (waveSpawner)
        {
            if (Application.isPlaying)
                //UpdateLighting(isDay, ref directionalLight);

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
        if(waitingForDay)
        {
            waveCompleteText.color = Color.Lerp(waveCompleteText.color, new Color(waveCompleteText.color.r, waveCompleteText.color.g, waveCompleteText.color.b, 1), 2 * Time.deltaTime);
        }
        if(!waitingForDay && waveCompleteText.color.a != 0)
        {
            waveCompleteText.color = new Color(waveCompleteText.color.r, waveCompleteText.color.g, waveCompleteText.color.b, 0);
        }

        // If time exceeded 60 seconds, start the next wave automatically
        if (dayLimitedTimer >= maximumDayTime)
        {
            GameObject.Find("Core").GetComponent<Shopkeep>().StartNextWave();
        }
    }

    IEnumerator FinishWave()
    {
        waveCompleteScreen.SetActive(true);
        yield return new WaitForSeconds(2f);
        waveCompleteScreen.SetActive(false);
        ToggleDay();
        waitingForDay = false;
        dayLimitedTimer = 0f;
    }
    
    public void ToggleDay()
    {
        // Change to day
        if (!isDay)
		{
            isDay = true;
            lightingManager.GetComponent<DayNightLighting>().ToDay();
            src.PlayOneShot(dayTransitionSound);
            nightSrc.Pause();
            daySrc.Play();
            waveIcon.sprite = Resources.Load<Sprite>("Sprites/wave-icon");
            dayTimerText.gameObject.SetActive(true);
        }
        // Change to night
        else
		{
            isDay = false;
            lightingManager.GetComponent<DayNightLighting>().ToNight();
            src.PlayOneShot(nightTransitionSound);
            daySrc.Stop();
            nightSrc.PlayDelayed(2);
            waveIcon.sprite = Resources.Load<Sprite>("Sprites/night-wave-icon");
            dayTimerText.gameObject.SetActive(false);

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