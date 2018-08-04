using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    GameObject player;

	void Start () {

        player = GameObject.Find("Player");

	}
	
	// Update is called once per frame
	void LateUpdate () {

        transform.position = player.transform.position-10*Vector3.forward;
	}
}
