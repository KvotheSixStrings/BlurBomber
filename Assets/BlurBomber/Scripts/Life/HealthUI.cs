using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthUI : MonoBehaviour {

	public float speed = 10;
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

	[ContextMenu("Update Color")]
	void UpdateColor() {
		image.color = Color.Lerp(deadColor, healthyColor, slider.value);
	}

	void OnHealthChanged(float health, float prevHealth, float maxHealth) {
		StopAllCoroutines();
		StartCoroutine(ChangeHealthCoroutine(health, prevHealth, maxHealth));
	}

	IEnumerator ChangeHealthCoroutine (float health, float prevHealth, float maxHealth) {
		float endValue = health / maxHealth;
		while (Mathf.Abs(slider.value - endValue) > 0.1f) {
			slider.value = Mathf.Lerp(slider.value, endValue, Time.deltaTime * speed);
			UpdateColor();
			yield return new WaitForEndOfFrame();
		}
		slider.value = endValue;
		UpdateColor();
	}
}
