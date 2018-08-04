using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{

    /*
         1.chase方法作为最基本的怪物AI，要改成横板的追击。
         添加一个是否追击的bool变量(默认false）该变量由管理器动态管理，该脚本不用动它，只需声明即可;
         2.改进chase方法，新声明chase1,chase2。比如chase1是玩家在怪物正前方时才追击
         可能需要用到的东西:射线检测。判断怪物的面朝向通过transform中的scale.x，1为向右，-1为向左。
         有什么问题尽管问。
         3.总之可以多写几种chase方法，自由发挥吧。
         4.加特殊动作，每个写一个方法。比如跳起。
         5.加判断方法，在update里调用。此方法控制怪物是 chase还是 用特殊动作。
         判断所用的bool变量统一用can前缀，具体参考Player脚本。

         6.自由发挥吧，可以多写一下有趣的方法，我最后把他们整理。
        
         一下是一些建议:
         1.怪物移动应该只有向左或者向右，(最基本的完成后可以加点其他方向的移动）
         所以在玩家相对位置被判断出来后，应该判断向左还是向右移动，在调用Move(Vector3.right/left);
         2.随便问我。
    */

    // 状态
    public int life;
    public int damage;
    public int speed;
    public float scaleX;

    // 追击
    public GameObject target; // 玩家对象
    public Vector3 chaseDir; //移动方向
    public bool canSee; // 检测发现角色
    public bool canJump;
    public bool canRush;
    public bool inEdge;
    public bool inShelter;         //检测怪物视线遮挡
    public bool inVirsion;         //检测怪物视角
    public float virsionResult;

    //public bool CheckSee(Vector3 distance);
    //public void ChaseAndAttack();
    //public void Patrol();
    //public void Move(Vector3 dir, float currentSpeed);
    //public bool CheckEdge();
    //public void Attack();
    //public void ChaseNormal();

    // Use this for initialization
    void OnEnable()
    {
        scaleX = transform.localScale.x;
        target = GameObject.Find("Player");
        //所有数值的初始化放到Init()方法中，在这个脚本中Init()方法是abstract类型，不用写内容
        //不同类型的怪物继承这个脚本类再改写(override)Init()方法，从而在初始化时有不同的life,speed等;
        Init();
    }

    public void Init()
    {
        life = 2;
        damage = 1;
        speed = 1;
    }

    void Update()
    {
        // 怪物状态控制；检查是否看见角色
        canSee = CheckSee();
        // 激活状态，追击或攻击角色
        if (canSee)
        {
            ChaseAndAttack();
        }
        // 待机状态，巡逻
        else
        {
            Patrol();
        }
    }

    //视线判断方法：（2级方法）检测怪物是否看见了玩家
    public bool CheckSee()//对角色的距离向量
    {
        Vector3 origin;
        Vector3 distance;
        LayerMask layerMask;    //检测对象层

        origin = transform.position;
        distance = target.transform.position - gameObject.transform.position;
        //（看完后可以删掉）检测的是平台是否遮挡了视线，应该是同一个对象层，所以没有改
        layerMask = LayerMask.GetMask("Obstacle");
        Debug.DrawRay(origin, distance, Color.red);
        //此射线由怪物指向玩家，途中若碰到障碍则返回ture;
        inShelter = Physics2D.Raycast(origin, distance, distance.magnitude, layerMask);


        // 怪物面向与视线在同一侧，乘积必然为正
        virsionResult = (distance.x / distance.magnitude) * transform.localScale.x/scaleX;
        inVirsion = (virsionResult >= 0.5 && virsionResult <= 1);
        if (inShelter || !inVirsion)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    // 追击控制方法：（2级方法）控制各种追击模式	
    public void ChaseAndAttack()
    {
        ChaseNormal();
    }

    // 巡逻控制方法：（2级方法）规定巡逻时的行为
    public void Patrol()
    {
        float chaseSpeed;
        inEdge = CheckEdge(); //判断是否处于边缘

        chaseSpeed = speed;

        if (inEdge)
        {
            if (transform.localScale.x > 0)
            {
                // 这里需要变更怪物的面向
                transform.localScale = new Vector3(-scaleX, transform.localScale.y, transform.localScale.z);
                chaseDir = Vector3.left; //向反方向移动
                Move(chaseDir, chaseSpeed);
            }
            else
            {
                // 这里也需要变更怪物的面向
                transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
                chaseDir = Vector3.right;
                Move(chaseDir, chaseSpeed);
            }
        }
        else
        {
            if (transform.localScale.x > 0)
            {
                chaseDir = Vector3.right; //面向移动
                Move(chaseDir, chaseSpeed);
            }
            else
            {
                chaseDir = Vector3.left;
                Move(chaseDir, chaseSpeed);
            }
        }
        int patrolSpeed = speed * 7 / 10;
        chaseDir.Normalize();          //方向向量应单位化
        Move(chaseDir, patrolSpeed);
    }

    //移动实现方法：（3级方法）
    public void Move(Vector3 dir, float currentSpeed)//增强方法的通用性，Move方法用于控制物体向dir方向移动。
    {
        gameObject.transform.Translate(dir * currentSpeed * Time.deltaTime);
    }

    //边缘判断方法：（3级方法）检测怪物是否到达了所在层的边缘
    public bool CheckEdge()
    {
        Vector3 origin;         //检测起点
        Vector3 direction = new Vector3(transform.localScale.x, -1, 0).normalized; // 面向俯角45度
        LayerMask layerMask;    //检测对象层
        float depth;            //检测深度

        depth = 3f;
        origin = transform.position;
        layerMask = LayerMask.GetMask("Obstacle");      //对象层为障碍


        Debug.DrawRay(origin, depth * direction, Color.red);        //显示射线

        return !Physics2D.Raycast(origin, direction, depth, layerMask);
    }

    //攻击方法：（）（占位）
    //public void Attack();

    //追逐-常规方法：(3级方法)玩家在哪边就往哪边追
    public void ChaseNormal()
    {
        int chaseSpeed = speed;
        

        if (target.transform.position.x - gameObject.transform.position.x > 0)
        {
            chaseDir = Vector3.right;
        }
        else
        {
            chaseDir = Vector3.left;
        }
        chaseDir.Normalize();          //方向向量应单位化
        Move(chaseDir, chaseSpeed);
    }
}
