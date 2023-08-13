using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum jumpDirection
{
    None,
    Left,
    Right,
    Up
}
public class MovePlayer : MonoBehaviour
{
    public float jumpSpeed = 20.0f;//施加在玩家身上并且方向向上的力的大小
    private Rigidbody2D rgb;//玩家身上的刚体
    private Collider2D collider;//玩家身上的碰撞器

    Animator animator;//玩家身上的动画控制器
    AudioSource audios;


    public AudioClip jump, Run;

    private bool isFalling;//判断玩家是否在下落，为播放跳跃后摇动画作准备
    private float deltaX;//玩家横向移动速度
    private jumpDirection lastJumpDirection = jumpDirection.None;//玩家上一次跳跃的方向
    SpriteRenderer spriterenderer;//玩家身上的精灵渲染器
    public float maxSpeed;
    private bool isOnWall;
    private jumpDirection direction;
    public Sprite normalImage;
    public Sprite onWallImage;
    public float moveForce;
    //private float dashCD;//玩家冲刺技能的cd
    //private bool canDash;//玩家现在可以用冲刺技能
    //public float dashTime;//玩家可以处在冲刺状态的时间
    //private float lastDashTime;//玩家距离上次释放Dash技能所经历的时间
    int i;
    // Start is called before the first frame update
    void Start()
    {
        spriterenderer = GetComponent<SpriteRenderer>();
        rgb = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        audios = GetComponent<AudioSource>();
        isFalling = false;
        maxSpeed = 20.0f;
        isOnWall = false;
        moveForce = 100.0f;
        //dashCD = 2.0f;
        //dashTime = 2.0f;
        i = 0;
    }
    // Update is called once per frame
    void Update()
    {
        if (direction != jumpDirection.Left && direction != jumpDirection.Right)
        {
            spriterenderer.sprite = normalImage;
        }
        else
        {
            spriterenderer.sprite = onWallImage;
        }
        float velocity = Input.GetAxis("Horizontal");
        if (Mathf.Approximately(velocity, 0) == false)
        {
            rgb.AddForce(velocity * moveForce * Vector2.right);
        }
        animator.SetFloat("RunningSpeed", Mathf.Abs(velocity * moveForce));//速度达到一定程度就切换为跑步动画
        rgb.velocity = new Vector2(
            Mathf.Clamp(velocity * moveForce, -maxSpeed, maxSpeed),
            rgb.velocity.y
        );
        if (!Mathf.Approximately(velocity * moveForce, 0))//切换玩家方向
        {
            spriterenderer.flipX = (Mathf.Sign(velocity * moveForce) == 1);
        }
        direction = getAllowJumpDirection();//获取现在玩家所允许跳跃的方向
        if (rgb.velocity.y < -0.1f && direction == jumpDirection.Up)//如果玩家在下落并且已经到地面上了就播放结束跳跃动画
        {
            //Debug.Log("hit");
            animator.SetBool("Jumping", false);
            animator.SetBool("FinishJump", true);
        }
        if (Input.GetKeyDown(KeyCode.W))//玩家按下跳跃键
        {
            if(direction == jumpDirection.Up)//跳跃方向可以向上
            {
                Debug.Log("Direction:"+i+++"up");
                animator.SetBool("BackToIdle", false);//设置播放跳跃前摇动画
                animator.SetBool("FinishJump", false);
                animator.SetBool("PrepareJump", true);
                StartCoroutine(Jump());
                lastJumpDirection = direction;
            }
            else if (direction == jumpDirection.Left&&direction!=lastJumpDirection)//跳跃方向可以向左
            {
                Debug.Log("Direction:" + i++ + "left");
                animator.SetBool("BackToIdle", false);
                animator.SetBool("FinishJump", false);
                rgb.velocity = new Vector2(rgb.velocity.x, jumpSpeed);
                lastJumpDirection = direction;
            }
            else if(direction == jumpDirection.Right&&direction!=lastJumpDirection)//跳跃方向可以向右
            {
                Debug.Log("Direction:" + i++ + "right");
                animator.SetBool("BackToIdle", false);
                animator.SetBool("FinishJump", false);
                rgb.velocity = new Vector2(rgb.velocity.x, jumpSpeed);
                lastJumpDirection = direction;
            }
            return;
        }
        else if(Input.GetKeyDown(KeyCode.F))//玩家开始攻击
        {
            RaycastHit2D[] raycastHit = Physics2D.RaycastAll(transform.position, new Vector2(Mathf.Sign(deltaX), transform.position.y), 2.0f, LayerMask.GetMask("Enemy"));
            if(raycastHit.Length > 0)
            {
                animator.Play("Attack");
                foreach(var ray in raycastHit)
                {
                    Destroy(ray.collider.gameObject);
                }
            }
            return;
        }
        /*Debug.Log(i++ + " " + "direction:"+direction);
        if(rgb.velocity.y < -0.1f && direction == jumpDirection.Up)//如果玩家在下落并且已经到地面上了就播放结束跳跃动画
        {
            Debug.Log("hit");
            animator.SetBool("Jumping", false);
            animator.SetBool("FinishJump", true);
        }*/
    }

    public void JumpReturnIdle()//这是玩家跳跃后摇动画的末尾帧事件，用于重新播放玩家空闲时的动画
    {
        animator.SetBool("BackToIdle", true);
        animator.SetBool("FinishJump", false);
    }

    public void RunReturnIdle()//这是玩家跑步后摇动画的末尾帧事件，用于重新播放玩家空闲时的动画
    {
        animator.SetBool("RunFinish", true);
    }
    private IEnumerator Jump()//利用协程等待跳跃前摇动画播放完后跳跃
    {
        yield return new WaitForSeconds(0.3f);
        animator.SetBool("PrepareJump", false);//设置播放跳跃动画
        animator.SetBool("Jumping", true);
        //rgb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        rgb.velocity = new Vector2(rgb.velocity.x, jumpSpeed);
    }

    private jumpDirection getAllowJumpDirection()//这是用于判断玩家可向哪个方向跳跃的函数
    {
        int downCount = collider.Cast(Vector2.down, new ContactFilter2D
        {
            layerMask = LayerMask.GetMask("Ground"),
            useLayerMask = true
        }, new RaycastHit2D[1], 0.1f, true);
        int leftCount = collider.Cast(Vector2.left, new ContactFilter2D
        {
            layerMask = LayerMask.GetMask("Ground"),
            useLayerMask = true
        }, new RaycastHit2D[1], 0.1f, true);
        int rightCount = collider.Cast(Vector2.right, new ContactFilter2D
        {
            layerMask = LayerMask.GetMask("Ground"),
            useLayerMask = true
        }, new RaycastHit2D[1], 0.1f, true);
        if (downCount > 0)
        {
            animator.SetBool("OnWall", false);
            return jumpDirection.Up;
        }
        else if (leftCount > 0)
        {
            animator.SetBool("OnWall", true);
            return jumpDirection.Right;
        }
        else if (rightCount > 0)
        {
            animator.SetBool("OnWall", true);
            return jumpDirection.Left;
        }
        else
        {
            return jumpDirection.None;
        }
    }
}

/*Vector2 max = boxCollider.bounds.max;
        Vector2 min = boxCollider.bounds.min;
        Vector2 maxForDetect = new Vector2(max.x, min.y - 0.01f);
        Vector2 minForDetect = new Vector2(min.x, min.y - 0.02f);
        Collider2D hit = Physics2D.OverlapArea(minForDetect, maxForDetect,LayerMask.GetMask("Ground"));//判断玩家是否处在地面上*/
/*bool grounded = false;
if (hit != null && hit.gameObject.tag == "Ground")
{
    grounded = true;
}*/
//Debug.Log("grounded : " + i+++ " " + grounded);
/*if (grounded && Input.GetKeyDown(KeyCode.W))
        {
            animator.SetBool("BackToIdle", false);
            animator.SetBool("FinishJump", false);
            animator.SetBool("PrepareJump", true);
            StartCoroutine(Jump());
        }*/