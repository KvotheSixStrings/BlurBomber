using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Paraphernalia.Extensions;
using Paraphernalia.Components;
using Paraphernalia.Utils;

public class Gun : MonoBehaviour {

	public float maxChargeTime = 1;
	public float maxChargeMultiplier = 3;
	public float maxChargeSize = 2;
	public Projectile projectilePrefab;
	public float launchDelay = 1.5f;
	public float sprayDelay = 0.01f;
	public int projectilesPerShot = 3;
	[Range(0,180)] public float spraySize = 30;
	public float kickbackForce = 1f;
	public AudioClip fireSound;

	private bool charging = false;
	private float chargeStart = 0;
	private Projectile chargeProjectile;
	private Transform chargeParticleParent;
	private Vector3 chargeParticlePosition;
	private List<Projectile> projectilePool = new List<Projectile>();
	private float launchTime = 0;

	Projectile GetNextProjectile () {
		Projectile projectile = null;
		foreach (Projectile p in projectilePool) {
			if (!p.gameObject.activeSelf) {
				projectile = p;
				break;
			}
		}

		if (projectile == null) {
			projectile = projectilePrefab.Instantiate() as Projectile;
			projectilePool.Add(projectile);
		}

		Damage d = projectile.gameObject.GetComponent<Damage>();
		if (d) d.multiplier = 1;

		projectile.transform.localScale = projectilePrefab.transform.localScale;
		
		return projectile;
	}

	void OnDisable () {
		charging = false;
	}

	public void Charge () {
		charging = true;
		chargeStart = Time.time;
		chargeProjectile = GetNextProjectile();
		if (chargeProjectile.particles) {
			chargeParticleParent = chargeProjectile.particles.transform.parent;
			chargeParticlePosition = chargeProjectile.particles.transform.localPosition;
			chargeProjectile.particles.transform.parent = transform;
			chargeProjectile.particles.transform.localPosition = Vector3.zero;
			chargeProjectile.particles.Play();
		}
	}

	public float Shoot (Vector3 dir, Vector3 parentVelocity, Rigidbody2D target = null) {
		if (chargeProjectile != null) {
			charging = false;
			float frac = (Time.time - chargeStart) / maxChargeTime;
			float multiplier = Mathf.Lerp(1, maxChargeMultiplier, frac);
			Damage d = chargeProjectile.gameObject.GetComponent<Damage>();
			if (d) d.multiplier = multiplier;
			float size = Mathf.Lerp(1, maxChargeSize, frac);
			chargeProjectile.size = size;
			chargeProjectile.transform.localScale = projectilePrefab.transform.localScale * size;
			chargeProjectile.particles.transform.parent = chargeParticleParent;
			chargeProjectile.particles.transform.localPosition = chargeParticlePosition;
			StartCoroutine(ChargeShotCoroutine(dir, parentVelocity, target));
			return kickbackForce * multiplier;
		}
		if (Time.time - launchTime > launchDelay) {
			launchTime = Time.time;
			StartCoroutine(ShootCoroutine(dir, parentVelocity, target));
			return kickbackForce;
		}
		return 0;
	}

	IEnumerator ChargeShotCoroutine(Vector3 dir, Vector3 parentVelocity, Rigidbody2D target) {
		chargeProjectile.target = target;
		float ang = spraySize * Random.Range(-0.5f, 0.5f);
		chargeProjectile.Fire(transform.position + Random.insideUnitSphere * 0.1f, Quaternion.AngleAxis(ang, Vector3.forward) * dir, parentVelocity);
		AudioManager.PlayEffect(fireSound, transform, Random.Range(0.7f, 1), Random.Range(1.05f, 1.2f));
		if (sprayDelay > Time.deltaTime) yield return new WaitForSeconds(sprayDelay);
		for (int i = 1; i < projectilesPerShot; i++) {
			Projectile projectile = GetNextProjectile();
			projectile.target = target;
			ang = spraySize * Random.Range(-0.5f, 0.5f);
			projectile.Fire(transform.position + Random.insideUnitSphere * 0.1f, Quaternion.AngleAxis(ang, Vector3.forward) * dir, parentVelocity);
			AudioManager.PlayEffect(fireSound, transform, Random.Range(0.7f, 1), Random.Range(0.95f, 1.05f));
			if (sprayDelay > Time.deltaTime) yield return new WaitForSeconds(sprayDelay);
		}
	}

	IEnumerator ShootCoroutine(Vector3 dir, Vector3 parentVelocity, Rigidbody2D target) {
		for (int i = 0; i < projectilesPerShot; i++) {
			Projectile projectile = GetNextProjectile();
			projectile.target = target;
			float ang = spraySize * Random.Range(-0.5f, 0.5f);
			projectile.Fire(transform.position + Random.insideUnitSphere * 0.1f, Quaternion.AngleAxis(ang, Vector3.forward) * dir, parentVelocity);
			AudioManager.PlayEffect(fireSound, transform, Random.Range(0.7f, 1), Random.Range(0.95f, 1.05f));
			if (sprayDelay > Time.deltaTime) yield return new WaitForSeconds(sprayDelay);
		}
	}

	void Update () {
		if (chargeProjectile != null && !charging) {
			chargeProjectile.particles.transform.parent = chargeParticleParent;
			chargeProjectile.particles.transform.localPosition = chargeParticlePosition;
		}
	}
}
