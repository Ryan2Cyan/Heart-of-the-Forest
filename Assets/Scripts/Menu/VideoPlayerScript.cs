using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Menu
{
    public class VideoPlayerScript : MonoBehaviour
    {
        [SerializeField] private RawImage rawVideo;
        [SerializeField] private AudioSource menuSrc;
        [SerializeField] private TextMeshProUGUI inputText;
        private VideoPlayer videoPlayer;
        private bool isTransparent;
        private float afkTimer;

        // Start is called before the first frame update
        private void Awake()
        {
            videoPlayer = GetComponent<VideoPlayer>();
            afkTimer = 30f;
            rawVideo.color = new Color(1f, 1f, 1f, 0f);
            rawVideo.gameObject.SetActive(false);
            videoPlayer.SetDirectAudioVolume(0, 0);
        }

        // Update is called once per frame
        private void Update()
        {
            // Increment timer:
            if(afkTimer >= 0)
            {
                afkTimer -= 1 * Time.deltaTime;
            }

            // If player is afk for too long, start playing video:
            if(afkTimer <= 0)
            {
                if(!rawVideo.gameObject.activeInHierarchy)
                    rawVideo.gameObject.SetActive(true);
                if(rawVideo.color.a <= 1f)
                    rawVideo.color = Color.Lerp(rawVideo.color, new Color(1, 1, 1, 1), 2 * Time.deltaTime);
                menuSrc.volume = 0;
                videoPlayer.Play();
                videoPlayer.SetDirectAudioVolume(0, 1);
                rawVideo.gameObject.SetActive(true);
            }

            // If player inputs any key or clicks, change to menu:
            if (Input.anyKey)
            {
                videoPlayer.frame = 0;
                videoPlayer.SetDirectAudioVolume(0, 0);
                if(rawVideo.gameObject.activeInHierarchy)
                    rawVideo.gameObject.SetActive(false);
                if(rawVideo.color.a > 0.001f)
                    rawVideo.color = new Color(1f, 1f, 1f, 0f);
                afkTimer = 25f;
                menuSrc.volume = 1;
            }

            // Check if player is not afk via mouse movement and key input:
            if(afkTimer >= 0)
            {
                if(Input.GetMouseButtonDown(0) || Input.anyKey)
                {
                    videoPlayer.SetDirectAudioVolume(0, 0);
                    videoPlayer.Stop();
                    rawVideo.gameObject.SetActive(false);
                    afkTimer = 25f;
                }
            }
            
            // Fade the input text in and out:
            if (inputText.color.a > 0.99f)
                isTransparent = true;
            if (inputText.color.a <= 0.001f)
                isTransparent = false;
            if (isTransparent)
                inputText.color = Color.Lerp(inputText.color, new Color(0, 0, 0, 0), 3 * Time.deltaTime);
            if (!isTransparent)
                inputText.color = Color.Lerp(inputText.color, new Color(1, 1, 1, 1), 3 * Time.deltaTime);
        }
    }
}
