using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Paraphernalia.Extensions;
using Paraphernalia.Components;
using Paraphernalia.Utils;

public class Gun : MonoBehaviour {

	public Projectile projectilePrefab;
	public float launchDelay = 1.5f;
	public float kickbackForce = 1f;
	public AudioClip fireSound;

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
		
		return projectile;
	}

	public float Shoot (Vector3 dir, Vector3 parentVelocity) {
		if (Time.time - launchTime > launchDelay) {
			launchTime = Time.time;
			Projectile projectile = GetNextProjectile();
			projectile.Fire(transform.position, dir, parentVelocity);
			AudioManager.PlayEffect(fireSound, transform, Random.Range(0.7f, 1), Random.Range(0.95f, 1.05f));
			return kickbackForce;
		}
		return 0;
	}
}
