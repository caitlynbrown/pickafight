using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMove : MonoBehaviour {
	public Transform platform;
	public Transform position1;
	public Transform position2;
	public Vector3 newPosition;
	public float moveTime;
	public float loopTime;

	public string state;

	// Use this for initialization
	void Start () {
		ChangeTarget ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		platform.position = Vector3.Lerp (platform.position, newPosition, moveTime * Time.deltaTime);
	}

	void ChangeTarget() {
		if (state == "To Position 1") {
			state = "To Position 2";
			newPosition = position2.position;
		}
		else if (state == "To Position 2") {
			state = "To Position 1";
			newPosition = position1.position;
		}
		else if (state == "") {
			state = "To Position 2";
			newPosition = position2.position;
		}

		Invoke ("ChangeTarget", loopTime);
	}

}
