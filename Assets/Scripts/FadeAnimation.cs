using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeAnimation : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private bool CanFade;

    public void Start()
    {
        CanFade = true;
    }

    public void Update()
    {
        if (CanFade == true)
        {
            if (m_CanvasGroup.alpha >= 0)
            {
                m_CanvasGroup.alpha -= Time.deltaTime;
            }
            if (m_CanvasGroup.alpha == 0)
            {
                CanFade = false;
            }
        }
    }
}
