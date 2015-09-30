using UnityEngine;
using System.Collections;

public class Powerup : MonoBehaviour {

	public string statName = "speed";
	public float duration = 5;

	private float startTime = 0;

	void OnTriggerEnter2D (Collider2D collider) {
		if (collider.gameObject.tag == "Player") {
			Activate(collider.gameObject);
		}
	}

	void OnCollisionEnter2D (Collision2D collision) {
		if (collision.gameObject.tag == "Player") {
			Activate(collision.gameObject);
		}
	}

	void Activate(GameObject player) {
		startTime = Time.time;
		gameObject.SetActive(false);
		PlayerController p = player.GetComponent<PlayerController>();
		p.AddPowerup(this);
	}

	public bool IsExpired () {
		return (Time.time - startTime > duration);
	}
}
