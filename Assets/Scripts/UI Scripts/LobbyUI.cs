using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour {
	[SerializeField] Button mainMenuButton;
	[SerializeField] Button createLobbyButton;
	[SerializeField] Button quickJoinButton;
	[SerializeField] Button joinCodeButton;
	[SerializeField] TMP_InputField codeText;
	[SerializeField] TMP_InputField playerNameInputField;
	[SerializeField] LobbyCreateUI lobbyCreateUI;

	private void Awake() {
		mainMenuButton.onClick.AddListener(() => {
			GameLobby.Instance.LeaveLobby();
			Loader.Load(Loader.scenes.MainMenu);
		});
		createLobbyButton.onClick.AddListener(() => {
			//GameLobby.Instance.CreateLobby("LobbyName", false);
			lobbyCreateUI.Show();
		});
		quickJoinButton.onClick.AddListener(() => {
			GameLobby.Instance.QuickJoin();
		});
		joinCodeButton.onClick.AddListener(() => {
			GameLobby.Instance.JoinWithCode(codeText.text);
		});

	}

	private void Start() {
		playerNameInputField.text = MultiplayerManager.instance.GetPlayerName();
		playerNameInputField.onValueChanged.AddListener((string newText) => {
			MultiplayerManager.instance.SetPlayerName(newText);
		});
	}
}
