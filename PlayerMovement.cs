using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : NetworkBehaviour {
	public float speed = 3.0f;
	public float rotateSpeed = 3.0f;
	public float jumpSpeed = 3.0f;
	public float gravity = 13.0f;
	public int controlInt;
	public bool canMove;
	public Animator characterMovements;

	private CharacterController controller;
	private Renderer renderer;
	private Vector3 moveDirection = Vector3.zero;
	private Vector3 position;
    private bool candoublejump = true;

	//DEBUGGING VARIABLES KILL THESE EVENTUALLY
	private int punchCount = 0;

	void Start () {
		controller = GetComponent<CharacterController> ();
		characterMovements = GetComponent<Animator> ();
		renderer = GetComponent<Renderer>();

		canMove = true;
		candoublejump = true;
	}

	void Update () {

		if (!isLocalPlayer) {
			return;
		}

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

			if (Input.GetButtonUp ("Jump")) {
				jumpListen ();
			}

			//TODO: Handle punching and such once networking is in place
			else if (Input.GetMouseButtonDown (0)) {
				doPunch ();
			} 

			//else if (Input.GetMouseButtonDown (1)) {
				
			//} 
    	}
	}

	void movementListen() {
		if (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.S) || Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.D)) {
			characterMovements.SetBool ("isMoving", true);
			characterMovements.SetBool ("isJumping", false);
		} 
		else if (moveDirection.y <= 0) {
			characterMovements.SetBool ("isJumping", false);
			characterMovements.SetBool ("isMoving", false);
			candoublejump = true;
		}
		else {
			characterMovements.SetBool ("isMoving", false);
		}
	}

	void jumpListen() {
			if (controller.isGrounded) { 
				characterMovements.SetBool ("isJumping", true);
				moveDirection.y = jumpSpeed;
				Debug.Log (candoublejump);
			}

			else if (controller.isGrounded == false && candoublejump) {
				moveDirection.y = jumpSpeed;
				candoublejump = false;
				Debug.Log ("d jumped");
			}
		
	}

	//TODO: Handle punching and such once networking is in place
	void doPunch() {
		punchCount++;
		Debug.Log (punchCount);
	}

	void handleControls(bool choiceTimeUp) {

	}

	public override void OnStartLocalPlayer() {
		Camera.main.GetComponent<CameraController> ().setTarget (gameObject.transform);
	}
}
		
