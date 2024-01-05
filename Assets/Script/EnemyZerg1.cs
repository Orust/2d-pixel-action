using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyZerg1 : MonoBehaviour
{
    #region// set on inspector
    [Header("add score")] public int myScore;
    public float speed;
    public float gravity;
    [Header("move without screen or not")] public bool nonVisible;
    [Header("judge contact")] public EnemyCollisionCheck checkCollision;
    #endregion

    #region// private value
    private Rigidbody2D rb = null;
    private SpriteRenderer sr = null;
    private Animator anim = null;
    private ObjectCollision oc = null;
    private BoxCollider2D col = null;
    private bool rightTleftF = false;
    private bool isDead = false;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        oc = GetComponent<ObjectCollision>();
        col = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!oc.playerStepOn)
        {
            if (sr.isVisible || nonVisible)
            {
                if (checkCollision.isOn)
                {
                    rightTleftF = !rightTleftF;
                }
                int xVector = -1;
                if (rightTleftF)
                {
                    xVector = 1;
                    transform.localScale = new Vector3(-1, 1, 1);
                }
                else
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }
                rb.velocity = new Vector2(xVector * speed, -gravity);
            }
            else
            {
                rb.Sleep();
            }
        }
        else
        {
            if (!isDead)
            {
                anim.Play("dead");
                rb.velocity = new Vector2(0, -gravity);
                isDead = true;
                col.enabled = false;
                if (GManager.instance != null)
                {
                    GManager.instance.score += myScore;
                }
                Destroy(gameObject, 3f);
            }
            else
            {
                transform.Rotate(new Vector3(0, 0, 5));
            }
        }
    }
}
