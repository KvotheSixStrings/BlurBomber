using UnityEngine;
using System.Collections;
using Paraphernalia.Utils;

public class TrashMan : BadGuy {

	public float maxSpeed = 10;
	public float minDistance = 2;
	public float maxDistance = 10;
	public Gun gun;

	[ContextMenu("Hide")]
	public void Hide () {
		animator.SetTrigger("hide");
	}

	public void Fire () {
		gun.Shoot(gun.transform.right, Vector2.zero);
	}

	void Update () {
		if (target == null) return;
		float distance = Vector2.Distance(transform.position, target.transform.position);
		float heading = Mathf.Sign(target.transform.position.x - transform.position.x);

		if (heading > 0) motor.facingRight = true;
		else if (heading < 0) motor.facingRight = false;

		float x = 0;
		if (distance > maxDistance) {
			if (animator.GetBool("hide")) animator.SetBool("hide", false);
			x = Steering.Pursue(rigidbody2D, target, maxSpeed).x;
			// Debug.Log("too far " + x);
		}
		else if (distance < minDistance) {
			if (animator.GetBool("hide")) animator.SetBool("hide", false);
			x = Steering.Evade(rigidbody2D, target, maxSpeed).x;
			// Debug.Log("too close " + x);
		}
		else {
			// Debug.Log("stop and fire");
			if (!animator.GetBool("hide")) animator.SetBool("hide", true);
		}

		motor.targetVelocity = transform.right * heading * x * maxSpeed;
	}
}
