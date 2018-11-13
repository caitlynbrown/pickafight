using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class platformInteract : MonoBehaviour {

	void OnTriggerEnter(Collider col) {
		Debug.Log ("Hit");
		col.transform.parent = gameObject.transform;
	}
		
    void OnTriggerExit(Collider col)
    {
		col.transform.parent = null;
    }
    
}
