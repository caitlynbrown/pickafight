using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.VR;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : NetworkBehaviour {
	public float speed = 3.0f;
	public float rotateSpeed = 3.0f;
	public float jumpSpeed = 3.0f;
	public float gravity = 13.0f;
	public int moveTimeLimit = 5;
	public int moveCooldown = 30;
	public int controlInt;
	public bool canMove;
	public Animator characterMovements;
	public GameObject block;
	public Transform blockSpawn;
	public bool gameStart;
	public bool isSpectator;


	private Button specButton;
	private CharacterController controller;
	private Renderer renderer;
	private Vector3 moveDirection = Vector3.zero;
	private Vector3 position;
	private Text powerup;
    private bool candoublejump = true;
	private bool canPunch;
	private IEnumerator coroutine;
	private IEnumerator phaseRoutine;
	private bool phaseHandled;
	private Camera spectateCam;
	private GameObject VRspectateCam;
	private GameObject mainCam;
	//private GameObject UIpowerup;
	//private GameObject UIDeathcount;
	//private GameObject UIspecbutton;
	private Canvas UIcanv;
	private Canvas UIhealth;
	private GameObject[] players;
	private GameObject[] walls;
	private GameObject[] bullets;

	[SyncVar]
	public bool isInv;

	[SyncVar]
	public bool isPhase;


	void Start () {
		controller = GetComponent<CharacterController> ();
		characterMovements = GetComponent<Animator> ();
		renderer = GetComponent<Renderer>();
		powerup = GameObject.Find ("Powerup").GetComponent<Text>();

		canMove = true;
		candoublejump = true;
		canPunch = true;
		isInv = false;
		isPhase = false;
		phaseHandled = false;
		gameStart = false;
		isSpectator = false;

		coroutine = moveLast (moveTimeLimit);
		phaseRoutine = phaseMove (moveTimeLimit);

		walls = GameObject.FindGameObjectsWithTag ("Wall");
		specButton = GameObject.Find ("SpectateButton").GetComponent<Button>();
		spectateCam = GameObject.Find ("SpectateCamera").GetComponent<Camera>();
		VRspectateCam = GameObject.Find ("OVRCameraRig");
		mainCam = GameObject.Find ("Main Camera");
		//UIpowerup = GameObject.Find ("Powerup");
		//UIDeathcount = GameObject.Find ("DeathCount");
		//UIspecbutton = GameObject.Find ("SpectateButton");
		UIhealth = GameObject.Find ("HealthCanvas").GetComponent<Canvas> ();
		UIcanv = GameObject.Find ("Canvas").GetComponent<Canvas> ();

		VRspectateCam.SetActive (false);

	}

	//DOUBLE JUMP = 1, BLOCK PUNCH = 2, INVISIBLE = 3, PHASE = 4

	void Update () {

		if (!isLocalPlayer) {
			return;
		}

		if (isSpectator) {
			//spectateCam.enabled = true;
			VRspectateCam.SetActive(true);
			mainCam.SetActive (false);
			StartCoroutine (LoadDevice ("Oculus"));

			//UIpowerup.SetActive (false);
			//UIDeathcount.SetActive (false);
			//UIhealth.enabled = false;
			UIhealth.enabled = false;
			UIcanv.enabled = false;
			specButton.enabled = false;

			CmdDestoryPlayer (this.gameObject);
		}

		specButton.onClick.AddListener (OnSpecClick);

		players = GameObject.FindGameObjectsWithTag ("Player");

		foreach (GameObject p in players) {
			//Debug.Log ("I'm being called " + this.gameObject);
			if (p.gameObject.GetComponent<NetworkIdentity> ().isLocalPlayer == false) {
				if (p.gameObject.GetComponent<PlayerMovement> ().isInv == true) {
					p.gameObject.GetComponent<SkinnedMeshRenderer> ().enabled = false; 
				} else if (p.gameObject.GetComponent<PlayerMovement> ().isInv == false) {
					p.gameObject.GetComponent<SkinnedMeshRenderer> ().enabled = true;
				}
				else if (p.gameObject.GetComponent<PlayerMovement> ().isPhase == true) {
					if (!phaseHandled) {
						p.gameObject.GetComponent<SkinnedMeshRenderer> ().material.color -= new Color (0, 0, 0, .7f);
						phaseHandled = true;
					}
				} else if (p.gameObject.GetComponent<PlayerMovement> ().isPhase == false) {
					if (phaseHandled) {
						p.gameObject.GetComponent<SkinnedMeshRenderer> ().material.color += new Color (0, 0, 0, .7f);
						phaseHandled = false;
					}
				}
			}
		}

		bullets = GameObject.FindGameObjectsWithTag ("Bullet");
		position = transform.position;
		float mouseInput = Input.GetAxis("Mouse X");

		if (canMove) {
			float pushSpeed = Input.GetAxis ("Vertical") * speed * Time.deltaTime;
			Vector3 lookhere = new Vector3(0,mouseInput,0);
			Quaternion rotationDirection = Quaternion.Euler (0f, lookhere.y * rotateSpeed * Time.deltaTime, 0f);
			transform.rotation *= rotationDirection;
			transform.position += pushSpeed * transform.forward;

			moveDirection.y -= gravity * Time.deltaTime;
			controller.Move(moveDirection * Time.deltaTime);

			movementListen ();


			if (controller.isGrounded) {
				characterMovements.SetBool ("isJumping", false);
			} else if (!controller.isGrounded) {
				characterMovements.SetBool ("isJumping", true);
			}

			if (Input.GetButtonUp ("Jump")) {
				jumpListen ();
			}

			//TODO: Handle punching and such once networking is in place
			else if (Input.GetMouseButtonDown (0)) {
				if (gameStart) {
					doPunch ();
				}
			} 

			else if (Input.GetMouseButtonDown (1)) {
				if (gameStart) {
					handleExtra ();
				}
			} 
    	}
			
	}

	void movementListen() {
		if (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.S) || Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.D)) {
			characterMovements.SetBool ("isMoving", true);
			characterMovements.SetBool ("isPunching", false);
			if (controller.isGrounded) {
				characterMovements.SetBool ("isJumping", false);
			}
		} 
		else if (moveDirection.y <= 0) {
			//characterMovements.SetBool ("isJumping", false);
			characterMovements.SetBool ("isMoving", false);
			characterMovements.SetBool ("isPunching", false);
			candoublejump = true;
		}
		else {
			characterMovements.SetBool ("isJumping", false);
			characterMovements.SetBool ("isMoving", false);
			characterMovements.SetBool ("isPunching", false);
		}
	}

	void jumpListen() {
			if (controller.isGrounded) { 
				characterMovements.SetBool ("isJumping", true);
				characterMovements.SetBool ("isPunching", false);
				moveDirection.y = jumpSpeed;
			}

		//Control int of 1 indicates double jump
		else if (controller.isGrounded == false && candoublejump && controlInt == 1) {
				moveDirection.y = jumpSpeed;
				candoublejump = false;
			}
		
	}

	//TODO: Handle punching and such once networking is in place
	void doPunch() {
		if (characterMovements.GetBool ("isPunching") == false) {
			characterMovements.SetBool ("isPunching", true);

		}
	}

	public void updateUI() {
		if (controlInt == 1) {
			powerup.text = "Double Jump";
		} 

		else if (controlInt == 2) {
			powerup.text = "Block Buff";
		}

		else if (controlInt == 3) {
			powerup.text = "Invisibility";
		} 

		else if (controlInt == 4) {
			powerup.text = "Phaseing";
		}
	}

	void handleExtra() {
		//BLOCK PUNCH = 2, INVISIBLE = 3, PHASE = 4
		if (controlInt == 2) {
			CmdFire ();
		} 

		else if (controlInt == 3) {
			//this.gameObject.GetComponent<SkinnedMeshRenderer> ().enabled = false;
			if (!isInv) {
				this.gameObject.GetComponent<SkinnedMeshRenderer> ().material.color -= new Color (0, 0, 0, .7f);
				StartCoroutine (coroutine);
			}

		} 

		else if (controlInt == 4) {
			Debug.Log ("Phase");

			foreach (GameObject w in walls) {
				var colBoxes = w.gameObject.GetComponents<BoxCollider> ();
				foreach (BoxCollider b in colBoxes) {
					b.enabled = false;
				}
			}

			foreach (GameObject b in bullets) {
				b.gameObject.GetComponent<BoxCollider> ().enabled = false;
			}

			if (!isPhase) {
				this.gameObject.GetComponent<SkinnedMeshRenderer> ().material.color -= new Color (0, 0, 0, .7f);
				StartCoroutine (phaseRoutine);
			}
		}
	}

	public override void OnStartLocalPlayer() {
		Camera.main.GetComponent<CameraController> ().setTarget (gameObject.transform);
	}

	public IEnumerator moveLast(int timeLimit) {
		while (true) {
			yield return new WaitForSeconds (1);
			timeLimit -= 1;

			if (timeLimit <= 0) {
				this.gameObject.GetComponent<SkinnedMeshRenderer> ().material.color += new Color (0, 0, 0, .7f);
				isInv = false;
				StopCoroutine (coroutine);
			} else {
				isInv = true;
			}
		}
	}

	public IEnumerator phaseMove(int timeLimit) {
		bool hasChanged = false;

		while (true) {
			yield return new WaitForSeconds (1);
			timeLimit -= 1;

			if (timeLimit <= 0) {
				if (!hasChanged) {
					this.gameObject.GetComponent<SkinnedMeshRenderer> ().material.color += new Color (0, 0, 0, .7f);
					hasChanged = true;
				}

				foreach (GameObject w in walls) {
					var colBoxes = w.gameObject.GetComponents<BoxCollider> ();
					foreach (BoxCollider b in colBoxes) {
						b.enabled = true;
					}
				}

				foreach (GameObject b in bullets) {
					b.gameObject.GetComponent<BoxCollider> ().enabled = true;
				}

				isPhase = false;
				StopCoroutine (coroutine);
			} else {
				isPhase = true;
			}
		}
	}

	[Command]
	void CmdFire() {
		var bullet = (GameObject)Instantiate (
			block,
			blockSpawn.position,
			blockSpawn.rotation);

		bullet.GetComponent<MeshRenderer> ().material.color = this.gameObject.GetComponent<SkinnedMeshRenderer> ().material.color;
		bullet.GetComponent<Rigidbody> ().velocity = bullet.transform.forward * 3;

		NetworkServer.Spawn (bullet);

		Destroy (bullet, 5.0f);
	}

	void OnTriggerStay(Collider col) {
		var player = col.gameObject;
		DamagePlayers (player);
	}
		
	void DamagePlayers(GameObject col) {
		if (isLocalPlayer) {
			if (col.gameObject.tag == "Player" && Input.GetMouseButtonDown (0) == true) {
				if (col.gameObject.GetComponent<NetworkIdentity> ().isLocalPlayer == false) {
					col.gameObject.GetComponent<Health> ().TakeDamage (20);
					Debug.Log ("Hit");
				}
			}
		}
	}

	[Command]
	private void CmdDestoryPlayer(GameObject player) {
		NetworkServer.Destroy (player);
	}

	void OnSpecClick() {
		if (isLocalPlayer) {
			isSpectator = true;
		}
	}

	IEnumerator LoadDevice(string newDevice) {
		if (string.Compare (VRSettings.loadedDeviceName, newDevice, true) != 0) {
			VRSettings.LoadDeviceByName (newDevice);
			yield return null;
			VRSettings.enabled = true;
		}
	}
		
}