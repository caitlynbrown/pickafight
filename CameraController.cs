using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	private Vector3 offset;
	private Transform playerTransform;
	private Space offsetPositionSpace = Space.Self;
	private bool lookAt = true;

	void Start() {
		offset = new Vector3 (0, 5, -10);
		transform.rotation = Quaternion.Euler (0, 270, 0);
	}

	void Update () {
		if (playerTransform != null) {
			camChange();
		}
	}

	public void setTarget(Transform target) {
		playerTransform = target;
	}

	public void camChange() {
		//Position
		if (offsetPositionSpace == Space.Self) {
			transform.position = playerTransform.TransformPoint (offset);
		} 
		else {
			transform.position = playerTransform.position + offset;
		}

		//Rotation
		if (lookAt) {
			transform.LookAt (playerTransform.position + new Vector3(0, 1, 0));
		} 
		else {
			transform.rotation = playerTransform.rotation * Quaternion.Euler (0, 270, 0);
		}
	}
}
