using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour {
	public string[] dialogue;
	public GameObject Indicator;

	// Use this for initialization
	void Start () {
		Indicator.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnTriggerEnter() {
		Indicator.SetActive (true);
	}

	void OnTriggerStay () {
		if (Input.GetKeyDown(KeyCode.T)) {
			Interact ();
		}
	}

	void OnTriggerExit () {
		Indicator.SetActive (false);
	}

	public void Interact () {
		DialogueSystem.Instance.newDialogue (dialogue);
	}
}
