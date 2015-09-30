using UnityEngine;
using System.Collections;
using Paraphernalia.Components;

public class StageController : MonoBehaviour {

	public PlayerController player;
	public BadGuy boss;
	public Transform startSpawn;

	void OnEnable () {
		player.healthController.onDeath += OnPlayerDeath;
		boss.healthController.onDeath += OnBossDeath;
	}

	void OnDisable () {
		player.healthController.onDeath -= OnPlayerDeath;
		boss.healthController.onDeath -= OnBossDeath;
	}

	void OnPlayerDeath () {
		Transform spawn = startSpawn;
		if (Checkpoint.lastCheckpoint != null) spawn = Checkpoint.lastCheckpoint.spawnPoint;
		player.transform.position = spawn.position;
		player.motor.facingRight = (spawn.transform.eulerAngles.y == 0);
		CameraController.instance.cameraZones.Clear();
		player.gameObject.SetActive(true);
		player.healthController.health = player.healthController.maxHealth;
		boss.healthController.health = boss.healthController.maxHealth;
	}

	void OnBossDeath () {
		Debug.Log("YOU WIN!!!");
		// load main menu
		// set stuff in game manager
	}
}
