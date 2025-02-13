using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MainMenuCleanUp : MonoBehaviour
{
	private void Awake() {
		if (NetworkManager.Singleton != null) {
			Destroy(NetworkManager.Singleton.gameObject);
		}

		if (MultiplayerManager.instance != null) {
			Destroy(MultiplayerManager.instance.gameObject);
		}

		if (GameLobby.Instance != null) {
			Destroy(GameLobby.Instance.gameObject);
		}
	}
}
