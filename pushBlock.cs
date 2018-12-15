using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pushBlock : MonoBehaviour {
	public float force = 6;

	//TODO: Add a color change or something to show the block is being manipulated?
	private void OnTriggerStay(Collider col) {
		if (col.gameObject.tag == "Player") {
			Vector3 dir = -col.gameObject.transform.position;

			if (Input.GetMouseButtonUp (1) && col.gameObject.GetComponent<PlayerMovement>().controlInt == 2) {
				GetComponent<Rigidbody> ().AddForce (dir * force);
			} 

			else {
				//GetComponent<Rigidbody> ().Sleep();
			}

		}
	}
}
