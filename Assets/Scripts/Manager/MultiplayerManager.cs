using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerManager : MonoBehaviour
{
	private const int MAX_PLAYERS = 4;
    public static MultiplayerManager instance {get; private set;}
	private void Awake() {
        instance = this;    
		DontDestroyOnLoad(gameObject);
	}

	public void StartHost() {
		NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
		NetworkManager.Singleton.StartHost();
	}

	public void StartClient() {
		NetworkManager.Singleton.StartClient();
	}

	private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest arg1, NetworkManager.ConnectionApprovalResponse	arg2) {
		if (SceneManager.GetActiveScene().name != Loader.scenes.CharacterSelectScene.ToString()) {
			arg2.Approved = false;
			arg2.Reason = "game has already started";
			return;
		}
		if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYERS) {
			arg2.Approved = false;
			arg2.Reason = "Lobby is full";
			return;
		}
			arg2.Approved = true;
	}
}
