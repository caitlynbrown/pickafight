using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class optionPlatform : MonoBehaviour {
	public int assignedOption;

	private PlayerMovement movement;

	// Use this for initialization
	void Start () {
		//movement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
		
	private void OnTriggerEnter (Collider col) {
		if (col.gameObject.tag == "Player") {
			Debug.Log (assignedOption);
			movement = col.gameObject.GetComponent<PlayerMovement> ();
			movement.controlInt = assignedOption;
			col.gameObject.GetComponent<SkinnedMeshRenderer> ().material.color = this.gameObject.GetComponent<MeshRenderer>().material.color;
			//Debug.Log (col.gameObject.GetComponent<MeshRenderer> ().material.color);
		}
	}
}
