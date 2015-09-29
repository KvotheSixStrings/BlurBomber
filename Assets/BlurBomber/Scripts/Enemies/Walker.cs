using UnityEngine;
using System.Collections;
using Paraphernalia.Extensions;

public class Walker : BadGuy {

	public float speed = 10;

	void Update () {
		motor.targetVelocity = Vector2.right * speed * motor.heading;

		RaycastHit2D hit = Physics2D.Linecast(
			transform.position, 
			transform.position + Vector3.right * motor.heading * speed - Vector3.up * speed,
			motor.environmentLayers
		);

		if (hit.collider == null || Mathf.Abs(Vector3.Dot(hit.normal, Vector3.right)) > 0.7f)
			motor.facingRight = !motor.facingRight;
	}

	void OnCollisionEnter2D (Collision2D collision) {
		if (!collision.gameObject.InLayerMask(motor.environmentLayers)) {
			motor.facingRight = !motor.facingRight;
		}
	}
}
