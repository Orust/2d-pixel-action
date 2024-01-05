using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    public FadeImage fade;

    private bool firstPush = false;
    private bool goNextScene = false;

    public void PressStart()
    {
        Debug.Log("press start");
        if (!firstPush)
        {
            Debug.Log("go to next scene");
            fade.StartFadeOut();
            firstPush = true;
        }

    }

    private void Update()
    {
        if (!goNextScene && fade.IsFadeOutComplete())
        {
            SceneManager.LoadScene("stage1");
            goNextScene = true;
        }
    }
}
