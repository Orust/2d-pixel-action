using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GManager : MonoBehaviour
{
    public static GManager instance = null;
    public int score;
    public int stageNum;
    public int continueNum;
    public int heartNum;
    public int defaultHeartNum;
    [HideInInspector] public bool isGameOver = false;
    [HideInInspector] public bool isStageClear = false;

    private AudioSource audioSource = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            // singleton
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// sub heart number
    /// </summary>
    public void SubHeartNum()
    {
        if(heartNum > 0)
        {
            --heartNum;
        }
        else
        {
            isGameOver = true;
        }

    }
    /// <summary>
    /// add heart number
    /// </summary>
    public void AddHeartNum()
    {
        if (heartNum < 99)
        {
            ++heartNum;
        }
    }

    public void RetryGame()
    {
        isGameOver = false;
        heartNum = defaultHeartNum;
        score = 0;
        stageNum = 1;
        continueNum = 0;
    }

    /// <summary>
    /// play SE
    /// </summary>
    /// <param name="clip"></param>
    public void PlaySE(AudioClip clip)
    {
        if (audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.Log("no setting for audiosource");
        }
    }
}
