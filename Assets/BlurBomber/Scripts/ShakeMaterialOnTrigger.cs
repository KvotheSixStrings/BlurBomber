using UnityEngine;
using System.Collections;

public class ShakeMaterialOnTrigger : MonoBehaviour {

	public float shakeTime = 0.2f;
	public float amplitude = 0.1f;
	public int oscillations = 3;

	private static int offsetID = -1;

	void Awake () {
		if (offsetID == -1) offsetID = Shader.PropertyToID("_Offset");
	}

	void OnTriggerEnter2D (Collider2D collider) {
		Shake();
	}

	[ContextMenu("Shake")]
	void Shake () {
		StartCoroutine("ShakeCoroutine");
	}

	IEnumerator ShakeCoroutine () {
		float t = 0;
		while (t < shakeTime) {
			t += Time.deltaTime;
			float frac = Mathf.Clamp01(t / shakeTime);
			float x = Mathf.Sin(frac * oscillations * 2 * Mathf.PI) * amplitude * (1-frac);
			SetOffset(new Vector4(x,0,0,1));
			yield return new WaitForEndOfFrame();
		}
		SetOffset(new Vector4(0,0,0,1));
	}

	public void SetOffset (Vector4 offset) {
		MaterialPropertyBlock props = new MaterialPropertyBlock();
		GetComponent<Renderer>().GetPropertyBlock(props);
		props.AddVector(offsetID, offset);
		GetComponent<Renderer>().SetPropertyBlock(props);
	}
}
