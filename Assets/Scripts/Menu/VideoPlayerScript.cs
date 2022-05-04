using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Menu
{
    public class VideoPlayerScript : MonoBehaviour
    {
        [SerializeField] private RawImage videoPlayer;
        [SerializeField] private AudioSource menuSrc;
        [SerializeField] private TextMeshProUGUI inputText;
        private bool isTransparent;
        private float afkTimer;

        // Start is called before the first frame update
        private void Awake()
        {
            afkTimer = 30f;
            videoPlayer.gameObject.SetActive(false);
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
                menuSrc.volume = 0;
                videoPlayer.gameObject.SetActive(true);
                transform.GetComponent<VideoPlayer>().Play();
                videoPlayer.gameObject.SetActive(true);
            }

            // If player inputs any key or clicks, change to menu:
            if (Input.anyKey)
            {
                videoPlayer.gameObject.SetActive(false);
                afkTimer = 10f;
                menuSrc.volume = 1;
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
