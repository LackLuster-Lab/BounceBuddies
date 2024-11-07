using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour {
	[SerializeField] Button mainMenuButton;
	[SerializeField] Button createLobbyButton;
	[SerializeField] Button BrowseLobbiesButotn;
	[SerializeField] Button joinCodeButton;
	[SerializeField] Button joinCodeUIButton;
	[SerializeField] Button CloseBrowse;
	[SerializeField] Button CloseJoin;
	[SerializeField] Button CloseCreate;
	[SerializeField] TMP_InputField codeText;
	[SerializeField] TMP_InputField playerNameInputField;
	[SerializeField] LobbyCreateUI lobbyCreateUI;
	[SerializeField] Transform lobbyContainer;
	[SerializeField] Transform lobbyTemplate;
	[SerializeField] Transform LobbyListUI;
	[SerializeField] JoinCodeInput JoinCodeUI;


	private void Awake() {
		mainMenuButton.onClick.AddListener(() => {
			GameLobby.Instance.LeaveLobby();
			Loader.Load(Loader.scenes.MainMenu);
		});
		createLobbyButton.onClick.AddListener(() => {
			//GameLobby.Instance.CreateLobby("LobbyName", false);
			lobbyCreateUI.Show();

		});
		BrowseLobbiesButotn.onClick.AddListener(() => {
			LobbyListUI.gameObject.SetActive(true);
			CloseBrowse.Select();
		});
		joinCodeButton.onClick.AddListener(() => {
			GameLobby.Instance.JoinWithCode(codeText.text);
		});
		CloseBrowse.onClick.AddListener(() => {
			LobbyListUI.gameObject.SetActive(false);
			createLobbyButton.Select();
		});
		CloseJoin.onClick.AddListener(() => {
			JoinCodeUI.gameObject.SetActive(false);
			createLobbyButton.Select();
		});
		CloseCreate.onClick.AddListener(() => {
			createLobbyButton.Select();
		});
		joinCodeUIButton.onClick.AddListener(() => {
			JoinCodeUI.show();
		});

		JoinCodeUI.gameObject.SetActive(false);
		lobbyTemplate.gameObject.SetActive(false);
		LobbyListUI.gameObject.SetActive(false);
	}

	private void Start() {
		playerNameInputField.text = MultiplayerManager.instance.GetPlayerName();
		playerNameInputField.onValueChanged.AddListener((string newText) => {
			MultiplayerManager.instance.SetPlayerName(newText);
		});

		createLobbyButton.Select();

		GameLobby.Instance.OnLobbyListChanged += Instance_OnLobbyListChanged;
		updateLobbyList(new List<Lobby>());
	}

	private void Instance_OnLobbyListChanged(object sender, GameLobby.OnLobbyListChangedEventArgs e) {
		updateLobbyList(e.lobbyList);
		Player.LocalInstance.mylobbyui = this;
	}

	private void updateLobbyList(List<Lobby> lobbies) {
		foreach (Transform child in lobbyContainer) {
			if (child == lobbyTemplate) continue;
			Destroy(child.gameObject);
		}

		foreach (Lobby lobby in lobbies) {
			Transform lobbyTransform = Instantiate(lobbyTemplate, lobbyContainer);
			lobbyTransform.gameObject.SetActive(true);
			lobbyTransform.GetComponent<LobbyListSingleUI>().SetLobby(lobby);
		}
	}

	private void OnDestroy() {
		GameLobby.Instance.OnLobbyListChanged -= Instance_OnLobbyListChanged;
	}
}
