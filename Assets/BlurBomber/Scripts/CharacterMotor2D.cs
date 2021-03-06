﻿/*
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
using Paraphernalia.Components;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class CharacterMotor2D : MonoBehaviour {
 
 	public AudioClip bumpSound;
 	public bool hugGround = true;
 	public float minHugSpeed = 20;
 	public float groundCheckDist = 1;
 	public float rotationSpeed = 100;
	public float maxSpeedChange = 1.0f;
	public float jumpHeight = 1.0f;
	public float jumpCancelRate = 2;
	public float airMovement = 0.1f;
	public float loopBoost = 10;
	public Interpolate.EaseType loopEase = Interpolate.EaseType.InCirc;
	public LayerMask environmentLayers = -1;
	public LayerMask ladderLayers = -1;
	
	[HideInInspector] public bool inControl = true;
 	[HideInInspector] public bool shouldClimb = false;
 	[HideInInspector] public bool shouldJump = false;
	[HideInInspector] public Vector2 targetVelocity = Vector2.zero;	

	private bool _facingRight = true;
	public bool facingRight {
		get { return _facingRight; }
		set {
			if (_facingRight != value) {
				_facingRight = value;
				transform.rotation = Quaternion.LookRotation(Vector3.forward * heading, transform.up);
			}
		}
	}

	[Range(0, 90)] public int maxIncline = 30;
	private bool _onLadder = false;
	public bool onLadder {
		get { return _onLadder; }
		set {
			if (_onLadder != value) {
				_onLadder = value;
				Physics2D.IgnoreLayerCollision(gameObject.layer, (int)Mathf.Log(environmentLayers, 2), value);
			}
		}
	}

	private bool _isGrounded = false;	
	public bool isGrounded {
		get { return _isGrounded; }
	}
	public bool inAir {
		get { return !_isGrounded && !onLadder; }
	}

	private Rigidbody2D _rigidbody2D;
	new public Rigidbody2D rigidbody2D {
		get { 
			if (_rigidbody2D == null) {
				_rigidbody2D = GetComponent<Rigidbody2D>(); 
			}
			return _rigidbody2D;
		}
	}

	public CircleCollider2D circleCollider {
		get { return (CircleCollider2D)GetComponent<Collider2D>(); }
	}

	public float heading {
		get { return (facingRight ? 1 : -1); }
	}

	private Vector2 _groundNormal = Vector2.up;
	private Vector2 targetNormal {
		get {
			if (_isGrounded && rigidbody2D.velocity.magnitude > minHugSpeed) return _groundNormal;
			else return Vector2.up;
		}
		set { _groundNormal = value; }
	}

	void Start () {
		Vector3 center = transform.TransformPoint(circleCollider.offset);
		RaycastHit2D hit = Physics2D.CircleCast(
			center,
			circleCollider.radius, 
			-transform.up * 1,
			groundCheckDist, 
			environmentLayers
		);

		if (hit.collider != null) {
			_isGrounded = true;
			transform.position = hit.point + hit.normal * circleCollider.radius;
		}
		else {
			_isGrounded = false;
		}
	}

	void CheckGround () {
		Vector3 center = transform.TransformPoint(circleCollider.offset);
		RaycastHit2D hit = Physics2D.CircleCast(
			center,
			circleCollider.radius, 
			-transform.up,
			groundCheckDist, 
			environmentLayers
		);

		if (hit.collider != null && Vector2.Angle(hit.normal, transform.up) < maxIncline) {
			_isGrounded = true;

			if (hugGround && rigidbody2D.velocity.magnitude > minHugSpeed) {
				_groundNormal = hit.normal;
				float t = Interpolate.Ease(loopEase, 0.5f + Vector2.Dot(hit.normal, -Vector2.up) * 0.5f);
				Vector2 centrifugalForce = -rigidbody2D.velocity.magnitude * hit.normal;
				Vector2 tangentialForce = rigidbody2D.velocity * loopBoost;
				rigidbody2D.AddForce(Vector2.Lerp(centrifugalForce, tangentialForce, t), ForceMode.Acceleration);
			}
		}
		else {
			_isGrounded = false;
		}
	}
 
	Vector2 JumpVelocity () {
		return (Vector2)transform.up * Mathf.Sqrt(2 * jumpHeight * Physics2D.gravity.magnitude);
	}

	void UpdateForces () {
		Vector2 velocity = rigidbody2D.velocity;
		Vector2 velocityChange = (targetVelocity - velocity);
		velocityChange = velocityChange.normalized * Mathf.Clamp(velocityChange.magnitude, -maxSpeedChange, maxSpeedChange);

		rigidbody2D.gravityScale = onLadder ? 0 : 1;

		if (inAir) {
			velocityChange.y = 0;
			velocityChange = velocityChange * airMovement;
			if (!shouldJump && velocity.y > 0) {
				velocity.y = Mathf.Lerp(velocity.y, 0, Time.fixedDeltaTime * jumpCancelRate);
				velocityChange.y = 0;
				rigidbody2D.velocity = velocity;
			}
		}
		else if (shouldJump) {
			_isGrounded = false;
			rigidbody2D.velocity = (Vector2)transform.right * Vector2.Dot(velocity, transform.right) + JumpVelocity();
		}
		
		rigidbody2D.AddForce(velocityChange, ForceMode.VelocityChange);
	}
	
	void FixedUpdate () {
		if (inControl) UpdateForces();
		if (_isGrounded) CheckGround();

		transform.rotation = Quaternion.Slerp(
			transform.rotation,
			Quaternion.LookRotation(Vector3.forward * heading, targetNormal),
			Time.deltaTime * rotationSpeed
		);
	}

	void OnCollisionEnter2D (Collision2D collision) {
		if (inAir && collision.gameObject.InLayerMask(environmentLayers) && collision.relativeVelocity.magnitude > 20) {
			ContactPoint2D contact = collision.contacts[0];
			float frac = collision.relativeVelocity.magnitude * -1 * Vector3.Dot(contact.normal, collision.relativeVelocity) / 200f;
			AudioManager.PlayEffect(bumpSound, transform, Mathf.Lerp(0.1f, 1.5f, frac), Mathf.Lerp(0.7f, 1.5f, frac));
		}
	}

	void OnCollisionStay2D (Collision2D collision) {
		if (collision.gameObject.InLayerMask(environmentLayers)) {
			foreach(ContactPoint2D contact in collision.contacts) {
				if (Vector2.Angle(contact.normal, transform.up) < maxIncline) {
					_isGrounded = true;
					return;
				}
			}
		}
	}

	void OnTriggerStay2D (Collider2D collider) {
		if (collider.gameObject.InLayerMask(ladderLayers) && shouldClimb &&
			Mathf.Abs(collider.gameObject.transform.position.x - transform.position.x) < 0.3f) {
			rigidbody2D.velocity = Vector2.up * Vector2.Dot(rigidbody2D.velocity, Vector2.up);
			onLadder = true;
		}
	}

	void OnTriggerExit2D (Collider2D collider) {
		if (collider.gameObject.InLayerMask(ladderLayers)) {
			onLadder = false;
		}
	}
}