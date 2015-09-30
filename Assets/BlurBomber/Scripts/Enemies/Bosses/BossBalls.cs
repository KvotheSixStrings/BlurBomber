using UnityEngine;
using System.Collections;

public class BossBalls : BadGuy {

	public Vector3 beamDir = Vector3.right;
	public int stage = 0;
	public float turnSpeed = 1;

	public Transform eye;
	public SpriteRenderer pupil;
	public Gun gun;
	public Beam beam;

	public float beamWidth = 3;
	public float beamChargeTime = 1.5f;
	public float beamFireTime = 0.5f;
	public float beamThreshold = 0.9f;
	private bool beamAnimating = false;

	void Update () {
		if (!alive || target == null || (target.transform.position - transform.position).magnitude > maxDist) return;

		Vector2 dir = (Vector2)target.position - (Vector2)transform.position;
		float angle = Vector2.Angle(eye.right, dir);
		float cross = Mathf.Sign(Vector3.Cross(eye.right, dir).z);
		if (Vector3.Dot(eye.transform.right, beamDir) > beamThreshold) {
			if (!beamAnimating && !beam.gameObject.activeSelf) StartCoroutine("TurnOnBeam"); 
		}
		else {
			if (!beamAnimating && beam.gameObject.activeSelf) StartCoroutine("TurnOffBeam");
			if (dir.magnitude > 0.1) gun.Shoot(dir, Vector3.zero, target);
		}

		eye.transform.Rotate(Vector3.forward * angle * cross * Time.deltaTime * turnSpeed);
		
		beam.transform.position = pupil.transform.position;
		beam.rayDir = (Vector2)eye.transform.right;


	}

	IEnumerator TurnOnBeam () {
		beamAnimating = true;
		float t = 0;
		while (t < beamChargeTime) {
			t += Time.deltaTime;
			float frac = t / beamChargeTime;
			pupil.color = Color.Lerp(Color.black, Color.red, frac);
			yield return new WaitForEndOfFrame();
		}
		beam.rayLen = 0;
		beam.halfWidth = 0;
		beam.gameObject.SetActive(true);
		t = 0;
		while (t < beamFireTime) {
			t += Time.deltaTime;
			float frac = t / beamFireTime;
			beam.rayLen = 500 * frac;
			beam.halfWidth = beamWidth * frac;
			yield return new WaitForEndOfFrame();
		}
		beamAnimating = false;
	}

	IEnumerator TurnOffBeam () {
		beamAnimating = true;
		float t = 0;
		while (t < beamFireTime) {
			t += Time.deltaTime;
			float frac = 1 - t / beamFireTime;
			beam.rayLen = 500 * frac;
			beam.halfWidth = beamWidth * frac;
			yield return new WaitForEndOfFrame();
		}
		t = 0;
		while (t < beamChargeTime) {
			t += Time.deltaTime;
			float frac = t / beamChargeTime;
			pupil.color = Color.Lerp(Color.red, Color.black, frac);
			yield return new WaitForEndOfFrame();
		}
		beam.gameObject.SetActive(false);
		beamAnimating = false;
	}
}
