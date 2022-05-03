using UnityEngine;

public class FadeInAnimation : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_CanvasGroup;
    public bool FinishedFading;

    private void Start()
    {
        FinishedFading = false;
    }
    public void Update()
    {
        if (FinishedFading == false)
            if (m_CanvasGroup.alpha <= 0)
            {
                m_CanvasGroup.alpha += Time.deltaTime;
            }

            if (m_CanvasGroup.alpha == 1)
            {
                FinishedFading = true;
            }

    }
}

