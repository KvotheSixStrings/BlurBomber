using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

	public static Checkpoint lastCheckpoint;
	public Transform spawnPoint;

	void OnTriggerEnter2D (Collider2D collider) {
		if (collider.gameObject.tag == "Player") {
			lastCheckpoint = this;
		}
	}
}
