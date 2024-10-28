using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour{

	[SerializeField] private TextMeshProUGUI ErrorMessageText;
	[SerializeField] private Button closeButton;

	private void Awake() {
		closeButton.onClick.AddListener(Hide);
	}

	private void Start() {
		MultiplayerManager.instance.OnFailedToJoinGame += Instance_OnFailedToJoinGame;
		GameLobby.Instance.OnCreateLobbyStarted += Instance_OnCreateLobbyStarted;
		GameLobby.Instance.OnCreateLobbyFailed += Instance_OnCreateLobbyFailed;
		GameLobby.Instance.OnJoinStarted += Instance_OnJoinStarted;
		GameLobby.Instance.OnJoinFailed += Instance_OnJoinFailed;
		GameLobby.Instance.OnQuickJoinFailed += Instance_OnQuickJoinFailed;

		Hide();
	}

	private void Instance_OnQuickJoinFailed(object sender, System.EventArgs e) {
		ShowMessage("Could not find a lobby to Quick Join!");
	}

	private void Instance_OnJoinFailed(object sender, System.EventArgs e) {
		ShowMessage("Failed to join Lobby!");
	}

	private void Instance_OnJoinStarted(object sender, System.EventArgs e) {
		ShowMessage("Joining Lobby...");
	}

	private void Instance_OnCreateLobbyFailed(object sender, System.EventArgs e) {
		ShowMessage("Failed To Create Lobby!");
	}

	private void Instance_OnCreateLobbyStarted(object sender, System.EventArgs e) {
		ShowMessage("Creating Lobby...");
	}

	private void Instance_OnFailedToJoinGame(object sender, System.EventArgs e) {
		if (NetworkManager.Singleton.DisconnectReason == "") {
			ShowMessage("Failed to connect");
		} else {
			ShowMessage(NetworkManager.Singleton.DisconnectReason);
		}
	}

	private void ShowMessage(string message) {
		Show();
		ErrorMessageText.text = message;
	}

	private void Show() {
		gameObject.SetActive(true);
	}

	private void Hide() {
		gameObject.SetActive(false);
	}

	private void OnDestroy() {
		MultiplayerManager.instance.OnFailedToJoinGame -= Instance_OnFailedToJoinGame;
		GameLobby.Instance.OnCreateLobbyStarted -= Instance_OnCreateLobbyStarted;
		GameLobby.Instance.OnCreateLobbyFailed -= Instance_OnCreateLobbyFailed;
		GameLobby.Instance.OnJoinStarted -= Instance_OnJoinStarted;
		GameLobby.Instance.OnJoinFailed -= Instance_OnJoinFailed;
		GameLobby.Instance.OnQuickJoinFailed -= Instance_OnQuickJoinFailed;
	}
}
