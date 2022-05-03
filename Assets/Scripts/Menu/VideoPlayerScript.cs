using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayerScript : MonoBehaviour
{
    [SerializeField] private RawImage videoPlayer;
    private float afkTimer;

    // Start is called before the first frame update
    void Start()
    {
        afkTimer = 0f;
        transform.GetComponent<VideoPlayer>().Play();
        videoPlayer.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

        // Increment timer:
        if(afkTimer >= 0)
        {
            afkTimer -= 1 * Time.deltaTime;
        }

        // If player is afk for too long, start playing video:
        if(afkTimer <= 0)
        {
            transform.GetComponent<VideoPlayer>().Play();
            videoPlayer.gameObject.SetActive(true);
        }

        // If player inputs any key or clicks, change to menu:
        if (Input.anyKey)
        {
            videoPlayer.gameObject.SetActive(false);
            afkTimer = 10f;
        }

        // Check if player is not afk via mouse movement and key input:
        if(afkTimer >= 0)
        {
            if(Input.GetAxis("Mouse X") < 0 || Input.GetAxis("Mouse X") > 0 || Input.GetAxis("Mouse Y") > 0 || Input.GetAxis("Mouse Y") < 0 || Input.anyKey)
            {
                videoPlayer.gameObject.SetActive(false);
                afkTimer = 10f;
            }
        }
    }
}
