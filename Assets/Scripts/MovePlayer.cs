using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    public float speed = 500.0f;//玩家的移动速度
    public float jumpForce = 20.0f;//施加在玩家身上并且方向向上的力的大小
    //public float jumpSpeed;
    private Rigidbody2D rgb;//玩家身上的刚体
    private BoxCollider2D boxCollider;//玩家身上的碰撞器
    Animator animator;//玩家身上的动画控制器
    private bool isFalling;
    //public float gravity;
    private float deltaX;
    int i;
    SpriteRenderer spriterenderer;
    private void Awake()
    {
        spriterenderer = GetComponent<SpriteRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        rgb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        isFalling = false;
        i = 0;
    }
    // Update is called once per frame
    void Update()
    {
        deltaX = Time.deltaTime * speed * Input.GetAxis("Horizontal");
        animator.SetFloat("Running", Mathf.Abs(deltaX));
        if (!Mathf.Approximately(deltaX, 0))
        {
            //transform.localScale = new Vector3(Mathf.Sign(-deltaX), 1, 1);
            if (Mathf.Sign(-deltaX) == 1)
            {
                spriterenderer.flipX = false;
            }
            else
            {
                spriterenderer.flipX = true;
            }
        }
        Vector2 movement = new Vector2(deltaX, rgb.velocity.y);
        rgb.velocity = movement;
        Vector2 max = boxCollider.bounds.max;
        Vector2 min = boxCollider.bounds.min;
        Vector2 maxForDetect = new Vector2(max.x, min.y - 0.01f);
        Vector2 minForDetect = new Vector2(min.x, min.y - 0.02f);
        Collider2D hit = Physics2D.OverlapArea(minForDetect, maxForDetect,LayerMask.GetMask("Ground"));//判断玩家是否处在地面上
        //Physics2D.gravity = new Vector3(0, gravity, 0);
        bool grounded = false;
        if (hit != null && hit.gameObject.tag == "Ground")
        {
            grounded = true;
        }
        //Debug.Log("grounded : " + i+++ " " + grounded);
        if (grounded && Input.GetKeyDown(KeyCode.W))
        {
            animator.SetBool("BackToIdle", false);
            animator.SetBool("FinishJump", false);
            animator.SetBool("PrepareJump", true);
            StartCoroutine(Jump());
        }
        if(isFalling==true&&grounded)
        {
            animator.SetBool("Jumping", false);
            animator.SetBool("FinishJump", true);
        }
        if (rgb.velocity.y < -0.1f)
        {
            isFalling = true;
        }
        else
        {
            isFalling = false;
        }
    }

    public void JumpReturnIdle()
    {
        isFalling = false;
        animator.SetBool("FinishJump", false);
        animator.SetBool("BackToIdle", true);
    }

    public void RunReturnIdle()
    {
        animator.SetBool("RunFinish", true);
    }
    private IEnumerator Jump()
    {
        yield return new WaitForSeconds(0.3f);
        animator.SetBool("PrepareJump", false);
        animator.SetBool("Jumping", true);
        //rgb.velocity = new Vector2(rgb.velocity.x, jumpSpeed);
        rgb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
}
