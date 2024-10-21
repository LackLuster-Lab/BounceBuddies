using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MultiplayerManager : MonoBehaviour
{
    public static MultiplayerManager instance {get; private set;}
	private void Awake() {
        instance = this;    
	}

	public void StartHost() {
		NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
		NetworkManager.Singleton.StartHost();
	}

	public void StartClient() {
		NetworkManager.Singleton.StartClient();
	}

	private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest arg1, NetworkManager.ConnectionApprovalResponse	arg2) {
		
	}
}
