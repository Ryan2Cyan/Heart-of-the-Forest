using UnityEngine;

    public class FadeAnimation : MonoBehaviour
    {
        [SerializeField] private CanvasGroup m_CanvasGroup;

        private void Update()
        {
            if (m_CanvasGroup.alpha >= 0)
                m_CanvasGroup.alpha -= Time.deltaTime;
            
            if (m_CanvasGroup.alpha == 0)
                enabled = false;
        }
    }
