using UnityEngine;
using System.Collections;

public class BossMouth : BadGuy {

	public Vector3 beamDir = Vector3.right;
	public int stage = 0;
	public float turnSpeed = 1;

	public Transform[] eyes;
	public Gun[] guns;

	public float beamWidth = 3;
	public float beamChargeTime = 1.5f;
	public float beamFireTime = 0.5f;
	public float beamThreshold = 0.9f;

	void Update () {
		if (!alive || target == null || (target.transform.position - transform.position).magnitude > maxDist) return;

		for (int i = 0; i < eyes.Length; i++) {
			Transform eye = eyes[i];
			Vector2 dir = (Vector2)target.position - (Vector2)eye.position;
			float angle = Vector2.Angle(eye.right, dir);
			float cross = Mathf.Sign(Vector3.Cross(eye.right, dir).z);
			guns[i].Shoot(dir, Vector3.zero, target);

			eye.transform.Rotate(Vector3.forward * angle * cross * Time.deltaTime * turnSpeed);
		}


	}
}
