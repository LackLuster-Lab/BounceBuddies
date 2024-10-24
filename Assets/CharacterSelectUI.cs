using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour {
	[SerializeField] Button mainMenuButton;
	[SerializeField] Button readyButton;
	[SerializeField] TextMeshProUGUI lobbyNameText;
	[SerializeField] TextMeshProUGUI lobbyCodeText;

	private void Awake() {
		mainMenuButton.onClick.AddListener(() => {
			NetworkManager.Singleton.Shutdown();
			Loader.Load(Loader.scenes.MainMenu);
		});
		readyButton.onClick.AddListener(() => {
			CharacterSelectReady.instance.SetPlayerReady();
		});
	}

	private void Start() {
		Lobby lobby = GameLobby.Instance.GetLobby();
		lobbyNameText.text = "Lobby Name: " + lobby.Name;
		lobbyCodeText.text = "Lobby Code: " + lobby.LobbyCode;
	}
}
