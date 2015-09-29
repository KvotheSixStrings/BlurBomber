using UnityEngine;
using System.Collections;
using Paraphernalia.Extensions;

[RequireComponent(typeof(HealthController))]
public class SpeedDamagable : MonoBehaviour {

	public float minSpeed = 40;
	public float maxSpeed = 60;
	public float maxDamage = 10;
	public bool takeXDamage = true;
	public bool takeYDamage = true;

	private HealthController healthController;

	void Start () {
		healthController = GetComponent<HealthController>();
	}

	void OnCollisionEnter2D (Collision2D collision) {
		Vector2 n = collision.contacts[0].normal;
		if ((takeXDamage && Mathf.Abs(Vector3.Dot(n, Vector2.right)) > 0.5f) || 
			(takeYDamage && Mathf.Abs(Vector3.Dot(n, Vector2.up)) > 0.5f)) {
			Vector2 v = collision.relativeVelocity;
			float frac = (Mathf.Clamp(v.magnitude * -Vector3.Dot(v, n), minSpeed, maxSpeed) - minSpeed) / (maxSpeed - minSpeed);
			healthController.TakeDamage(frac * maxDamage);
		}
	}
}
