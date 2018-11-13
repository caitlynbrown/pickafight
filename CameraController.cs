using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	private Transform playerTransform;
	public int offset = -10;


	void Update () {
		if (playerTransform != null) {
			transform.position = playerTransform.position + new Vector3 (0, 2, offset);
			transform.rotation = playerTransform.rotation;
		}
	}

	public void setTarget(Transform target) {
		playerTransform = target;
	}
}
