using System.Collections;
using System.Globalization;
using Entities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

namespace Core
{
    public class GameState : MonoBehaviour
    {

        // General
        private WaveSpawner waveSpawner;
        public bool isDay { get; private set; }
        public int currentWave { get; private set; }
    
        // UI:
        private TMP_Text waveCountUI;
        private TMP_Text waveCompleteText;
        private Image waveIcon;
        private bool waitingForDay;
        private float dayLimitedTimer;
        public float maximumDayTime { get; private set; }
    
        [SerializeField] private GameObject coreGameOverScreen;
        [SerializeField] private GameObject waveCompleteScreen;
        [SerializeField] private TMP_Text dayTimerText;
        [SerializeField] private GameObject lightingManager;

        // Audio:
        private AudioSource src;
        private AudioSource daySrc;
        private AudioSource nightSrc;
        private AudioClip nightTransitionSound;
        private AudioClip dayTransitionSound;

        private void Start()
        {
            // Get components:
            waveCountUI = GameObject.Find("WaveCount").transform.GetChild(0).GetComponent<TMP_Text>();
            waveIcon = GameObject.Find("WaveCount").transform.GetChild(1).GetComponent<Image>();
            waveIcon.sprite = Resources.Load<Sprite>("Sprites/wave-icon");
            waveSpawner = transform.GetChild(0).GetComponent<WaveSpawner>();
            src = transform.GetComponent<AudioSource>();
            daySrc = GameObject.Find("MusicPlayer").transform.GetChild(0).GetComponent<AudioSource>();
            nightSrc = GameObject.Find("MusicPlayer").transform.GetChild(1).GetComponent<AudioSource>();
            waveCompleteText = waveCompleteScreen.GetComponent<TMP_Text>();

            // Set values:
            nightTransitionSound = Resources.Load<AudioClip>("Sounds/SFX/AlexSFX/night-transition");
            dayTransitionSound = Resources.Load<AudioClip>("Sounds/SFX/AlexSFX/day-transition");
            dayLimitedTimer = 0f;
            maximumDayTime = 60f;
            isDay = true;
            waitingForDay = false;
            currentWave = 0;
            waveCountUI.text = currentWave.ToString();
            daySrc.Play();
        }
    
        private void Update()
        {
            // Increment daytime counter (during day):
            if (isDay)
            {
                dayLimitedTimer += 1 * Time.deltaTime;
                dayTimerText.transform.GetChild(0).GetComponent<TMP_Text>().text = (maximumDayTime - 
                    Mathf.Round(dayLimitedTimer)).ToString(CultureInfo.InvariantCulture);
            }

            // If there are no enemies left, end the wave:
            if (waveSpawner && 
                Application.isPlaying && 
                waveSpawner.aliveEnemies.Count <= 0 &&
                !isDay &&
                !waitingForDay)
            {
                StartCoroutine(FinishWave());
                waitingForDay = true;
            }
            
            FadeText(ref waveCompleteText, ref waitingForDay, 2f);
            CheckDayTimer(ref dayLimitedTimer, maximumDayTime);
        }

        private IEnumerator FinishWave()
        {
            waveCompleteScreen.SetActive(true);
            yield return new WaitForSeconds(2f);
            waveCompleteScreen.SetActive(false);
            ToggleDay();
            waitingForDay = false;
            dayLimitedTimer = 0f;
        }

        // If timer reaches the max threshold, automatically transition to next wave:
        private static void CheckDayTimer(ref float dayTimer, float maxVal)
        {
            if (dayTimer >= maxVal)
            {
                GameObject.Find("Core").GetComponent<Shopkeep>().StartNextWave();
            }
        }

        // True: Increment alpha to 1 at specified speed,
        // False: Set objects alpha to 0.
        private static void FadeText(ref TMP_Text textArg, ref bool arg, float fadeSpeed)
        {
            textArg.color = arg switch
            {
                true => Color.Lerp(textArg.color, new Color(textArg.color.r, textArg.color.g, textArg.color.b, 1),
                    fadeSpeed * Time.deltaTime),
                false when textArg.color.a != 0 => new Color(textArg.color.r, textArg.color.g, textArg.color.b, 0),
                _ => textArg.color
            };
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
        
        // Increment current wave:
        public void UpdateWaveCount()
        {
            currentWave += 1;
            waveCountUI.text = currentWave.ToString();
        }
        
        // Game over:
        public void CoreDestroyed()
        {
            coreGameOverScreen.SetActive(true);
            Time.timeScale = 0;
            GameObject.FindWithTag("Player").GetComponent<FirstPersonController>().enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}