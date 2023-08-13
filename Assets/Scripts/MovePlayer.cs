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
    public float speed = 500.0f;//��ҵ��ƶ��ٶ�
    public float jumpForce = 20.0f;//ʩ����������ϲ��ҷ������ϵ����Ĵ�С
    private Rigidbody2D rgb;//������ϵĸ���
    private Collider2D collider;//������ϵ���ײ��
    Animator animator;//������ϵĶ���������
    private bool isFalling;//�ж�����Ƿ������䣬Ϊ������Ծ��ҡ������׼��
    private float deltaX;//��Һ����ƶ��ٶ�
    public float jumpSpeed;//��ҵ�ǽ������Ծ�ٶ�
    private jumpDirection lastJumpDirection = jumpDirection.None;//�����һ����Ծ�ķ���
    SpriteRenderer spriterenderer;//������ϵľ�����Ⱦ��
    //private float dashCD;//��ҳ�̼��ܵ�cd
    //private bool canDash;//������ڿ����ó�̼���
    //public float dashTime;//��ҿ��Դ��ڳ��״̬��ʱ��
    //private float lastDashTime;//��Ҿ����ϴ��ͷ�Dash������������ʱ��
    int i;
    private void Awake()
    {
        spriterenderer = GetComponent<SpriteRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        rgb = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        isFalling = false;
        //dashCD = 2.0f;
        //dashTime = 2.0f;
        i = 0;
    }
    // Update is called once per frame
    void Update()
    {
        deltaX = Time.deltaTime * speed * Input.GetAxis("Horizontal");
        animator.SetFloat("Running", Mathf.Abs(deltaX));//�ٶȴﵽһ���̶Ⱦ��л�Ϊ�ܲ�����
        if (!Mathf.Approximately(deltaX, 0))//�л���ҷ���
        {
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
        rgb.velocity = movement;//Ϊ��������ٶ�
        jumpDirection direction = getAllowJumpDirection();//��ȡ���������������Ծ�ķ���
        if(Input.GetKeyDown(KeyCode.W))//��Ұ�����Ծ��
        {
            if(direction == jumpDirection.Up)//��Ծ�����������
            {
                animator.SetBool("BackToIdle", false);//���ò�����Ծǰҡ����
                animator.SetBool("FinishJump", false);
                animator.SetBool("PrepareJump", true);
                StartCoroutine(Jump());
                lastJumpDirection = direction;
            }
            else if (direction == jumpDirection.Left&&direction!=lastJumpDirection)//��Ծ�����������
            {
                animator.SetBool("BackToIdle", false);
                animator.SetBool("FinishJump", false);
                rgb.velocity = new Vector2(rgb.velocity.x, jumpSpeed);
                lastJumpDirection = direction;
            }
            else if(direction == jumpDirection.Right&&direction!=lastJumpDirection)//��Ծ�����������
            {
                animator.SetBool("BackToIdle", false);
                animator.SetBool("FinishJump", false);
                rgb.velocity = new Vector2(rgb.velocity.x, jumpSpeed);
                lastJumpDirection = direction;
            }
        }
        //Debug.Log(i++ + " " + "direction:"+direction);
        if(isFalling==true&&direction==jumpDirection.Up)//�����������䲢���Ѿ����������˾Ͳ��Ž�����Ծ����
        {
            animator.SetBool("Jumping", false);
            animator.SetBool("FinishJump", true);
        }
        if (rgb.velocity.y < -0.1f)//��������Ƿ���������
        {
            isFalling = true;
        }
        else
        {
            isFalling = false;
        }
    }

    public void JumpReturnIdle()//���������Ծ��ҡ������ĩβ֡�¼����������²�����ҿ���ʱ�Ķ���
    {
        isFalling = false;
        animator.SetBool("FinishJump", false);
        animator.SetBool("BackToIdle", true);
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
        rgb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private jumpDirection getAllowJumpDirection()//���������ж���ҿ����ĸ�������Ծ�ĺ���
    {
        int downCount = collider.Cast(Vector2.down, new ContactFilter2D
        {
            layerMask = LayerMask.GetMask("Ground"),
            useLayerMask = true
        }, new RaycastHit2D[1], 0.1f, true);
        if (downCount > 0)
        { 
            return jumpDirection.Up; 
        }
        int leftCount = collider.Cast(Vector2.left, new ContactFilter2D
        {
            layerMask = LayerMask.GetMask("Ground"),
            useLayerMask = true
        }, new RaycastHit2D[1], 0.1f, true);
        if (leftCount > 0)
        {
            return jumpDirection.Right; 
        }
        int rightCount = collider.Cast(Vector2.right, new ContactFilter2D
        {
            layerMask = LayerMask.GetMask("Ground"),
            useLayerMask = true
        }, new RaycastHit2D[1], 0.1f, true);
        if (rightCount > 0)
        {
            return jumpDirection.Left;
        }
        return jumpDirection.None;
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