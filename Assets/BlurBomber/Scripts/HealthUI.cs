using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthUI : MonoBehaviour {

	public Text text;

	void OnEnable () {
		HealthController.onAnyHealthChanged += OnAnyHealthChanged;
	}

	void OnDisable () {
		HealthController.onAnyHealthChanged -= OnAnyHealthChanged;
	}

	void OnAnyHealthChanged(HealthController healthController, float health, float prevHealth, float maxHealth) {
		text.text = "HP: " + health;
	}
}
