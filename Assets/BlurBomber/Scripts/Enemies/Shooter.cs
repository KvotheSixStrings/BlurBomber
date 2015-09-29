using UnityEngine;
using System.Collections;

public class Shooter : BadGuy {

	public Gun[] guns;
	private int gunIndex = 0;

	void Update () {
		if (!alive) return;

		Vector2 dir = (Vector2)target.position - (Vector2)transform.position;
		
		if (dir.magnitude > 0.1) {
			guns[gunIndex].Shoot(dir, Vector3.zero);
			gunIndex = (gunIndex + 1) % guns.Length;
		}
	}
}
