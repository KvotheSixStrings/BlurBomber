using UnityEngine;
using System.Collections;
using Paraphernalia.Utils;
using Paraphernalia.Extensions;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(HealthController))]
public class Drone : MonoBehaviour {

	public float maxSpeed = 5;
	public float maxForce = 1;
	public float seekDist = 10;
	public Transform target;

	public LayerMask avoidLayers;
	new private Rigidbody2D rigidbody2D;
	private HealthController healthController;
	private Animator animator;
	private bool alive = true;

	void OnEnable() {
		healthController.onDeath += OnDeath;
	}

	void OnDisable() {
		healthController.onDeath -= OnDeath;
	}

	void Awake () {
		healthController = GetComponent<HealthController>();
		animator = GetComponent<Animator>();
	}

	void Start () {
		rigidbody2D = GetComponent<Rigidbody2D>();
	}

	void OnDeath() {
		rigidbody2D.mass = 10;
		rigidbody2D.drag = 10;
		alive = false;
		animator.SetTrigger("death");
	}

	void Update () {
		if (!alive) return;

		Vector2 dir = target.position - transform.position;
		
		if (dir.magnitude > seekDist) {
			Vector2 seek = Steering.Seek(transform.position, rigidbody2D.velocity, target.position, maxSpeed);
			seek = Steering.ObstacleSweep2D(transform.position, 1, seek, 10, avoidLayers);
			rigidbody2D.AddForce(seek, ForceMode.Force);
		}
		else {
			Vector3 flee = Steering.Flee(transform.position, rigidbody2D.velocity, target.position, maxSpeed);
			flee = Steering.ObstacleSweep2D(transform.position, 1, flee, 10, avoidLayers);
			rigidbody2D.AddForce(flee, ForceMode.Force);
		}
	}
}