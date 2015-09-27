using UnityEngine;
using System.Collections;
using Paraphernalia.Extensions;

public class Damage : MonoBehaviour {

	public float damage = 1;
	public bool disableOnCollision = false;

	protected virtual float GetDamage() {
		if (disableOnCollision) gameObject.SetActive(false);
		return damage;
	}

	void OnTriggerEnter2D (Collider2D collider) {
		HealthController h = collider.gameObject.GetComponent<HealthController>();
		if (h == null) h = collider.gameObject.GetAncestorComponent<HealthController>();
		if (h != null) h.TakeDamage(GetDamage());
	}

	void OnCollisionEnter2D (Collision2D collision) {
		HealthController h = collision.gameObject.GetComponent<HealthController>();
		if (h == null) h = collision.gameObject.GetAncestorComponent<HealthController>();
		if (h != null) h.TakeDamage(GetDamage());
	}	
}
