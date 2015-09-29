using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Paraphernalia.Extensions;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(AudioSource))]
public class Beam : MonoBehaviour {

	public AudioClip normalSound;
	public AudioClip hitSound;
	public float damagePerSecond = 0.5f;
	public int sortingOrder = 100;
	[SortingLayer] public string sortingLayer;

	public Color color = Color.red;
	[Range(8,512)]public int segments = 8;
	public float halfWidth = 0.1f;
	public Vector2 rayDir = Vector2.right;
	public float rayLen = 10;
	public LayerMask collisionLayers = -1;
	public List<Collider2D> ignoredColliders = new List<Collider2D>();
	public ParticleSystem particles;
	[Range(0,1)] public float variation = 1;

	private float[] offsets;
	private Vector2 beamDir = Vector2.right;
	private new AudioSource audio;

	private Mesh _mesh;
	private Mesh mesh {
		get {
			if (_mesh == null) {
				_mesh = new Mesh();
			}
			return _mesh;
		}
	}

	void Start() {
		audio = GetComponent<AudioSource>();
		Setup();
	}

	void OnEnable() {
		GetComponent<MeshRenderer>().enabled = true;
		GetComponent<PolygonCollider2D>().enabled = true;
		if (particles) particles.Play();
	}

	void OnDisable() {
		GetComponent<MeshRenderer>().enabled = false;
		GetComponent<PolygonCollider2D>().enabled = false;
		if (particles) particles.Stop();
	}

	[ContextMenu("Setup")]
	void Setup () {
		offsets = new float[segments+1];

		float w = beamDir.magnitude / (float)segments;
		float prev = Random.Range(-0.01f, 0.01f);
		for (int i = 0; i <= segments; i++) {
			prev += Random.Range(-0.01f, 0.01f);
			offsets[i] = 0.05f * Mathf.Sin((float)i * w) + prev;
		}
		SetupMesh();
	}

	void SetupMesh () {		
		Vector3[] vertices = new Vector3[segments * 4];
		Vector3[] normals = new Vector3[segments * 4];
		Vector2[] uv = new Vector2[segments * 4];
		Color[] colors = new Color[segments * 4];
		int[] triangles = new int[segments * 6];

		int triIndex = 0;
		float frac = beamDir.magnitude / (halfWidth * (float)segments);
		for (int i = 0; i < segments; i++) {
			float a = (float)i * frac;
			float b = (float)(i + 1) * frac;
			uv.SetRange(
				i * 4, 
				new Vector2[]{
					new Vector2(a, 1),
					new Vector2(b, 1),
					new Vector2(b, 0),
					new Vector2(a, 0)
				}
			);
			colors.SetRange(i * 4, new Color[] {color, color, color, color});
			normals.SetRange(i * 4, new Vector3[] {-Vector3.forward, -Vector3.forward, -Vector3.forward, -Vector3.forward});
			triangles.SetRange(i * 6, new int[] {triIndex, triIndex+2, triIndex+3, triIndex, triIndex+1, triIndex+2});
			triIndex += 4;
		}

		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.uv = uv;
		mesh.colors = colors;
		mesh.triangles = triangles;
		GetComponent<MeshFilter>().mesh = mesh;

		Renderer r = GetComponent<Renderer>();
		r.sortingOrder = sortingOrder;
		r.sortingLayerName = sortingLayer;

		UpdateMesh();
	}

	void UpdateMesh () {
		Vector3[] vertices = new Vector3[segments * 4];
		float len = beamDir.magnitude;
		float segLen = len / (float)segments;
		Vector3 dir = (Vector3)beamDir.normalized;
		Vector3 perp = beamDir.GetPerpendicular().normalized;
		for (int i = 0; i < segments; i++) {
			vertices.SetRange(
				i * 4, 
				new Vector3[] {
					perp * (halfWidth + offsets[i]) + dir * i * segLen,
					perp * (halfWidth + offsets[i+1]) + dir * (i+1) * segLen,
					-perp * (halfWidth - offsets[i+1]) + dir * (i+1) * segLen,
					-perp * (halfWidth - offsets[i]) + dir * i * segLen,
				}
			);
		}

		mesh.vertices = vertices;
		mesh.RecalculateBounds();
		GetComponent<MeshFilter>().mesh = mesh;

		UpdateCollider();
	}

	void UpdateCollider () {
		Vector2 perp = beamDir.GetPerpendicular().normalized * halfWidth;
		
		GetComponent<PolygonCollider2D>().points = new Vector2[] {
			perp,
			perp + (Vector2)beamDir,
			-perp + (Vector2)beamDir,
			-perp,
		};
	}

	void UpdateBeamDir () {
		Collider2D collider = null;
		if (ignoredColliders.Count == 0) {
			RaycastHit2D hit = Physics2D.CircleCast(
				transform.position, 
				halfWidth,
				rayDir.normalized, 
				rayLen, 
				collisionLayers
			);
			collider = hit.collider;
			beamDir = rayDir.normalized * Vector2.Distance(hit.point, transform.position);
		}
		else {
			RaycastHit2D[] hits = Physics2D.CircleCastAll(
				transform.position, 
				halfWidth,
				rayDir.normalized, 
				rayLen, 
				collisionLayers
			);

			foreach (RaycastHit2D hit in hits) {
				if (ignoredColliders.Contains(hit.collider)) continue;
				else {
					collider = hit.collider;
					beamDir = rayDir.normalized * Vector2.Distance(hit.point, transform.position);
					break;
				}
			}
		}

		if (collider != null) {
			GameObject go = collider.gameObject;
			Projectile p = go.GetComponent<Projectile>();
			if (p != null) p.OnHit(Vector3.up); // TODO: should be perpindicular to beam

			HealthController h = go.GetComponent<HealthController>();
			if (h == null) h = go.GetAncestorComponent<HealthController>();
			if (h != null) {
				if (particles) particles.Play();
				if (audio.clip != hitSound) {
					audio.clip = hitSound;
					audio.Play();
				}
				h.TakeDamage(damagePerSecond * Time.deltaTime);
			}
			else if (audio.clip != normalSound) {
				if (particles) particles.Stop();
				audio.clip = normalSound;
				audio.Play();
			}
		}
		else {
			if (particles) particles.Stop();
			if (audio.clip != normalSound) {
				audio.clip = normalSound;
				audio.Play();
			}
			beamDir = rayDir.normalized * rayLen;
		}

		if (particles) particles.transform.position = transform.position + (Vector3)beamDir;
	}

	void Update() {
		UpdateBeamDir();

		if (segments * 6 != mesh.triangles.Length) Setup();
		else {
			for (int i = 0; i < segments; i++) {
				offsets[i] = (Random.Range(-halfWidth, halfWidth) * variation + offsets[Mathf.Min(i+1, segments-1)]) / 2;
			}
			UpdateMesh();
		}
	}
}