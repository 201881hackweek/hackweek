using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseScript : MonoBehaviour {
	private int speed;
	private GameObject targ;
	private Vector3 chase;

	// Use this for initialization
	void Start () {
		targ = GameObject.Find("player");
		// 得到怪物的速度，接口的原因需要改写？
		speed = 2;	
	}
	
	// Update is called once per frame
	void Update () {
		// 追逐玩家，暂时没有考虑障碍物
	    chase = targ.transform.position - gameObject.transform.position;
		gameObject.transform.Translate(chase*speed*Time.deltaTime);
	}
}
