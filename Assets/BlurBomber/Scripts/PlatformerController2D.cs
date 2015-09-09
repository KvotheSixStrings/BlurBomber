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

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterMotor2D))]
public class PlatformerController2D : MonoBehaviour {

	public float maxSpeed = 10;
	public ParticleSystem dustParticles;
	public Gun gun;

	private Animator _animator;
	public Animator animator {
		get {
			if (_animator == null) {
				_animator = GetComponent<Animator>(); 
			}
			return _animator;
		}
	}

	private CharacterMotor2D _motor;
	public CharacterMotor2D motor {
		get {
			if (_motor == null) {
				_motor = GetComponent<CharacterMotor2D>(); 
			}
			return _motor;
		}
	}
	
	void Update () {
		float x = Input.GetAxis("Horizontal");
		// float y = Input.GetAxis("Vertical");

		if (x > 0.1f) motor.facingRight = true;
		else if (x < -0.1f) motor.facingRight = false;

		transform.rotation = Quaternion.LookRotation(Vector3.forward * motor.heading, transform.up);
	
		Vector2 direction = Mathf.Abs(x) * transform.right;// + y * transform.up;
		direction.Normalize();
		motor.targetVelocity = direction * maxSpeed;

		bool jumpPressed = Input.GetButton("Jump");
		motor.shouldJump = jumpPressed;
		animator.SetBool("grounded", motor.isGrounded);

		Vector2 v = GetComponent<Rigidbody2D>().velocity;
		float speed = Mathf.Abs(v.x);
		animator.SetFloat("speed", speed);
		animator.SetFloat("yVelocity", v.y);
		if (motor.isGrounded && v.x * direction.x < 0 && !animator.GetBool("skidding")) {
			dustParticles.Play();
			animator.SetBool("skidding", true);
		}
		else if (v.x * direction.x > 0 || speed < 0.1f || motor.inAir) {
			dustParticles.Stop();
			animator.SetBool("skidding", false);
		}

		float alpha = Mathf.Clamp01(speed / maxSpeed) * 0.3f;
		dustParticles.startColor = dustParticles.startColor.SetAlpha(alpha);

		if (Input.GetButtonUp("Fire1")) animator.SetTrigger("fire"); 
	}

	public void Fire() {
		gun.Shoot(transform.right, GetComponent<Rigidbody2D>().velocity);
	}
}