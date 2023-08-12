using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class MoveEnemy : MonoBehaviour
{
    public GameObject player;
    public float distance;
    private Transform leftTransform;
    private Transform rightTransform;
    private float leftX;
    private float rightX;
    private int faceDirection;//-1��ʾ����1��ʾ����
    private Rigidbody2D rgd;
    public float speed;

    private int state;//0��ʾ�����ƶ���1��ʾ��ֹ,2��ʾ׷�����
    private float time;
    private float setTime;//���õ��ƶ���ֹͣʱ��
    private float maxMoveTime;//�����ƶ���ֹͣʱ��
    private float minMoveTime;//��С���ƶ���ֹͣʱ��
    public float chaseDist;
    //private Collider2D collider2D;
    //private NavMeshAgent2D navMeshAgent;
    // Start is called before the first frame update
    void Start()
    {
        leftTransform = transform.GetChild(0);
        rightTransform = transform.GetChild(1);
        leftX = leftTransform.position.x;
        rightX = rightTransform.position.x;
        Destroy(leftTransform.gameObject);
        Destroy(rightTransform.gameObject);
        faceDirection = 1;//Ĭ���������ƶ�
        rgd = GetComponent<Rigidbody2D>();
        speed = 10000.0f;
        state = 0;//Ĭ���������ƶ�
        time = 0;
        setTime = Random.Range(minMoveTime, maxMoveTime);
        minMoveTime = 0f;
        maxMoveTime = 5.0f;
        chaseDist = 2.0f;
        //collider2D = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Transform playerPos = player.transform;
        Collider2D collider2Ds = Physics2D.OverlapBox(transform.position, new Vector2(10.0f,10.0f), 0, LayerMask.GetMask("Player"));
        if(collider2Ds != null)
        {
            if (player.transform.position.x < transform.position.x)
            {
                rgd.velocity = new Vector2(-1 * speed * Time.deltaTime, rgd.velocity.y);
            }
            else
            {
                rgd.velocity = new Vector2(1 * speed * Time.deltaTime, rgd.velocity.y);
            }
        }
        else
        {
            time = time + Time.deltaTime;
            if (time > setTime)
            {
                if (state == 0)
                {
                    state = 1;
                }
                else
                {
                    state = 0;
                }
                time = 0;
                setTime = Random.Range(minMoveTime, maxMoveTime);
            }
            else
            {
                if (state == 0)
                {
                    FaceDirection();
                    rgd.velocity = new Vector2(faceDirection * Time.deltaTime * speed, rgd.velocity.y);
                }
                else
                {
                    rgd.velocity = Vector2.zero;
                }
            }
        }
    }

    void FaceDirection()
    {
        if(faceDirection == 1)
        {
            if(transform.position.x >=rightX)
            {
                faceDirection = -1;
            }
        }
        else
        {
            if(transform.position.x<=leftX)
            {
                faceDirection = 1;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(faceDirection ==-1)
        {
            faceDirection = 1;
        }
        else if(faceDirection ==1)
        {
            faceDirection = -1;
        }
    }
}
