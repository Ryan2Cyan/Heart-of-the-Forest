using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    [SerializeField] private Slider audioSlider;

    private void Start()
    {
        // Get the saved sound volume, default = 0f
        float volume = PlayerPrefs.GetFloat("volume", 0f);

        // Set the audio slider initial value to be player pref value
        audioSlider.value = volume;
    }

    // Controlled by slider - Sets mixer called "volume" to input slider volume
    public void SetVolume(float volume)
    {
        // Update AudioMixer
        audioMixer.SetFloat("volume", volume);

        // Update PlayerPrefs "volume"
        PlayerPrefs.SetFloat("volume", volume);

        // Save PlayerPrefs
        PlayerPrefs.Save();
    }
}
