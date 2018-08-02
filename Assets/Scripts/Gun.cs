using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

    public Vector3 hitPos;
    public Vector2 hitDir;
    public Camera mainCamera;
    public GameObject bullet;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        //计算瞄准位置
        hitPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        hitDir = hitPos-transform.position;
        hitDir.Normalize();

        //右键开始瞄准
        if (Input.GetMouseButtonDown(1))
            StartCoroutine("Shot");


    }
 

    IEnumerator Shot()
    {
        Vector2 dir, realDir;                           //目标瞄准位置和实际瞄准位置
        float angle, realAngle, timer, bulletSpeed;     //最大范围误差角，实际误差角以及瞄准精确所需最小时间，子弹速度
        int error;                                      //最大范围误差参数 越小 误差范围越大

        timer = 1f;
        bulletSpeed = 10f;
        dir = hitDir;
        error = 5;
       
        while (true)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
                timer = 0.1f;
            angle = timer * Mathf.PI / error;
            realAngle = Random.Range(-angle, angle);        //获取误差范围内的最终偏差角
            realDir = Quaternion.AngleAxis(realAngle* (180 / Mathf.PI), Vector3.forward) * dir;       //四元数 不懂

            Debug.DrawRay(transform.position, dir * bulletSpeed, Color.red);
            Debug.DrawRay(transform.position, realDir * bulletSpeed, Color.green);

            if (Input.GetMouseButtonDown(0))
            {    
                //发射
                ShotIt(realDir,bulletSpeed);
                break;
            }
            if(Input.GetMouseButtonUp(1))
            {
                //结束瞄准
                break;
            }

            yield return null;
        }
    }
    
    public void ShotIt(Vector2 dir,float bulletSpeed)
    {
        bullet = Pool.poolInstance.SetByPool();
        bullet.transform.position = transform.position;
        bullet.SetActive(true);
        bullet.GetComponent<Rigidbody2D>().AddForce(bulletSpeed * dir, ForceMode2D.Impulse);
    }
}
