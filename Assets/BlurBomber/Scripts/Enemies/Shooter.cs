using UnityEngine;
using System.Collections;

public class Shooter : MonoBehaviour {

	public Transform target;
	public Gun[] guns;
	private int gunIndex = 0;
	private HealthController healthController;
	private bool alive = true;

	void OnEnable() {
		healthController.onDeath += OnDeath;
	}

	void OnDisable() {
		healthController.onDeath -= OnDeath;
	}

	void Awake () {
		healthController = GetComponent<HealthController>();
	}

	void OnDeath () {
		alive = false;
	}

	void Update () {
		if (!alive) return;

		Vector2 dir = target.position - transform.position;
		
		if (Mathf.Abs(dir.x) < 0.1 || Mathf.Abs(dir.y) < 0.1) {
			guns[gunIndex].Shoot(dir, Vector3.zero);
			gunIndex = (gunIndex + 1) % guns.Length;
		}
	}
}
