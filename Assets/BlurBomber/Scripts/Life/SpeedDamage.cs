using UnityEngine;
using System.Collections;
using Paraphernalia.Extensions;

public class SpeedDamage : Damage {

	public float minSpeed = 20;
	public float maxSpeed = 50;

	new private Rigidbody2D rigidbody2D;

	void Start () {
		rigidbody2D = GetComponent<Rigidbody2D>();
	}

	protected override float GetDamage() {
		return damage * Mathf.Clamp01((maxSpeed - minSpeed) / (rigidbody2D.velocity.magnitude - minSpeed));
	}
}
