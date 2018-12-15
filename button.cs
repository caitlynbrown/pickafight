using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class button : MonoBehaviour {
	//Game Object that the button interacts with.
	public GameObject step;
	//What height to move the button when pressed.
	public float downHeight;
	//How long the button's effect will last.
	public float timeLimit;

	//Sees if the button is pressed or not.
	private bool triggered;
	//How much time has passed since the button press.
	private float timePassed;

	void Start () {
		//Initial settings. Button isn't pressed and the step isn't there.
		step.SetActive (false);
		triggered = false;
	}

	void Update () {
		//If the button is pressed, a timer will start.
		if (triggered) {
			timePassed += Time.deltaTime;
			//When the timer exceeds the time limit, the effects of the button press will reverse.
			if (timePassed > timeLimit) {
				triggered = false;
				step.SetActive (false);
				gameObject.transform.Translate (0, -downHeight, 0);
			}
			//If the button hasn't been pressed, the timer resets.
		} else {
			timePassed = 0;
		}
	}
	//When stepping into the trigger, the button will be pressed.
	void OnTriggerEnter (Collider col) {
		if (!triggered) {
			step.SetActive (true);
			gameObject.transform.Translate (0, downHeight, 0);
			triggered = true;
		}
	}
}
