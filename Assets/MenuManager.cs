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



    void CheckGameObject()
    {
        if (EndlessBG.activeSelf)
        {
            // do something, if it is  active...
            CanPlaySelectedMode = true;
        }

        else
        {
            // do something, if it is active...
            CanPlaySelectedMode = false;
        }
    }

    public void PlayLevel()
    {
        
        Debug.Log(CanPlaySelectedMode);
        if (CanPlaySelectedMode == true)
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

