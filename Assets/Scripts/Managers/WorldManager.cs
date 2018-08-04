using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour {

    //负责里表世界切换，管理san值和产怪

    public static WorldManager instance;

    public bool isReal;         //判断所在世界
    float timer;                //计时器
    float creatVal;             //产怪乱数

    //产怪位置相关
    public Vector3 creatPos;
    public Vector3 playerPos;
    public float r1;
    public float r2;

    public List<Transform> positions;             //固定产怪点,在场景内拖
    public List<Transform> positions2;
    public List<Transform> readyPos;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        isReal = true;
        timer = 0;
        r1 = 2;                                     //产怪点内半径
        r2 = 20;                                    //外半径
    }

    void Update () {

        if (Input.GetKeyDown(KeyCode.X))
            ChangeWorld();                  //切换世界
   
        timer += Time.deltaTime;
        if (timer >= 1)        
        {
              timer = 0;
              ReadyCreat();        //产怪操作
              CheckNum();         //检测怪物数量，根据结果减少san值
        }
	}
    
    public void ChangeWorld()
    {
        if (isReal)
        {
            isReal = false;
        }
        else
            isReal = true;
        MonsterManager.instance.SendMessage("Active");      //切换至里世界讲怪物激活，表世界怪物失效
    }

    //产怪相关
    public void SetCreatVal()                   //产生乱数
    {
        creatVal = Random.Range(0, 100);
    }

    public void SetCreatPos()
    {
        readyPos = new List<Transform>();
        readyPos.Clear();
        playerPos = Player.instance.transform.position;

        foreach (Transform pos in positions)            //对于每一个固定产怪点，若存在范围内，将其加入临时链表
        {
            if( Mathf.Abs((pos.position - playerPos).magnitude)>r1&&             
                Mathf.Abs((pos.position - playerPos).magnitude)<r2)
            {
                readyPos.Add(pos);
            }
        }
        if (readyPos.Count > 0)
        {
            creatPos = readyPos[Random.Range(0, readyPos.Count - 1)].position;          //随机选择临时链表中的位置对象
        }
        else
            creatPos = new Vector3(0, 0, 0);
    }
    public void ReadyCreat()
    {
        if(isReal)      //表世界产怪
        {
            SetCreatVal();
            if (creatVal < 80)               
            {
                MonsterManager.instance.num++;
                /*
                SetCreatPos();
                MonsterManager.instance.CreatMonster(creatPos);
                */
            }
        }
        else            //里世界产怪
        {
            MonsterManager.instance.num++;
            //MonsterManager.instance.CreatMonster(creatPos);
            {
                SetCreatVal();
                if(creatVal<50)
                {
                    MonsterManager.instance.num++;
                    /*
                    SetCreatPos();
                    MonsterManager.instance.CreatMonster(creatPos);
                    MonsterManager.instance.SendMessage("Active");
                    */
                }
            }
        }
    }

    public void CheckNum()          //控制san值
    {
        if (isReal)
        {
            if (MonsterManager.instance.num > 10)
                Player.instance.SendMessage("ReduceSan");
            else
                Player.instance.SendMessage("AddSan");
        }
    }
}
