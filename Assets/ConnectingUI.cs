using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingUI : MonoBehaviour {

	private void Start() {
		MultiplayerManager.instance.OnTryingToJoinGame += Instance_OnTryingToJoinGame;
		MultiplayerManager.instance.OnFailedToJoinGame += Instance_OnFailedToJoinGame;
		Hide();
	}

	private void Instance_OnFailedToJoinGame(object sender, System.EventArgs e) {
		Hide();
	}

	private void Instance_OnTryingToJoinGame(object sender, System.EventArgs e) {
		Show();
	}

	private void Show(){ 
		gameObject.SetActive(true);
	}

	private void Hide() {
		gameObject.SetActive(false);
	}

	private void OnDestroy() {
		MultiplayerManager.instance.OnTryingToJoinGame -= Instance_OnTryingToJoinGame;
		MultiplayerManager.instance.OnFailedToJoinGame -= Instance_OnFailedToJoinGame;
	}
}
