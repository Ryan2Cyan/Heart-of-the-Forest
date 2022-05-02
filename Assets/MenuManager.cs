using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MenuManager : MonoBehaviour
{
    public AudioClip uiClick;
    public AudioSource src;

    public GameObject EndlessBG;
    public GameObject StoryBG;
    public GameObject LoadingScreen;

    public bool CanPlaySelectedMode;
    public bool LevelAlreadyLoading;

    private void Awake()
    {
        CanPlaySelectedMode = false;
        LevelAlreadyLoading = false;
    }

    public void SetSelectedMode()
    {
        CanPlaySelectedMode = true;
    }
    public void LoadingLevel()
    {
        LevelAlreadyLoading = true;
    }

    public void PlayLevel()
    {
        Debug.Log(CanPlaySelectedMode);
        if (CanPlaySelectedMode == true)
            if (LevelAlreadyLoading == true)
            {
                LoadingScreen.SetActive(true);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }

    }
    public void playClick()
    {
        src.PlayOneShot(uiClick);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}

