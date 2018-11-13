using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlRNG : MonoBehaviour {
	public GameObject option1;
	public GameObject option2;
	public GameObject option3;
	public GameObject option4;
	public int controlOption;
	public float choiceTimeLimit = 5.0f;

	private GameObject[] players;
	private int[] controlOptions = new int[] { 1, 2, 3, 4 };
	private int tempStore;
	private optionPlatform plat1;
	private optionPlatform plat2;
	private optionPlatform plat3;
	private optionPlatform plat4;
	private bool timerDone = false;
	private Text countdownBox;
	private IEnumerator coroutine;
	private GameObject spawnpoint;

	// Use this for initialization
	void Start () {
		countdownBox = GameObject.Find("ChoiceCountdown").GetComponent<Text>();
		//TODO: Replace single spawnpoint with RNG spawnpoints for players.
		spawnpoint = GameObject.Find ("Spawn1");
		countdownBox.text = choiceTimeLimit.ToString();
		randomize ();

		coroutine = countDown (choiceTimeLimit);
		StartCoroutine (coroutine);
	}
	
	// Update is called once per frame
	void Update () {
		players = GameObject.FindGameObjectsWithTag ("Player");
	}

	void randomize() {
		for (int i = 0; i < controlOptions.Length; i++) {
			int rnd = Random.Range (0, controlOptions.Length);
			tempStore = controlOptions [rnd];
			controlOptions [rnd] = controlOptions [i];
			controlOptions [i] = tempStore;
		}

		plat1 = option1.GetComponent<optionPlatform>();
		plat1.assignedOption = controlOptions [0];
		plat2 = option2.GetComponent<optionPlatform>();
		plat2.assignedOption = controlOptions [1];
		plat3 = option3.GetComponent<optionPlatform>();
		plat3.assignedOption = controlOptions [2];
		plat4 = option4.GetComponent<optionPlatform>();
		plat4.assignedOption = controlOptions [3];
	}

	public IEnumerator countDown(float timeLimit) {
		while (true) {
			yield return new WaitForSeconds (1);
			timeLimit -= 1;
			countdownBox.text = timeLimit.ToString();

			if (timeLimit <= 0) {
				Debug.Log ("Why");
				//TODO: Replace with a foreach to manage randomly placing on spawnpoints.
				GameObject.Find("ChoiceCountdown").SetActive(false);
				players [0].gameObject.transform.position = spawnpoint.transform.position;
				StopCoroutine (coroutine);
			}
		}
	}
}
