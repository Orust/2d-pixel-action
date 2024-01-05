using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageCtrl : MonoBehaviour
{
    public GameObject playerObj;
    public GameObject[] continuePoint;
    public GameObject gameOverObj;
    public FadeImage fade;
    public AudioClip gameOverSE;
    public AudioClip retrySE;
    public AudioClip stageClearSE;
    public GameObject stageClearObj;
    public PlayerTriggerCheck stageClearTriger;

    private Player p;
    private int nextStageNum;
    private bool startFade = false;
    private bool doGameOver = false;
    private bool retryGame = false;
    private bool doSceneChange = false;
    private bool doClear = false;

    // Start is called before the first frame update
    void Start()
    {
        if (playerObj != null && continuePoint != null && continuePoint.Length > 0 && gameOverObj != null && fade != null)
        {
            gameOverObj.SetActive(false);
            stageClearObj.SetActive(false);
            playerObj.transform.position = continuePoint[0].transform.position;

            p = playerObj.GetComponent<Player>();
            if (p == null)
            {
                Debug.Log("attached not player");
            }
        }
        else
        {
            Debug.Log("not enough settings!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // game over
        if (GManager.instance.isGameOver && !doGameOver)
        {
            GManager.instance.PlaySE(gameOverSE);
            gameOverObj.SetActive(true);
            doGameOver = true;
        }
        else if (p != null && p.IsContinueWaiting() && !doGameOver)
        {
            if (continuePoint.Length > GManager.instance.continueNum)
            {
                playerObj.transform.position = continuePoint[GManager.instance.continueNum].transform.position;
                p.ContinuePlayer();
            }
            else
            {
                Debug.Log("not enough settings for continue point");
            }
        }
        else if (stageClearTriger != null && stageClearTriger.isOn && !doGameOver && !doClear)
        {
            StageClear();
            doClear = true;
        }

        // change stage
        if (fade != null && startFade && !doSceneChange)
        {
            if (fade.IsFadeOutComplete())
            {
                if (retryGame)
                {
                    GManager.instance.RetryGame();
                }
                else
                {
                    GManager.instance.stageNum = nextStageNum;
                }
                GManager.instance.isStageClear = false;
                SceneManager.LoadScene("stage" + nextStageNum);
                doSceneChange = true;
            }
        }
    }

    public void Retry()
    {
        GManager.instance.PlaySE(retrySE);
        ChangeScene(1); // go back to the first scene
        retryGame = true;
    }

    /// <summary>
    /// change stage
    /// </summary>
    /// <param name="num"></param>
    public void ChangeScene(int num)
    {
        if (fade != null)
        {
            nextStageNum = num;
            fade.StartFadeOut();
            startFade = true;
        }
    }
    /// <summary>
    /// stage clear
    /// </summary>
    public void StageClear()
    {
        GManager.instance.isStageClear = true;
        stageClearObj.SetActive(true);
        GManager.instance.PlaySE(stageClearSE);
    }
}
