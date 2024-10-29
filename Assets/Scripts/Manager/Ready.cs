using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Ready : NetworkBehaviour {

	public static Ready instance {get; private set; }


	public event EventHandler OnReadyChange;
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
		SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);
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
			GameManager.instance.loadScene();
		}
	}

	[ClientRpc]
	private void SetPlayerReadyClientRpc(ulong clientId) {
		PlayerReadyDictionary[clientId] = true;

		OnReadyChange?.Invoke(this, EventArgs.Empty);
	}

	public bool IsPlayerReady(ulong clientId) {
		return PlayerReadyDictionary.ContainsKey(clientId) && PlayerReadyDictionary[clientId];
	}
}
