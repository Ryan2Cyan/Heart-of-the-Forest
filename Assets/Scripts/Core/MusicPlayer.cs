using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    // If there's more than one music player, kill it
    // Otherwise, the music player will not be destroyed between scenes
    void Start()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Music");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }
    }
}
