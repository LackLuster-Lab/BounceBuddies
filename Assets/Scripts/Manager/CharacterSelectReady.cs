using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterSelectReady : NetworkBehaviour {

	public static CharacterSelectReady instance {get; private set; }

	private Dictionary<ulong, bool> PlayerReadyDictionary;

	private void Awake() {
		PlayerReadyDictionary = new Dictionary<ulong, bool>();
		instance = this;
	}

	public void SetPlayerReady() {
		SetPlayerReadyServerRpc();
	}

	[ServerRpc(RequireOwnership = false)]
	private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default) {
		PlayerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;
		bool allClientsReady = true;
		foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
			if (!PlayerReadyDictionary.ContainsKey(clientId) || !PlayerReadyDictionary[clientId]) {
				// This player is NOT ready
				allClientsReady = false;
				break;
			}
		}

		if (allClientsReady) {
			Loader.LoadNetwork(Loader.scenes.GameScene);
		}
	}
}
