using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Health : NetworkBehaviour {

	public const int maxHealth = 200;

	[SyncVar(hook = "OnChangeHealth")]
	public int currentHealth = maxHealth;

	[SyncVar(hook="OnLifeChange")]
	public int deathCount;

	public RectTransform healthBar;
	public Text deathCounter;

	private GameObject spawnPoint;

	void Start() {
		healthBar = GameObject.Find ("Foreground").GetComponent<RectTransform> ();
		spawnPoint = GameObject.Find ("Spawn1");
		deathCounter = GameObject.Find ("DeathCount").GetComponent<Text> ();
		deathCount = 0;
		deathCounter.text = deathCount.ToString ();
	}

	public void TakeDamage(int dmg) {
		if (!isServer) {
			return;
		}

		currentHealth -= dmg;

		if (currentHealth <= 0) {
			currentHealth = maxHealth;
			deathCount++;

			RpcRespawn ();
		}
			
	}

	void OnChangeHealth(int health) {
		if (isLocalPlayer) {
			Debug.Log (health);
			healthBar.sizeDelta = new Vector2 (health * 2, healthBar.sizeDelta.y);
		}
	}

	void OnLifeChange(int deathCount) {
		if (isLocalPlayer) {
			deathCounter.text = deathCount.ToString ();
		}
	}

	[ClientRpc]
	void RpcRespawn() {
		if (isLocalPlayer) {
			transform.position = spawnPoint.transform.position;
		}
	}
}
