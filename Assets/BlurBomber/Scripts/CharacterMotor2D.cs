/*
Copyright (C) 2014 Nolan Baker

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions 
of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
DEALINGS IN THE SOFTWARE.
*/

using UnityEngine;
using System.Collections;
using Paraphernalia.Extensions;
using Paraphernalia.Utils;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class CharacterMotor2D : MonoBehaviour {
 
 	public bool hugGround = true;
 	public bool facingRight = true;
	public bool inControl = true;
 	public float groundCheckDist = 1;
 	public float rotationSpeed = 100;
	public float maxSpeedChange = 1.0f;
	public float jumpHeight = 1.0f;
	public float airMovement = 0.1f;
	public float loopBoost = 10;
	public Interpolate.EaseType loopEase = Interpolate.EaseType.InCirc;
	public LayerMask environmentLayers = -1;
	
	[HideInInspector] public bool shouldJump = false;
	[HideInInspector] public bool cancelJump = false;
	[HideInInspector] public Vector2 targetVelocity = Vector2.zero;	

	[Range(0, 90)] public int maxIncline = 30;
	public bool isGrounded = true;	
	public bool inAir {
		get { return !isGrounded; }
	}

	public CircleCollider2D circleCollider {
		get { return (CircleCollider2D)GetComponent<Collider2D>(); }
	}

	public float heading {
		get { return (facingRight ? 1 : -1); }
	}

	void HugGround () {
		Vector3 center = transform.TransformPoint(circleCollider.offset);
		RaycastHit2D hit = Physics2D.CircleCast(
			center,
			circleCollider.radius, 
			-transform.up,
			groundCheckDist, 
			environmentLayers
		);

		if (hit.collider != null) {
			isGrounded = true;

			transform.rotation = Quaternion.Slerp(
				transform.rotation,
				Quaternion.LookRotation(
					Vector3.forward * heading, 
					hit.normal
				),
				Time.deltaTime * rotationSpeed
			);

			Rigidbody2D r = GetComponent<Rigidbody2D>();
			float t = Interpolate.Ease(loopEase, 0.5f + Vector2.Dot(hit.normal, -Vector2.up) * 0.5f);
			Vector2 centrifugalForce = -r.velocity.magnitude * hit.normal;
			Vector2 tangentialForce = r.velocity * loopBoost;
			r.AddForce(Vector2.Lerp(centrifugalForce, tangentialForce, t), ForceMode.Acceleration);
		}
		else {
			isGrounded = false;
		}
	}
 
	Vector2 JumpVelocity () {
		return (Vector2)transform.up * Mathf.Sqrt(2 * jumpHeight * Physics2D.gravity.magnitude);
	}

	void UpdateForces () {
		Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
		Vector2 velocity = rigidbody2D.velocity;
		Vector2 velocityChange = (targetVelocity - velocity);
		velocityChange = velocityChange.normalized * Mathf.Clamp(velocityChange.magnitude, -maxSpeedChange, maxSpeedChange);

		if (inAir) {
			velocityChange.y = 0;
			velocityChange = velocityChange * airMovement;
		}
		else if (shouldJump) {
			isGrounded = false;
			rigidbody2D.velocity = (Vector2)transform.right * Vector2.Dot(velocity, transform.right) + JumpVelocity();
		}
		
		rigidbody2D.AddForce(velocityChange, ForceMode.VelocityChange);
	}
	
	void FixedUpdate () {
		if (hugGround && isGrounded) HugGround();		
		if (inControl) UpdateForces();
	}

	void Update () {
		if (inAir) {
			transform.rotation = Quaternion.Slerp(
				transform.rotation, 
				Quaternion.LookRotation(Vector3.forward * heading),
				Time.deltaTime * rotationSpeed
			);
		}
	}

	void OnCollisionStay2D (Collision2D collision) {
		if (collision.gameObject.InLayerMask(environmentLayers)) isGrounded = true;
	}
}