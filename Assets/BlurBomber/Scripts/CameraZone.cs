using UnityEngine;
using System.Collections;

public class CameraZone : MonoBehaviour {

	void OnCollider2DEnter(Collider2D collider) {
		if (collider.gameObject.tag == "Player") {
			
		}
	}
}
