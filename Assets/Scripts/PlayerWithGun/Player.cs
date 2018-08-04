using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    //
    public static Player instance;
    //状态值
    public int life;
    public int damage;
    public int san;

    //状态
    public bool isJumping;
    public bool isMovingX;
    public bool isStaticX;
    public bool isLanding;
    public bool isRunning;
    public bool isCorching;

    //限制 (以下数值public方便观察，完成后改private
    public bool canJump;
    public bool canMove;
    public bool canRun;
    public bool canCorch;

    //值
    public float jumpVal;
    public float jumpSpeed;
    public float walkSpeed;
    public float runSpeed;
    public float moveVal;
    public float moveSpeed;
    public float verVal;
    public float scaleX;

    public int audioIndex;

    //组件
    public Rigidbody2D rigidbody2;
    public Animator animator;
    public CapsuleCollider2D collider2;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }
    // Use this for initialization
    void Start () {

        rigidbody2 = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        collider2 = gameObject.GetComponent<CapsuleCollider2D>();

        //值初始化
        life = 3;
        damage = 1;
        san = 10;

        //状态初始化
        isJumping = false;
        isMovingX = false;
        isRunning = false;
        isCorching = false;
        isStaticX = true;
        isLanding = true;

        //数据初始化
        canJump = true;
        canMove = true;
        canRun = true;
        canCorch = true;

        jumpSpeed = 15f;
        moveSpeed = 3f;
        walkSpeed = 3f;
        runSpeed = 6f;
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

        //下蹲操作
        verVal = Input.GetAxis("Vertical");
        if (canCorch && verVal < 0)
            Corch();
        else
            isCorching = false;

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

        if (Input.GetKey(KeyCode.LeftShift) && canRun/*&& isLanding*/)
        {
            if (san <= 3)
                moveSpeed = runSpeed * 4 / 5;
            else
                moveSpeed = runSpeed;
            isRunning = true;
        }
        else
        {
            if (san <= 3)
                moveSpeed = walkSpeed * 4 / 5;
            else
                moveSpeed = walkSpeed;
            isRunning = false;
        }
        rigidbody2.velocity = new Vector2(val * moveSpeed, rigidbody2.velocity.y);

        isStaticX = false;
        isMovingX = true;
        isCorching = false;
        ShowAnimation();
        ShowAudio();
    }

    void StopMovingX()
    {
        rigidbody2.velocity = new Vector2(0, rigidbody2.velocity.y);

        isStaticX = true;
        isMovingX = false;
        isCorching = false;
        ShowAnimation();
        ShowAudio();
    }

    //跳跃
    void Jump()
    {
        rigidbody2.velocity = new Vector2(rigidbody2.velocity.x,jumpSpeed * jumpVal);

        isJumping = true;
        isCorching = false;
        ShowAnimation();
    }
    
    //蹲下
    void Corch()
    {
        isCorching = true;
        ShowAnimation();
    }

    //根据状态修改限制条件
    void AlterByState()
    {

        if (isMovingX)                                   //isMovingX和isStaticX刚好相反可以考虑取消其中一个
        {
            canCorch = false;
        }
        if (isLanding)
        {
            canJump = true;
            canCorch = true;
        }
        if (isJumping)
        {
            canJump = false;                            //不能二段跳
            canCorch = false;
        }
        
        if(isStaticX)
        {
            
        }

        if (isCorching)
        {
            canMove = false;
        }
        else
            canMove = true;
    }

    //检测着地状态
    public void CheckLanding()
    {
        Vector3 origin;         //检测起点
        Vector3 direction;      //检测方向
        LayerMask layerMask;    //检测对象层
        float depth;            //检测深度

        depth = 1.4f;
        origin = transform.position;
        direction = Vector3.down;                       
        layerMask = LayerMask.GetMask("Obstacle");      //对象层为障碍

   
        Debug.DrawRay(origin, depth * direction, Color.red);        //显示射线

        if (Physics2D.Raycast(origin, direction, depth, layerMask))
        {
            isLanding = true;              //着地状态
            isJumping = false;
        }
        else
            isLanding = false;
    }

    public bool CheckOnLanding()
    {
        Vector3 origin;         //检测起点
        Vector3 direction;      //检测方向
        LayerMask layerMask;    //检测对象层
        float depth;            //检测深度

        depth = 1.8f;
        origin = transform.position;
        direction = Vector3.down;
        layerMask = LayerMask.GetMask("Obstacle");      //对象层为障碍


        Debug.DrawRay(origin, depth * direction, Color.red);        //显示射线

        if (Physics2D.Raycast(origin, direction, depth, layerMask))
            return true;
        else
            return false;
            
    }

    //生命值相关
    public void ReduceLife(int n)
    {
        life -= n;
        CheckLife();
    }
    void CheckLife()
    {
        if (life <= 0)
            Debug.Log("GameOver");
    }

    //动画相关
    public void ShowAnimation()
    {
        //0Idle1跳 2跑 3走
        if(isLanding&&isMovingX)
        {
            if (isRunning)
                animator.SetInteger("State", 2);
            else
                animator.SetInteger("State", 3);
        }
        if(isLanding&&isStaticX)
        { 
            animator.SetInteger("State", 0);
        }
        if(isCorching)
        {
            animator.SetInteger("State", 4);
        }
        if(isJumping)
        {
            animator.SetInteger("State", 1);
        }
    }

    //音频相关
    public void ShowAudio()
    {
        

        if (isLanding && isMovingX)
        {
            if (isRunning)
            {
                if (audioIndex != 1)
                {
                    audioIndex = 1;
                    SoundManager.instance.SetEffect(audioIndex);
                    SoundManager.instance.SpeedEffect(1.5f);
                    SoundManager.instance.PlayEffect();
                }
            }
            else
            {
                if (audioIndex != 0)
                {
                    audioIndex = 0;
                    SoundManager.instance.SetEffect(audioIndex);
                    SoundManager.instance.SpeedEffect(1);
                    SoundManager.instance.PlayEffect();
                }
            }
        }

        if(isLanding && isStaticX)
        {
            audioIndex = -1;
            SoundManager.instance.StopEffect();
        }
        if (isJumping)
        {
            if (CheckOnLanding() && rigidbody2.velocity.y<0)
            {
                if (audioIndex != 2)
                {
                    audioIndex = 2;
                    SoundManager.instance.SetEffect(audioIndex);
                    SoundManager.instance.SpeedEffect(1);
                    SoundManager.instance.PlayEffect();
                }
            }
            else
            {
                audioIndex = -1;
                SoundManager.instance.StopEffect();
            }
        }
    }

    //san值相关
    public void ReduceSan()
    {
        san--;
        CheckSan();
    }
    public void AddSan()
    {
        san++;
        if (san > 10)
            san = 10;
        CheckSan();
    }
    public void CheckSan()
    {
        if (san <= 0)
            Debug.Log("GameOver");
    }
}
