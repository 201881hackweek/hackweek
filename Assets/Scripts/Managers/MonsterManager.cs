using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour {

    //储存怪物链表和怪物数量，提供产怪和激活方法

    public static MonsterManager instance;
    public List<GameObject> monsters;
    public int maxNum;
    public int num;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        Init();
    }

    public enum Type
    {
        NORMOL
    }

    public void Init()
    {
        monsters = new List<GameObject>();
        maxNum = 15;
        num = 0;
    }

    //添加/删除对象
    public void AddMonster(GameObject monster)
    {
        monsters.Add(monster);
    }
    public void AddMonster(GameObject[] monster)
    {
        monsters.AddRange(monster);
    }
    public void RemoveMonster(GameObject monster)
    {
        monsters.Remove(monster);
    }

    //创建对象
    public void CreatMonster(Vector2 pos)
    {
        if (num < maxNum)
        {
            //创建并添加到链表中
            GameObject monster;
            monster = MonsterPool.instance.SetByPool();
            AddMonster(monster);

            //放置好位置
            monster.transform.position = pos;

            num = monsters.Count;
        }
    }

    //创建全部对象


    public void Active()
    {
        bool isReal = WorldManager.instance.isReal;
        if(isReal)
        {
            foreach(GameObject monster in monsters)
            {
                monster.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject monster in monsters)
            {
                monster.SetActive(true);
            }
        }
    }
}
