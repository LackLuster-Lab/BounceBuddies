using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class GameLobby : MonoBehaviour {

	public static GameLobby Instance { get; private set; }

	private Lobby joinedLobby;

	private void Awake() {
		DontDestroyOnLoad(gameObject);
		Instance = this;
		InitializeUnityAuthentication();
	}

	private async void InitializeUnityAuthentication() {
		if (UnityServices.State != ServicesInitializationState.Initialized) {
			InitializationOptions options = new InitializationOptions();
			options.SetProfile(Random.Range(0,10000).ToString());
			await UnityServices.InitializeAsync();

			await AuthenticationService.Instance.SignInAnonymouslyAsync();
		}
	}

	public async void CreateLobby(string lobbyName, bool isPrivate) {
		try {
			joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MultiplayerManager.MAX_PLAYERS, new CreateLobbyOptions {
				IsPrivate = isPrivate
			});

			MultiplayerManager.instance.StartHost();
			Loader.LoadNetwork(Loader.scenes.CharacterSelectScene);
		} catch (LobbyServiceException e) {
			Debug.Log(e);
		}
	}

	public async void QuickJoin() {
		try {
			joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

			MultiplayerManager.instance.StartClient();
		} catch (LobbyServiceException e) {
			Debug.Log(e);
		}
	}
}
