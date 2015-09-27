using UnityEngine;
using System.Collections;

public class BadGuy : MonoBehaviour {

	private Rigidbody2D _target;
	public Rigidbody2D target {
		get {
			if (_target == null) {
				GameObject go = GameObject.FindWithTag("Player");
				if (go != null) _target = go.GetComponent<Rigidbody2D>();
			}
			return _target;
		}
	}

	private Animator _animator;
	public Animator animator {
		get {
			if (_animator == null) {
				_animator = GetComponent<Animator>();
			}
			return _animator;
		}
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

	private CharacterMotor2D _motor;
	public CharacterMotor2D motor {
		get {
			if (_motor == null) {
				_motor = GetComponent<CharacterMotor2D>();
			}
			return _motor;
		}
	}
}
