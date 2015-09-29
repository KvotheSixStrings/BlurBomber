using UnityEngine;
using System.Collections;
using Paraphernalia.Extensions;

[RequireComponent(typeof(HealthController))]
public class SubdivideOnDeath : MonoBehaviour {

	[Range(2, 10)] public int fragments = 4;
	[Range(0,1)] public float sizeScale = 0.25f;
	public int maxDepth = 1;
	private int currentDepth = 0;
	private HealthController healthController;

	void Awake () {
		healthController = GetComponent<HealthController>();
	}
	void OnEnable () {
		healthController.onDeath += OnDeath;
	}

	void OnDisable () {
		healthController.onDeath -= OnDeath;
	}

	void OnDeath() {
		if (currentDepth == maxDepth) return;
		
		float spread = 360f / (float)(fragments - 1);
		float angle = 0;
        for (int i = 0; i < fragments-1; i++) {
		    GameObject go = gameObject.Instantiate() as GameObject;
			go.transform.localScale = transform.localScale * sizeScale; 
			go.transform.position = transform.position + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
            angle += spread;
        	SubdivideOnDeath s = go.GetComponent<SubdivideOnDeath>();
			s.currentDepth = currentDepth + 1;
			Rigidbody2D r = go.GetComponent<Rigidbody2D>();
			r.isKinematic = false;
		}
		currentDepth++;
		transform.localScale = transform.localScale * sizeScale;
	}
}
