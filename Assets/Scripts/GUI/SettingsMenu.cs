using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public bool isEnabled;
    [SerializeField] private Slider audioSlider;
    [SerializeField] private TMP_Dropdown dropdownQuality;
    [SerializeField] private Toggle fullscreenCheckbox;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private GameObject settingsCanvas;
    [SerializeField] private GameObject controlsCanvas;
    public AudioSource src;
    public AudioClip uiClick;
    public AudioClip uiHover;


    Resolution[] resolutions;

    private void Start()
    {
        // Set resolutions array to be what Unity provides 
        resolutions = Screen.resolutions;

        // Clear out any options in the dropdown
        resolutionDropdown.ClearOptions();

        // Create a list of strings which will be the options
        List<string> options = new List<string>();

        // Loop through each element in the resolutions array
        // For each element, create a formatted string that displays res
        // And then adds it to the options list
        int currentResolutionIndex = PlayerPrefs.GetInt("resolution", 0);
        for(int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height + " " + resolutions[i].refreshRate + "hz";
            options.Add(option);

            if(resolutions[i].width == Screen.width &&
                resolutions[i].height == Screen.height)
            {
                PlayerPrefs.SetInt("resolution", i);
            }
        }

        // Once loop is done, add options list to the resolution dropdown
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Get playerpref volume and set slider val to it
        float volume = PlayerPrefs.GetFloat("volume", 0.35f);
        audioSlider.value = volume;

        // Get the saved quality and update dropdown
        int quality = PlayerPrefs.GetInt("quality", 5);
        dropdownQuality.value = quality;
        dropdownQuality.RefreshShownValue();

        // Check fullscreen status and apply it to existing checkbox | 0 is false, 1 is true
        int fullscreen = PlayerPrefs.GetInt("fullscreen", 0);
        if(fullscreen == 0)
        {
            fullscreenCheckbox.isOn = false;
        }    
        else
        {
            fullscreenCheckbox.isOn = true;
        }
    }

    // Controlled by slider - Sets mixer called "volume" to input slider volume
    public void SetVolume(float volume)
    {
        // Update AudioMixer
        audioMixer.SetFloat("volume", Mathf.Log10(volume) * 20);

        // Update PlayerPrefs "volume"
        PlayerPrefs.SetFloat("volume", volume);

        // Save PlayerPrefs
        PlayerPrefs.Save();
    }

    public void SetQuality(int qualityIndex)
    {
        // Update quality level
        QualitySettings.SetQualityLevel(qualityIndex);

        // Update PlayerPrefs quality
        PlayerPrefs.SetInt("quality", qualityIndex);

        // Save PlayerPrefs
        PlayerPrefs.Save();
    }

    public void SetFullScreen (bool isFullscreen)
    {
        // Update fullscreen
        Screen.fullScreen = isFullscreen;

        // Convertion of bool to int
        int playerPrefInt = isFullscreen ? 1 : 0;

        // Update PlayerPrefs fullscreen
        PlayerPrefs.SetInt("fullscreen", playerPrefInt);

        // Save PlayerPrefs
        PlayerPrefs.Save();
    }

    public void SetResolution(int resolutionIndex)
    {
        // Select resolution based on input
        Resolution resolution = resolutions[resolutionIndex];

        // Update PlayerPrefs resolution to input
        PlayerPrefs.SetInt("resolution", resolutionIndex);

        // Set screen resolution
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        // Save PlayerPrefs
        PlayerPrefs.Save();
    }

    // Switches between controls and settings
    public void SwitchSetting(int setting)
    {
        if(setting == 0)
        {
            settingsCanvas.SetActive(true);
            controlsCanvas.SetActive(false);
        }
        if (setting == 1)
        {
            settingsCanvas.SetActive(false);
            controlsCanvas.SetActive(true);
        }
        if(setting == 2)
        {
            settingsCanvas.SetActive(false);
            controlsCanvas.SetActive(false);
        }
    }

    public void playClick()
	{
        src.PlayOneShot(uiClick);
	}

    public void playHover()
	{
        src.PlayOneShot(uiHover);
	}
}
