using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bullet : MonoBehaviour {

	void OnCollisionEnter(Collision col) {

		var hit = col.gameObject;
		var health = hit.gameObject.GetComponent<Health> ();
		if (health != null) {
			health.TakeDamage (10);
		}
		Destroy (gameObject);
	}
}
