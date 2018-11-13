using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour {
	public static DialogueSystem Instance { get; set; }
	public List<string> dialogueLines = new List<string> ();
	public GameObject dialoguePanel;
	//private PlayerMovement movement;

	Text dialogueText;
	int dialogueIndex;

	// Use this for initialization
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			continueDialogue ();
		}
	}

	void Awake () {
		//movement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
		dialogueText = dialoguePanel.transform.Find ("Text").GetComponent<Text>();
		dialoguePanel.SetActive (false);
		if (Instance != null && Instance != this) {
			Destroy (gameObject);
		} 
		else {
			Instance = this;
		}
	}
	
	public void newDialogue (string[] lines) {
		dialogueIndex = 0;
		dialogueLines = new List<string> (lines.Length);
		dialogueLines.AddRange (lines);
		Debug.Log (dialogueLines.Count);
		createDialogue ();
	}

	public void continueDialogue () {
		if (dialogueIndex < dialogueLines.Count - 1) {
			dialogueIndex++;
			dialogueText.text = dialogueLines [dialogueIndex];
		} else {
			dialoguePanel.SetActive (false);
			//movement.canMove = true;
		}
	}

	public void createDialogue () {
		dialogueText.text = dialogueLines [dialogueIndex];
		dialoguePanel.SetActive (true);
		//movement.canMove = false;
	}
}
