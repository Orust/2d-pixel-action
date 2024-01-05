using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeImage : MonoBehaviour
{
    [Header("is complete fadeIn or not")] public bool firstFadeInComp;

    private Image img = null;
    private int frameCount = 0;
    private bool fadeIn = false;
    private bool fadeOut = false;
    private float timer = 0.0f;
    private bool compFadeIn = false;
    private bool compFadeOut = false;

    public void StartFadeIn()
    {
        if (fadeIn || fadeOut)
        {
            return;
        }
        fadeIn = true;
        compFadeIn = false;
        timer = 0.0f;
        img.color = new Color(1, 1, 1, 1);
        img.fillAmount = 1;
        img.raycastTarget = true;
    }
    public bool IsFadeInComplete()
    {
        return compFadeIn;
    }
    public void StartFadeOut()
    {
        if (fadeIn || fadeOut)
        {
            return;
        }
        fadeOut = true;
        compFadeOut = false;
        timer = 0.0f;
        img.color = new Color(1, 1, 1, 0);
        img.fillAmount = 0;
        img.raycastTarget = true;
    }
    public bool IsFadeOutComplete()
    {
        return compFadeOut;
    }

    // Start is called before the first frame update
    void Start()
    {
        img = GetComponent<Image>();
        if (firstFadeInComp)
        {
            FadeInComplete();
        }
        else
        {
            StartFadeIn();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // wait 2 frame because Time.deltaTime is longer by process moving to the scene
        if (frameCount > 2)
        {
            if (fadeIn)
            {
                FadeInUpdate();
            }
            else if (fadeOut)
            {
                FadeOutUpdate();
            }
        }
        ++frameCount;
    }
    private void FadeInUpdate()
    {
        if (timer < 1)
        {
            // use original color by 1,1,1
            // unvisualize by 1 - timer
            img.color = new Color(1, 1, 1, 1 - timer);
            img.fillAmount = 1 - timer;
        }
        else
        {
            FadeInComplete();
        }
        timer += Time.deltaTime;
    }
    private void FadeOutUpdate()
    {
        if (timer < 1)
        {
            img.color = new Color(1, 1, 1, timer);
            img.fillAmount = timer;
        }
        else
        {
            FadeOutComplete();
        }
        timer += Time.deltaTime;
    }

    private void FadeInComplete()
    {
        img.color = new Color(1, 1, 1, 0);
        img.fillAmount = 0;
        img.raycastTarget = false; // ignore image collusion
        timer = 0.0f;
        fadeIn = false;
        compFadeIn = true;
    }
    private void FadeOutComplete()
    {
        img.color = new Color(1, 1, 1, 1);
        img.fillAmount = 0;
        img.raycastTarget = false; // ignore image collusion
        timer = 0.0f;
        fadeOut = false;
        compFadeOut = true;
    }
}
