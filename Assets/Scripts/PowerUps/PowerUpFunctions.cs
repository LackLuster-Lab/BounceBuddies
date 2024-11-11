using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class PowerUpFunctions : NetworkBehaviour
{
    public static PowerUpFunctions instance {get; private set; }

	[SerializeField] protected GameObject Rocketvfx;
	[SerializeField] protected GameObject timeBombArea;
	[SerializeField] protected GameObject projectile;

	public EventHandler OnExplosion;
	public EventHandler OnProjectile;

	public EventHandler<ProjectileCallbackEventArgs> onProjectileCallback;
	public class ProjectileCallbackEventArgs : EventArgs {
		public Vector3 position;
	}

	public enum powerup {
		None,
        Rocket,
		TimeBomb
    }

	private void Awake() {
		instance = this;
	}

	public void UsePowerup(powerup usedPowerup, GameInput gameInput, GameObject gameObject) {
		switch (usedPowerup) {
			case powerup.None:
				break;
			case powerup.Rocket:
				RocketPower(gameInput, gameObject);
				break;
			case powerup .TimeBomb:
				TimeBomb(gameInput, gameObject);
				break;
		}
	}

	public void TimeBomb(GameInput gameInput, GameObject gameObject) {
		Vector2 currentDir = gameInput.playerInputActions.Player.Move.ReadValue<Vector2>();
		Vector3 velocity = new Vector3(currentDir.x, currentDir.y, 0);
		SpawnTimeBombServerRpc(gameObject.transform.position, velocity);
	}

	[ServerRpc(RequireOwnership = false)]
	private void SpawnTimeBombServerRpc(Vector3 position, Vector3 velocity) {
		SpawnTimeBombClientRpc(position, velocity);
	}

	[ClientRpc]
	private void SpawnTimeBombClientRpc(Vector3 position,Vector3 velocity) {
		GameObject timeBomb = Instantiate(projectile, position, Quaternion.identity);
		timeBomb.GetComponent<Projectile>().SetVelocity(velocity);
		OnProjectile?.Invoke(this, EventArgs.Empty);
		onProjectileCallback += spawnTimeBombArea;
	}

	private void spawnTimeBombArea(object sender, ProjectileCallbackEventArgs e) {
		OnExplosion?.Invoke(this, EventArgs.Empty);
		GameObject timeBomb = Instantiate(timeBombArea, e.position, Quaternion.identity);
		Destroy(timeBomb, 5);
	}


	public void RocketPower(GameInput gameInput, GameObject gameObject) {
		Vector2 currentDir = gameInput.playerInputActions.Player.Move.ReadValue<Vector2>();
		gameObject.GetComponent<Rigidbody2D>().velocity = currentDir * 50;

		Vector3 Rotation = new Vector3(0, 0, (Mathf.Rad2Deg * Mathf.Acos(Vector2.Dot(Vector2.right, currentDir))));
		if (currentDir.y < 0) {
			Rotation *= -1;

		}
		Rotation = Rotation + new Vector3(0, 0, 180);
		//GameObject particles = Instantiate(vfx, eventArgs.gameObject.transform.position, Quaternion.Euler(Rotation));
		Debug.Log("finishedAction");
		SpawnRocketVFXServerRpc(gameObject.transform.position, Quaternion.Euler(Rotation));
		//FIX PARTICLES
	}

	[ServerRpc(RequireOwnership = false)]
	public void SpawnRocketVFXServerRpc(Vector3 position, Quaternion rotation) {
		Debug.Log("ConnectedtoServer");
		SpawnRocketVFXClientRpc(position, rotation);
	}

	[ClientRpc]
	public void SpawnRocketVFXClientRpc(Vector3 position, Quaternion rotation) {
		Debug.Log("spawnParticles");
		GameObject particles = Instantiate(Rocketvfx, position, rotation);
		Destroy(particles, 1);
		OnExplosion?.Invoke(this, EventArgs.Empty);
	}
}
