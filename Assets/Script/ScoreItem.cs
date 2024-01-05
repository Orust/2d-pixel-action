using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreItem : MonoBehaviour
{
    [Header("how many add score")] public int myScore;
    [Header("judge player")] public PlayerTriggerCheck playerCheck;
    public AudioClip getStarSE;

    // Update is called once per frame
    void Update()
    {
        if (playerCheck.isOn)
        {
            if (GManager.instance != null)
            {
                GManager.instance.score += myScore;
                GManager.instance.PlaySE(getStarSE);
                Destroy(this.gameObject);
            }
        }
    }
}
