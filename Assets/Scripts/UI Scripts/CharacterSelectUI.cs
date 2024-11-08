using System;
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
	public static event EventHandler OnButtonPress;

	private void Awake() {
		mainMenuButton.onClick.AddListener(() => {
			OnButtonPress?.Invoke(this, EventArgs.Empty);
			GameLobby.Instance.LeaveLobby();
			NetworkManager.Singleton.Shutdown();
			Loader.Load(Loader.scenes.MainMenu);
		});
		readyButton.onClick.AddListener(() => {
			OnButtonPress?.Invoke(this, EventArgs.Empty);
			CharacterSelectReady.instance.SetPlayerReady();
		});
	}

	private void Start() {
		readyButton.Select();
	}

	private void Update() {
		Lobby lobby = GameLobby.Instance.GetLobby();
		lobbyNameText.text = lobby.Name;
		lobbyCodeText.text = "Lobby Code: " + lobby.Data["password"].Value;
	}
}
