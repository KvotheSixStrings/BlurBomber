using UnityEngine;
using System.Collections;

public class BeamFollower : MonoBehaviour {

	public Transform target;
	public Beam beam;
	public float followSpeed = 2;
	
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
		
		Vector2 dir = (target.position - transform.position) * 10;
		
		beam.rayDir = Vector2.Lerp(beam.rayDir, dir, Time.deltaTime * followSpeed);
	}
}
