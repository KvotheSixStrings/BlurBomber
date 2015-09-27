using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(HealthController))]
public class Turret : MonoBehaviour {

	public float hideTime = 10;

	private float lastShown;
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
		animator = GetComponent<Animator>();
		healthController = GetComponent<HealthController>();
	}

	void Start () {
		lastShown = Time.time;
	}

	void Update () {
		if (!alive) return;
		if (Time.time - lastShown > hideTime) {
			lastShown = Time.time;
			GetComponent<Animator>().SetTrigger("show");
		}
	}

	void OnDeath() {
		alive = false;
		animator.SetTrigger("death");
	}
}
