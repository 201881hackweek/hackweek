using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    //状态
    public bool isJumping;
    public bool isMovingX;
    public bool isStaticX;
    public bool isLanding;

    //限制 (以下数值public方便观察，完成后改private
    public bool canJump;
    public bool canMove;

    //值
    public float jumpVal;
    public float jumpSpeed;
    public float moveVal;
    public float moveSpeed;
    public float scaleX;

    //组件
    public Rigidbody2D rigidbody2;
    public Animator animator;

	// Use this for initialization
	void Start () {

        rigidbody2 = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        //状态初始化
        isJumping = false;
        isMovingX = false;
        isStaticX = true;
        isLanding = true;

        //数据初始化
        canJump = true;
        canMove = true;
        jumpSpeed = 15f;
        moveSpeed = 5f;
        scaleX = transform.localScale.x;
	}
	
	// Update is called once per frame
	void Update ()
    {
        //检测
        CheckLanding();             //根据着地更新状态
        AlterByState();             //根据状态更新限制条件

        //移动操作
        moveVal = Input.GetAxis("Horizontal");
        if (canMove && moveVal != 0)
            MoveX(moveVal);
        else if( moveVal == 0)
            StopMovingX();

        //跳跃操作
        jumpVal = Input.GetAxis("Jump");
        if (canJump && jumpVal>0)
            Jump();

        
	}

    //移动
    void MoveX(float val)
    {
        if (val < 0)
            transform.localScale = new Vector3(-1* scaleX, transform.localScale.y, transform.localScale.z);
        if (val > 0)
            transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);


        rigidbody2.velocity = new Vector2(val * moveSpeed, rigidbody2.velocity.y);

        isStaticX = false;
        isMovingX = true;
    }

    void StopMovingX()
    {
        rigidbody2.velocity = new Vector2(0, rigidbody2.velocity.y);

        isStaticX = true;
        isMovingX = false;
    }

    //跳跃
    void Jump()
    {
        rigidbody2.velocity = new Vector2(rigidbody2.velocity.x,jumpSpeed * jumpVal);

        isJumping = true;
    }


    //根据状态修改限制条件
    void AlterByState()
    {
        if (isLanding)
        {
            canJump = true;
        }
        if (isJumping)
        {
            canJump = false;                            //不能二段跳
        }
        if(isMovingX)                                   //isMovingX和isStaticX刚好相反可以考虑取消其中一个
        {
            
        }
        if(isStaticX)
        {
            
        }
    }

   
    //检测着地状态
    public void CheckLanding()
    {
        Vector3 origin;         //检测起点
        Vector3 direction;      //检测方向
        LayerMask layerMask;    //检测对象层
        float depth;            //检测深度

        depth = 1.2f;
        origin = transform.position;
        direction = Vector3.down;                       
        layerMask = LayerMask.GetMask("Obstacle");      //对象层为障碍

   
        Debug.DrawRay(origin, depth * direction, Color.red);        //显示射线

        if (Physics2D.Raycast(origin, Vector3.down, depth, layerMask))
        {
            isLanding = true;              //着地状态
            isJumping = false;
        }
        else
            isLanding = false;
    }
}
