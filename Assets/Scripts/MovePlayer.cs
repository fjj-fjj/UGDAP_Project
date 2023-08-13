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
    public float jumpSpeed = 20.0f;//ʩ����������ϲ��ҷ������ϵ����Ĵ�С
    private Rigidbody2D rgb;//������ϵĸ���
    private Collider2D collider;//������ϵ���ײ��

    Animator animator;//������ϵĶ���������
    AudioSource audios;


    public AudioClip jump, Run;

    private bool isFalling;//�ж�����Ƿ������䣬Ϊ������Ծ��ҡ������׼��
    private float deltaX;//��Һ����ƶ��ٶ�
    private jumpDirection lastJumpDirection = jumpDirection.None;//�����һ����Ծ�ķ���
    SpriteRenderer spriterenderer;//������ϵľ�����Ⱦ��
    public float maxSpeed;
    private bool isOnWall;
    private jumpDirection direction;
    public Sprite normalImage;
    public Sprite onWallImage;
    public float moveForce;
    //private float dashCD;//��ҳ�̼��ܵ�cd
    //private bool canDash;//������ڿ����ó�̼���
    //public float dashTime;//��ҿ��Դ��ڳ��״̬��ʱ��
    //private float lastDashTime;//��Ҿ����ϴ��ͷ�Dash������������ʱ��
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
        animator.SetFloat("RunningSpeed", Mathf.Abs(velocity * moveForce));//�ٶȴﵽһ���̶Ⱦ��л�Ϊ�ܲ�����
        rgb.velocity = new Vector2(
            Mathf.Clamp(velocity * moveForce, -maxSpeed, maxSpeed),
            rgb.velocity.y
        );
        if (!Mathf.Approximately(velocity * moveForce, 0))//�л���ҷ���
        {
            spriterenderer.flipX = (Mathf.Sign(velocity * moveForce) == 1);
        }
        direction = getAllowJumpDirection();//��ȡ���������������Ծ�ķ���
        if (rgb.velocity.y < -0.1f && direction == jumpDirection.Up)//�����������䲢���Ѿ����������˾Ͳ��Ž�����Ծ����
        {
            //Debug.Log("hit");
            animator.SetBool("Jumping", false);
            animator.SetBool("FinishJump", true);
        }
        if (Input.GetKeyDown(KeyCode.W))//��Ұ�����Ծ��
        {
            if(direction == jumpDirection.Up)//��Ծ�����������
            {
                Debug.Log("Direction:"+i+++"up");
                animator.SetBool("BackToIdle", false);//���ò�����Ծǰҡ����
                animator.SetBool("FinishJump", false);
                animator.SetBool("PrepareJump", true);
                StartCoroutine(Jump());
                lastJumpDirection = direction;
            }
            else if (direction == jumpDirection.Left&&direction!=lastJumpDirection)//��Ծ�����������
            {
                Debug.Log("Direction:" + i++ + "left");
                animator.SetBool("BackToIdle", false);
                animator.SetBool("FinishJump", false);
                rgb.velocity = new Vector2(rgb.velocity.x, jumpSpeed);
                lastJumpDirection = direction;
            }
            else if(direction == jumpDirection.Right&&direction!=lastJumpDirection)//��Ծ�����������
            {
                Debug.Log("Direction:" + i++ + "right");
                animator.SetBool("BackToIdle", false);
                animator.SetBool("FinishJump", false);
                rgb.velocity = new Vector2(rgb.velocity.x, jumpSpeed);
                lastJumpDirection = direction;
            }
            return;
        }
        else if(Input.GetKeyDown(KeyCode.F))//��ҿ�ʼ����
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
        if(rgb.velocity.y < -0.1f && direction == jumpDirection.Up)//�����������䲢���Ѿ����������˾Ͳ��Ž�����Ծ����
        {
            Debug.Log("hit");
            animator.SetBool("Jumping", false);
            animator.SetBool("FinishJump", true);
        }*/
    }

    public void JumpReturnIdle()//���������Ծ��ҡ������ĩβ֡�¼����������²�����ҿ���ʱ�Ķ���
    {
        animator.SetBool("BackToIdle", true);
        animator.SetBool("FinishJump", false);
    }

    public void RunReturnIdle()//��������ܲ���ҡ������ĩβ֡�¼����������²�����ҿ���ʱ�Ķ���
    {
        animator.SetBool("RunFinish", true);
    }
    private IEnumerator Jump()//����Э�̵ȴ���Ծǰҡ�������������Ծ
    {
        yield return new WaitForSeconds(0.3f);
        animator.SetBool("PrepareJump", false);//���ò�����Ծ����
        animator.SetBool("Jumping", true);
        //rgb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        rgb.velocity = new Vector2(rgb.velocity.x, jumpSpeed);
    }

    private jumpDirection getAllowJumpDirection()//���������ж���ҿ����ĸ�������Ծ�ĺ���
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
        Collider2D hit = Physics2D.OverlapArea(minForDetect, maxForDetect,LayerMask.GetMask("Ground"));//�ж�����Ƿ��ڵ�����*/
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