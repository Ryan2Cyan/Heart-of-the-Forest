using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    private void Start()
    {
        Time.timeScale = 1;
        //DontDestroyOnLoad(this.gameObject);
    }

    // Loads the next scene
    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Loads a specific scene via build index number
    public void LoadScene(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
    }

    // Loads the settings menu specifically
    public void LoadSettingsMenu()
    {
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
