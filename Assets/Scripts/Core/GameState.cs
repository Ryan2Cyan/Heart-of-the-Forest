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
        [SerializeField] private GameObject coreGameOverScreen;
        [SerializeField] private GameObject waveCompleteScreen;
        [SerializeField] private GameObject lightingManager;
        private bool waitingForDay;
        private float dayLimitedTimer;
        public float maximumDayTime { get; private set; }
        public GameObject dayTimerIcon { get; private set; }
        private TMP_Text dayTimerText;

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
            dayTimerIcon = GameObject.Find("DayTimerIcon");
            dayTimerText = GameObject.Find("DayTimer").GetComponent<TextMeshProUGUI>();

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
            CheckCurrentWave();
            CheckDayTimer(ref dayLimitedTimer, ref dayTimerText, isDay, maximumDayTime);
            FadeText(ref waveCompleteText, ref waitingForDay, 2f);
        }

        // Check if the current wave has ended:
        private void CheckCurrentWave()
        {
            // If there are no enemies left, end the wave:
            if (waveSpawner.aliveEnemies.Count <= 0 &&
                !isDay &&
                !waitingForDay)
            {
                StartCoroutine(FinishWave());
                waitingForDay = true;
            }
        }

        // Execute at the end of a wave:
        private IEnumerator FinishWave()
        {
            waveCompleteScreen.SetActive(true);
            yield return new WaitForSeconds(2f);
            dayTimerIcon.SetActive(true);
            waveCompleteScreen.SetActive(false);
            ToggleDay();
            waitingForDay = false;
            dayLimitedTimer = 0;
        }

        // If timer reaches the max threshold, automatically transition to next wave:
        private void CheckDayTimer(ref float dayTimer, ref TMP_Text timerTxt, bool isDayArg, float maxVal)
        {
            if (dayTimer >= maxVal)
            {
                GameObject.Find("Core").GetComponent<Shopkeep>().StartNextWave();
            }
            
            // Increment and display timer during day:
            if (isDayArg)
            {
                dayTimer += 1 * Time.deltaTime;   
                timerTxt.text = (maxVal - Mathf.Round(dayTimer)).ToString(CultureInfo.InvariantCulture);
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