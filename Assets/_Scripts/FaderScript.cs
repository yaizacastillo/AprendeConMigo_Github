using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaderScript : MonoBehaviour {

    private CanvasGroup fadeGroup;
    private float loadTime;
    private float fadeTime = 3.0f;
    public bool fadeDone = false;
    public float fadeTimer = 0.0f;

    private void Start()
    {
        fadeGroup = FindObjectOfType<CanvasGroup>();

        fadeGroup.alpha = 1.0f;
    }

    private void Update()
    {
        if(!fadeDone)
        {
            fadeTimer += Time.deltaTime;
            Fade();
        }
    }

    private void Fade()
    {
        if (fadeTimer < fadeTime)
            fadeGroup.alpha = 1 - fadeTimer/fadeTime;

        else
            fadeDone = true;
    }
}
