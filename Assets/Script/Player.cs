using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region// possible to set in inspector
    public float speed;
    public float jumpSpeed;
    public float jumpHeight;
    public float jumpLimitTime;
    public float stepOnRate;
    public float gravity;
    public GroundCheck ground;
    public GroundCheck head;
    public AnimationCurve dashCurve;
    public AnimationCurve jumpCurve;
    public AudioClip jumpSE;
    public AudioClip otherJumpSE;
    public AudioClip receiveDamageSE;
    #endregion

    #region// private value
    private Animator anim = null;
    private Rigidbody2D rb = null;
    private CapsuleCollider2D capcol = null;
    private SpriteRenderer sr = null;
    private MoveObject moveObj = null;
    private bool isGround = false;
    private bool isHead = false;
    private bool isJump = false;
    private bool isRun = false;
    private bool isDead = false;
    private bool isOtherJump = false;
    private bool isContinue = false;
    private bool nonDeadAnim = false;
    private bool isClearMotion = false;
    private float continueTime = 0.0f;
    private float blinkTime = 0.0f;
    private float jumpPos = 0.0f;
    private float otherJumpHeight = 0.0f;
    private float jumpTime = 0.0f;
    private float dashTime = 0.0f;
    private float beforeKey = 0.0f;
    private string enemyTag = "Enemy";
    private string deadAreaTag = "DeadArea";
    private string hitAreaTag = "HitArea";
    private string moveFloorTag = "MoveFloor";
    private string fallFloorTag = "FallFloor";
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        capcol = GetComponent<CapsuleCollider2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (isContinue)
        {
            if (blinkTime > 0.2f)
            {
                sr.enabled = true;
                blinkTime = 0.0f;
            }
            else if(blinkTime > 0.1f)
            {
                sr.enabled = false;
            }
            else
            {
                sr.enabled = true;
            }
            if(continueTime > 1.0f)
            {
                isContinue = false;
                blinkTime = 0.0f;
                continueTime = 0.0f;
                sr.enabled = true;
            }
            else
            {
                blinkTime += Time.deltaTime;
                continueTime += Time.deltaTime;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isDead && !GManager.instance.isGameOver && !GManager.instance.isStageClear)
        {
            // judge grounding
            isGround = ground.IsGround();
            isHead = head.IsGround();

            // get velocity

            float ySpeed = GetYSpeed();
            float xSpeed = GetXSpeed();

            // apply animation
            SetAnimation();

            // set velocity
            Vector2 addVelocity = Vector2.zero;
            if (moveObj != null)
            {
                addVelocity = moveObj.GetVelocity();
            }
            rb.velocity = new Vector2(xSpeed, ySpeed) + addVelocity;
        }
        else
        {
            if (!isClearMotion && GManager.instance.isStageClear)
            {
                anim.Play("player_clear");
                isClearMotion = true;
            }
            rb.velocity = new Vector2(0, -gravity);
        }
    }

    /// <summary>
    /// calculate y value and return velocity
    /// </summary>
    /// <returns>velocity on y-axis</returns>
    private float GetYSpeed()
    {
        float verticalKey = Input.GetAxis("Vertical");
        float ySpeed = -gravity;

        if (isOtherJump)
        {
            //check possibility to jump by current height
            bool canHeight = jumpPos + otherJumpHeight > transform.position.y;
            //check jump time too long or not
            bool canTime = jumpLimitTime > jumpTime;
            if (canHeight && canTime && !isHead)
            {
                ySpeed = jumpSpeed;
                jumpTime += Time.deltaTime;
            }
            else
            {
                isOtherJump = false;
                jumpTime = 0.0f;
            }
        }
        else if (isGround)
        {
            if (verticalKey > 0)
            {
                if (!isJump)     // let sound once
                {
                    GManager.instance.PlaySE(jumpSE);
                }
                ySpeed = jumpSpeed;
                jumpPos = transform.position.y;
                isJump = true;
                jumpTime = 0.0f;
            }
            else
            {
                isJump = false;
            }
        }
        else if (isJump)
        {
            //check push up key
            bool pushUpKey = verticalKey > 0;
            //check possibility to jump by current height
            bool canHeight = jumpPos + jumpHeight > transform.position.y;
            //check jump time too long or not
            bool canTime = jumpLimitTime > jumpTime;
            if (pushUpKey && canHeight && canTime && !isHead)
            {
                ySpeed = jumpSpeed;
                jumpTime += Time.deltaTime;
            }
            else
            {
                isJump = false;
                jumpTime = 0.0f;
            }
        }
        if (isJump || isOtherJump)
        {
            ySpeed *= jumpCurve.Evaluate(jumpTime);
        }
        return ySpeed;
    }

    /// <summary>
    /// calculate x value and return velocity
    /// </summary>
    /// <returns>velocity on x-axis</returns>
    private float GetXSpeed()
    {
        float horizontalKey = Input.GetAxis("Horizontal");
        float xSpeed = 0.0f;

        if (horizontalKey > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            isRun = true;
            dashTime += Time.deltaTime;
            xSpeed = speed;
        }
        else if (horizontalKey < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            isRun = true;
            dashTime += Time.deltaTime;
            xSpeed = -speed;
        }
        else
        {
            isRun = false;
            dashTime = 0.0f;
            xSpeed = 0.0f;
        }

        //reset time by reversing before input
        if (horizontalKey > 0 && beforeKey < 0)
        {
            dashTime = 0.0f;
        }
        else if (horizontalKey < 0 && beforeKey > 0)
        {
            dashTime = 0.0f;
        }
        beforeKey = horizontalKey;

        //apply animationcurve to velocity
        xSpeed *= dashCurve.Evaluate(dashTime);
        return xSpeed;
    }

    /// <summary>
    /// set animation
    /// </summary>
    private void SetAnimation()
    {
        anim.SetBool("jump", isJump || isOtherJump);
        anim.SetBool("ground", isGround);
        anim.SetBool("run", isRun);
    }

 
    /// <summary>
    /// dead animation waiting
    /// </summary>
    /// <returns></returns>
    /// 
    public bool IsContinueWaiting()
    {
        if (GManager.instance.isGameOver)
        {
            return false;
        }
        else
        {
            return IsDeadAnimEnd() || nonDeadAnim;
        }
    }

    /// <summary>
    /// dead animation is ended
    /// </summary>
    /// <returns></returns>
    private bool IsDeadAnimEnd()
    {
        if (isDead && anim != null)
        {
            AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);
            if (currentState.IsName("player_dead"))
            {
                if (currentState.normalizedTime >= 1)
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// continue
    /// </summary>
    public void ContinuePlayer()
    {
        isDead = false;
        anim.Play("player_stand");
        isJump = false;
        isOtherJump = false;
        isRun = false;
        isContinue = true;
        nonDeadAnim = false;
    }

    private void ReceiveDamage(bool deadAnim)
    {
        if (isDead || GManager.instance.isStageClear)
        {
            return;
        }
        else
        {
            if (deadAnim)
            {
                anim.Play("player_dead");
            }
            else
            {
                nonDeadAnim = true;
            }
            
            isDead = true;
            GManager.instance.SubHeartNum();
        }
    }

    /// <summary>
    /// step on object
    /// </summary>
    /// <param name="collision"></param>
    /// 
    private void OnCollisionEnter2D(Collision2D collision)
    {
        bool enemy = (collision.collider.tag == enemyTag);
        bool moveFloor = (collision.collider.tag == moveFloorTag);
        bool fallFloor = (collision.collider.tag == fallFloorTag);

        if (enemy || moveFloor || fallFloor)
        {
            //踏みつけ判定になる高さ
            float stepOnHeight = (capcol.size.y * (stepOnRate / 100f));

            //踏みつけ判定のワールド座標
            float judgePos = transform.position.y - (capcol.size.y / 2f) + stepOnHeight;

            foreach (ContactPoint2D p in collision.contacts)
            {
                if (p.point.y < judgePos)
                {
                    if (enemy || fallFloor)
                    {
                        ObjectCollision o = collision.gameObject.GetComponent<ObjectCollision>();
                        if (o != null)
                        {
                            if (enemy)
                            {
                                otherJumpHeight = o.boundHeight;    //踏んづけたものから跳ねる高さを取得する
                                o.playerStepOn = true;        //踏んづけたものに対して踏んづけた事を通知する
                                jumpPos = transform.position.y; //ジャンプした位置を記録する
                                if (!isOtherJump)     // let sound once
                                {
                                    GManager.instance.PlaySE(otherJumpSE);
                                }
                                isOtherJump = true;
                                isJump = false;
                                jumpTime = 0.0f;
                            }
                            else if (fallFloor)
                            {
                                o.playerStepOn = true;
                            }
                        }
                        else
                        {
                            Debug.Log("ObjectCollisionが付いてないよ!");
                        }
                    }
                    else if (moveFloor)
                    {
                        moveObj = collision.gameObject.GetComponent<MoveObject>();
                    }
                }
                else
                {
                    if (enemy)
                    {
                        GManager.instance.PlaySE(receiveDamageSE);
                        ReceiveDamage(true);
                        break;
                    }
                }
            }
        }
        // before simplification
        /**
        if (collision.collider.tag == enemyTag)
        {
            // height to be sit on
            float stepOnHeight = (capcol.size.y * (stepOnRate / 100f));

            // possition for sit on
            float judgePos = transform.position.y - (capcol.size.y / 2f) + stepOnHeight;

            // many contact points
            foreach (ContactPoint2D p in collision.contacts)
            {
                if (p.point.y < judgePos)
                {
                    // jump again
                    // gameObject is the object attached Rigidbody2D
                    ObjectCollision o = collision.gameObject.GetComponent<ObjectCollision>();
                    if (o != null)
                    {
                        otherJumpHeight = o.boundHeight;    //踏んづけたものから跳ねる高さを取得する
                        o.playerStepOn = true;        //踏んづけたものに対して踏んづけた事を通知する
                        jumpPos = transform.position.y; //ジャンプした位置を記録する
                        if (!isOtherJump)     // let sound once
                        {
                            GManager.instance.PlaySE(otherJumpSE);
                        }
                        isOtherJump = true;
                        isJump = false;
                        jumpTime = 0.0f;
                    }
                    else
                    {
                        Debug.Log("no ObjectCollision");
                    }
                }
                else
                {
                    // player dead
                    GManager.instance.PlaySE(receiveDamageSE);
                    ReceiveDamage(true);
                    break;
                }
            }
        }
        else if (collision.collider.tag == moveFloorTag)
        {
            // height to be sit on
            float stepOnHeight = (capcol.size.y * (stepOnRate / 100f));

            // possition for sit on
            float judgePos = transform.position.y - (capcol.size.y / 2f) + stepOnHeight;

            // many contact points
            foreach (ContactPoint2D p in collision.contacts)
            {
                // ride on the moving floor
                if (p.point.y < judgePos)
                {
                    // get script of moving floor
                    moveObj = collision.gameObject.GetComponent<MoveObject>();
                }
            }
        }
        **/
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == moveFloorTag)
        {
            // release the script if player take off from the moving floor 
            //動く床から離れた
            moveObj = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.tag == deadAreaTag)
        {
            GManager.instance.PlaySE(receiveDamageSE);
            ReceiveDamage(false);
        }
        else if(collision.tag == hitAreaTag)
        {
            GManager.instance.PlaySE(receiveDamageSE);
            ReceiveDamage(true);
        }
    }
}
