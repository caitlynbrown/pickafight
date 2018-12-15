using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button2 : MonoBehaviour {
	//Game Object that the button interacts with.
	public GameObject wall;
	//What height to move the button when pressed.
	public float downHeight;

	private bool triggered;
	private bool first;

	// Use this for initialization
	void Start () {
		triggered = false;
		first = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (triggered) {
			wall.transform.Translate (0, 4, 0);
			triggered = false;
			}
	}

	void OnTriggerEnter (Collider col) {
		if (!triggered && !first) {
			gameObject.transform.Translate (0, downHeight, 0);
			triggered = true;
			first = true;
		}
	}
}
