using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class ControlRNG : NetworkBehaviour {
	public GameObject option1;
	public GameObject option2;
	public GameObject option3;
	public GameObject option4;
	public int controlOption;
	public float choiceTimeLimit = 5.0f;
	public float gameTimeLimit = 180.0f;

	private GameObject[] players;
	private int[] controlOptions = new int[] { 1, 2, 3, 4 };
	private List<int> playerScores = new List<int> ();
	private int tempStore;
	private optionPlatform plat1;
	private optionPlatform plat2;
	private optionPlatform plat3;
	private optionPlatform plat4;
	private bool timerDone = false;
	private Text countdownBox;
	private IEnumerator coroutine;
	private IEnumerator gameTimer;
	private GameObject spawnpoint;
	private PlayerMovement movement;
	private GameObject specButton;
	private GameObject winPopup;
	private GameObject losePopup;
	//private GameObject deathUI;
	//private Canvas healthUI;
	private bool setScript;

	// Use this for initialization
	void Start () {
		countdownBox = GameObject.Find("ChoiceCountdown").GetComponent<Text>();
		//TODO: Replace spawns with an RNG of various locations.
		spawnpoint = GameObject.Find ("Spawn1");
		specButton = GameObject.Find ("SpectateButton");
		winPopup = GameObject.Find ("WinPopup");
		losePopup = GameObject.Find ("LosePopup");
		//deathUI = GameObject.Find ("DeathCount");
		//healthUI = GameObject.Find ("HealthCanvas").GetComponent<Canvas> ();
		countdownBox.text = choiceTimeLimit.ToString();
		randomize ();

		coroutine = countDown (choiceTimeLimit);
		gameTimer = gameCountDown (gameTimeLimit);
		winPopup.SetActive (false);
		losePopup.SetActive (false);
		StartCoroutine (coroutine);

		//deathUI.SetActive (false);
		//healthUI.enabled = false;
		setScript = false;

	}
	
	// Update is called once per frame
	void Update () {
		players = GameObject.FindGameObjectsWithTag ("Player");

		if (!setScript) {
			foreach (GameObject p in players) {
				if (p.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer) {
					movement = p.gameObject.GetComponent<PlayerMovement> ();
					setScript = true;
				}
			}
		}
	}

	public void randomize() {
		for (int i = 0; i < controlOptions.Length; i++) {
			int rnd = UnityEngine.Random.Range (0, controlOptions.Length);
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

				foreach (GameObject p in players) {
					if (p.gameObject.GetComponent<NetworkIdentity> ().isLocalPlayer) {
						p.gameObject.GetComponent<PlayerMovement> ().gameStart = true;

						if (p.gameObject.GetComponent<PlayerMovement> ().isSpectator == false) {
							movement.updateUI ();
							specButton.SetActive (false);
							//deathUI.SetActive (true);
							//healthUI.enabled = true;
						}
					}
					p.gameObject.transform.position = spawnpoint.transform.position;
				}
				countdownBox.text = gameTimeLimit.ToString();
				StartCoroutine (gameTimer);
				StopCoroutine (coroutine);
			}

		}
	}

	public IEnumerator gameCountDown(float timeLimit) {
		while (true) {
			yield return new WaitForSeconds (1);
			timeLimit -= 1;
			countdownBox.text = timeLimit.ToString();

			if (timeLimit <= 0) {
				countdownBox.enabled = false;
				int i = 0;
				foreach (GameObject p in players) {
					playerScores.Add(p.gameObject.GetComponent<Health> ().deathCount);

				}
				playerScores.Sort();
				Debug.Log (playerScores[0]);
				foreach (GameObject p in players) {
					if (p.gameObject.GetComponent<NetworkIdentity> ().isLocalPlayer) {
						p.gameObject.GetComponent<PlayerMovement> ().canMove = false;

						if (p.gameObject.GetComponent<Health> ().deathCount == playerScores [0]) {
							winPopup.SetActive (true);
						} else {
							losePopup.SetActive (true);
						}
					}

					StopCoroutine (gameTimer);

				}
			}
		}
	}
}

