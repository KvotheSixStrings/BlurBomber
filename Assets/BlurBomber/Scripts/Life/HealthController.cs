using UnityEngine;
using System.Collections;
using Paraphernalia.Components;
using Paraphernalia.Utils;

public class HealthController : MonoBehaviour {

	public delegate void OnHealthChanged(float health, float prevHealth, float maxHealth);
	public event OnHealthChanged onHealthChanged = delegate {};

	public delegate void OnAnyHealthChanged(HealthController healthController, float health, float prevHealth, float maxHealth);
	public static event OnAnyHealthChanged onAnyHealthChanged = delegate {};

	public delegate void OnAnyLifeChangeEvent(HealthController controller);
	public static event OnAnyLifeChangeEvent onAnyDeath = delegate {};

	public delegate void OnLifeChangeEvent();
	public event OnLifeChangeEvent onDeath = delegate {};
	public event OnLifeChangeEvent onResurection = delegate {};

	public AudioClip deathSound;
	public string particlesName;
	public bool disableOnDeath = false;

	public float maxHealth = 3;
	private float _health = 3;
	public float health {
		get {
			return _health;
		}
		set {
			if (value <= 0 && _health > 0) {
				if (deathSound != null) AudioManager.PlayEffect(deathSound);
				if (!string.IsNullOrEmpty(particlesName)) ParticleManager.Play(particlesName, transform.position);
				onAnyDeath(this);
				onDeath();
				if (disableOnDeath) GameObjectUtils.Destroy(gameObject);
			}
			else if (value > 0 && _health == 0) {
				onResurection();
			}
			
			if (_health != value) {
				onHealthChanged(value, _health, maxHealth);
				onAnyHealthChanged(this, value, _health, maxHealth);
				_health = value;
			}

			if (_health > maxHealth) {
				_health = maxHealth;
			}
		}
	}

	public bool isDead {
		get { return health <= 0; }
	}
	
	void Start() {
		health = maxHealth;
	}

	public void TakeDamage(float damage) {
		health -= damage;
	}
}
