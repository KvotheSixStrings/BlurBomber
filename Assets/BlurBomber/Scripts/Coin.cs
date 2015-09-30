using UnityEngine;
using System.Collections;
using Paraphernalia.Components;

public class Coin : MonoBehaviour {

	void OnTriggerEnter2D (Collider2D collider) {
		if (collider.gameObject.tag == "Player") {
			Pickup(collider.gameObject);
		}
	}

	void OnCollisionEnter2D (Collision2D collision) {
		if (collision.gameObject.tag == "Player") {
			Pickup(collision.gameObject);
		}
	}

	void Pickup(GameObject player) {
		gameObject.SetActive(false);
		AudioManager.PlayEffect("coin");
		PlayerController p = player.GetComponent<PlayerController>();
		p.AddCoin(this);
	}
}
