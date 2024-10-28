using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameLobby : MonoBehaviour {

	private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";

	public static GameLobby Instance { get; private set; }

	public event EventHandler OnCreateLobbyStarted;
	public event EventHandler OnCreateLobbyFailed;
	public event EventHandler OnJoinStarted;
	public event EventHandler OnJoinFailed;
	public event EventHandler OnQuickJoinFailed;
	public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
	public class OnLobbyListChangedEventArgs : EventArgs {
		public List<Lobby> lobbyList;
	}

	private Lobby joinedLobby;
	private float heartBeatTimer;
	private float listLobbiesTimer;

	private void Awake() {
		DontDestroyOnLoad(gameObject);
		Instance = this;
		InitializeUnityAuthentication();
	}

	private async void InitializeUnityAuthentication() {
		if (UnityServices.State != ServicesInitializationState.Initialized) {
			InitializationOptions options = new InitializationOptions();
			options.SetProfile(UnityEngine.Random.Range(0,10000).ToString());
			await UnityServices.InitializeAsync();

			await AuthenticationService.Instance.SignInAnonymouslyAsync();
		}
	}
	private void Update() {
		handleHeartbeat();
		HandlePeriodicListLobbies();
	}

	private void HandlePeriodicListLobbies() {
		if (joinedLobby == null && AuthenticationService.Instance.IsSignedIn && SceneManager.GetActiveScene().name == Loader.scenes.LobbyScene.ToString()) {
			listLobbiesTimer -= Time.deltaTime;
			if (listLobbiesTimer <= 0f) {
				listLobbiesTimer = 3f;
				ListLobbies();
			}
		}
	}

	private void handleHeartbeat() {
		if (isLobbyHost()) {
			heartBeatTimer-= Time.deltaTime;
			if (heartBeatTimer <= 0f) {
				heartBeatTimer = 15f;
				LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
			}
		}
	}

	private bool isLobbyHost() {
		return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
	}

	private async Task<Allocation> AllocateRelay() {
		try {
			Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MultiplayerManager.MAX_PLAYERS - 1);

			return allocation;
		} catch (RelayServiceException ex) {
			Debug.Log(ex);

			return default;
		}
	}

	private async Task<string> getRelayJoinCode(Allocation allocation) {
		try {
			string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
			return relayJoinCode;
		} catch (RelayServiceException ex) {
			Debug.Log(ex);
			return default;
		}
	}

	private async Task<JoinAllocation> JoinRelay(string joinCode) {
		try {
			JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
			return joinAllocation;
		} catch (RelayServiceException ex) {
			Debug.Log(ex);
			return default;
		}
	}
	public async void CreateLobby(string lobbyName, bool isPrivate) {
		OnCreateLobbyStarted?.Invoke(this,EventArgs.Empty);
		try {
			joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MultiplayerManager.MAX_PLAYERS, new CreateLobbyOptions {
				IsPrivate = isPrivate
			});

			Allocation allocation = await AllocateRelay();
			string relayjoinCode = await getRelayJoinCode(allocation);
			await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions {
				Data = new Dictionary<string, DataObject> {
					{KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayjoinCode)}
				}
			});

NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
			MultiplayerManager.instance.StartHost();
			Loader.LoadNetwork(Loader.scenes.CharacterSelectScene);
		} catch (LobbyServiceException e) {
			Debug.Log(e);
			OnCreateLobbyFailed?.Invoke(this, EventArgs.Empty);
		}
	}

	public async void QuickJoin() {
		OnJoinStarted?.Invoke(this, EventArgs.Empty);
		try {
			joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

			string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
			JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);


			NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
			MultiplayerManager.instance.StartClient();
		} catch (LobbyServiceException e) {
			Debug.Log(e);

			OnQuickJoinFailed?.Invoke(this, EventArgs.Empty);
		}
	}

	public async void JoinWithCode(string LobbyCode) {
		OnJoinStarted?.Invoke(this, EventArgs.Empty);
		try {
			joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(LobbyCode);
			string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
			JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);


			NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
			MultiplayerManager.instance.StartClient();
		} catch (LobbyServiceException e) {
			Debug.Log(e);
			OnJoinFailed?.Invoke(this, EventArgs.Empty);
		}
	}
	public async void JoinWithLobbyId(string LobbyId) {
		OnJoinStarted?.Invoke(this, EventArgs.Empty);
		try {
			joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(LobbyId);
			string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
			JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);


			NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
			MultiplayerManager.instance.StartClient();
		} catch (LobbyServiceException e) {
			Debug.Log(e);
			OnJoinFailed?.Invoke(this, EventArgs.Empty);
		}
	}

	private async void ListLobbies() {
		try {
			QueryLobbiesOptions query = new QueryLobbiesOptions {
				Filters = new List<QueryFilter> {
				new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
			}
			};
			QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(query);
			OnLobbyListChanged.Invoke(this, new OnLobbyListChangedEventArgs {
				lobbyList = queryResponse.Results
			});
		} catch (LobbyServiceException e) {
			Debug.Log(e);
		}
	}

	public async void DeleteLobby() {
		if (joinedLobby != null) {
			try {
				await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
				joinedLobby = null;
			} catch (LobbyServiceException e) {
				Debug.Log(e);
			}
		}
	}

	public async void LeaveLobby() {
		if (joinedLobby != null) {
			try {
				await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
				
				joinedLobby = null;
			} catch (LobbyServiceException e) {
				Debug.Log(e);
			}
		}
	}

	public async void KickPlayer(string playerId) {
		if (isLobbyHost()) {
			try {
				await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
			} catch (LobbyServiceException e) {
				Debug.Log(e);
			}
		}
	}

	public Lobby GetLobby() { return joinedLobby; }
}
