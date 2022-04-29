using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private MusicPlayer musicPlayer;
    public GameObject loadingScreen;
    public Slider slider;
    public Text progressText;

    private void Start()
    {
        musicPlayer = GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>();
        Time.timeScale = 1;
    }

    // Loads the next scene
    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Loads a specific scene via build index number
    public void LoadScene(int sceneNumber)
    {
        // Destroy the menu music player before loading a new scene

        StartCoroutine(LoadAsynchronously(sceneNumber));
    }

    IEnumerator LoadAsynchronously(int sceneNumber)
	{
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneNumber, LoadSceneMode.Single);

        loadingScreen.SetActive(true);

        while (!operation.isDone)
		{
            float progress = Mathf.Clamp01(operation.progress / .9f);

            slider.value = progress;
            progressText.text = (Mathf.Round(progress * 100f) + "%");
            Debug.Log(progress);

            yield return null;
		}
        if (GameObject.Find("MusicPlayer") != null)
        {
            Destroy(GameObject.Find("MusicPlayer"));
        }
    }

    // Loads the settings menu specifically
    public void LoadSettingsMenu()
    {
        DontDestroyOnLoad(musicPlayer);
        SceneManager.LoadScene("SettingsMenu");
    }

    // Loads the first scene (menu)
    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }

    // Quits the game
    public void QuitGame()
    {
        Application.Quit();
    }
    
}
