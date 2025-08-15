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
	[SerializeField] Transform NoLobbiesText;
	[SerializeField] JoinCodeInput JoinCodeUI;
	public static event EventHandler OnButtonPress;


	private void Awake() {
		mainMenuButton.onClick.AddListener(() => {
			GameLobby.Instance.LeaveLobby();
			Loader.Load(Loader.scenes.MainMenu);
		});
		createLobbyButton.onClick.AddListener(() => {
			//GameLobby.Instance.CreateLobby("LobbyName", false);
			lobbyCreateUI.Show();
			OnButtonPress?.Invoke(this, EventArgs.Empty);

		});
		BrowseLobbiesButotn.onClick.AddListener(() => {
			LobbyListUI.gameObject.SetActive(true);
			CloseBrowse.Select();
			OnButtonPress?.Invoke(this, EventArgs.Empty);
		});
		joinCodeButton.onClick.AddListener(() => {
			OnButtonPress?.Invoke(this, EventArgs.Empty);
			GameLobby.Instance.JoinWithCode(codeText.text);
		});
		CloseBrowse.onClick.AddListener(() => {
			LobbyListUI.gameObject.SetActive(false);
			createLobbyButton.Select();
			OnButtonPress?.Invoke(this, EventArgs.Empty);
		});
		CloseJoin.onClick.AddListener(() => {
			JoinCodeUI.gameObject.SetActive(false);
			createLobbyButton.Select();
			OnButtonPress?.Invoke(this, EventArgs.Empty);
		});
		CloseCreate.onClick.AddListener(() => {
			createLobbyButton.Select();
			OnButtonPress?.Invoke(this, EventArgs.Empty);
		});
		joinCodeUIButton.onClick.AddListener(() => {
			JoinCodeUI.show();
			OnButtonPress?.Invoke(this, EventArgs.Empty);
		});

		JoinCodeUI.gameObject.SetActive(false);
		lobbyTemplate.gameObject.SetActive(false);
		LobbyListUI.gameObject.SetActive(false);
	}

	private void Start() {
		playerNameInputField.text = MultiplayerManager.instance.GetPlayerName();
		playerNameInputField.onValueChanged.AddListener((string newText) => {
			if (newText.Trim() != "") {
				MultiplayerManager.instance.SetPlayerName(newText.Trim());
			}
		});

		createLobbyButton.Select();

		GameLobby.Instance.OnLobbyListChanged += Instance_OnLobbyListChanged;
		updateLobbyList(new List<Lobby>());
	}

	private void Instance_OnLobbyListChanged(object sender, GameLobby.OnLobbyListChangedEventArgs e) {
		updateLobbyList(e.lobbyList);
		//Player.LocalInstance.mylobbyui = this;
	}

	private void updateLobbyList(List<Lobby> lobbies) {
		foreach (Transform child in lobbyContainer) {
			if (child == lobbyTemplate) continue;
			Destroy(child.gameObject);
		}


		if (lobbies.Count > 0) {
			foreach (Lobby lobby in lobbies) {
				Transform lobbyTransform = Instantiate(lobbyTemplate, lobbyContainer);
				lobbyTransform.gameObject.SetActive(true);
				lobbyTransform.GetComponent<LobbyListSingleUI>().SetLobby(lobby);
			}
			NoLobbiesText.gameObject.SetActive(false);
		} else {
			NoLobbiesText.gameObject.SetActive(true);
		}
	}

	private void OnDestroy() {
		GameLobby.Instance.OnLobbyListChanged -= Instance_OnLobbyListChanged;
	}
}
