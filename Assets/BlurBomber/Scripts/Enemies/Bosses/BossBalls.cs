using UnityEngine;
using System.Collections;

public class BossBalls : BadGuy {

	public int stage = 0;

	public Transform eye;
	public SpriteRenderer pupil;
	public Gun gun;

	void Update () {
		if (!alive || target == null || (target.transform.position - transform.position).magnitude > maxDist) return;

		Vector2 dir = (Vector2)target.position - (Vector2)transform.position;
		eye.right = dir;
		
		if (dir.magnitude > 0.1) gun.Shoot(dir, Vector3.zero, target);
	}
}
