using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthUI : MonoBehaviour {

	public Slider slider;
	public Image image;
	public HealthController healthController;
	public Color healthyColor = Color.green;
	public Color deadColor = Color.red;

	void OnEnable () {
		healthController.onHealthChanged += OnHealthChanged;
	}

	void OnDisable () {
		healthController.onHealthChanged -= OnHealthChanged;
	}

	void OnHealthChanged(float health, float prevHealth, float maxHealth) {
		slider.value = health / maxHealth;
		image.color = Color.Lerp(deadColor, healthyColor, slider.value);
	}
}
