using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public AudioClip uiClick;
    public AudioSource src;


    public void playClick()
    {
        src.PlayOneShot(uiClick);
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}
