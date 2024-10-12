using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PowerUpFunctions : NetworkBehaviour
{
    public static PowerUpFunctions instance {get; private set; }

	[SerializeField] protected GameObject Rocketvfx;

	public enum powerup {
		None,
        Rocket,
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
		}
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
	}
}
