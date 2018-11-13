using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Telepads : MonoBehaviour {
	public GameObject telepad;

	private Vector3 trans;

	void Start() {
		trans = telepad.transform.position;
	}

	void OnTriggerEnter (Collider col) {
		Debug.Log("Hit");
		col.transform.position = trans;
	}
}
