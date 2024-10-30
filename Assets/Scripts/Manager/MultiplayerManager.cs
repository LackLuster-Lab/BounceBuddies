using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerManager : NetworkBehaviour {
	public const int MAX_PLAYERS = 4;
	public const string PLAYER_PREFS_PLAYER_NAME = "PlayerName";
	public static MultiplayerManager instance { get; private set; }

	public event EventHandler OnTryingToJoinGame;
	public event EventHandler OnFailedToJoinGame;
	public event EventHandler OnPlayerDataNetworkListChanged;

	[SerializeField] private List<Color> playerColors;

	private NetworkList<PlayerData> playerDataNetworkList;
	private string playerName;

	private void Awake() {
		instance = this;
		DontDestroyOnLoad(gameObject);

		playerName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME, "PlayerName" + UnityEngine.Random.Range(100, 1000));

		playerDataNetworkList = new NetworkList<PlayerData>();
		playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
	}

	public string GetPlayerName() {
		return playerName;
	}

	public void SetPlayerName(string playerName) {
		this.playerName = playerName;

		PlayerPrefs.SetString(PLAYER_PREFS_PLAYER_NAME, playerName);
	}

	private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent) {
		OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
	}

	public void StartHost() {
		NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
		NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
		NetworkManager.Singleton.OnClientDisconnectCallback += Singleton_Host_OnClientDisconnectCallback;
		NetworkManager.Singleton.StartHost();
	}

	private void Singleton_Host_OnClientDisconnectCallback(ulong clientId) {
		for (int i = 0; i < playerDataNetworkList.Count; i++) {
			PlayerData playerData = playerDataNetworkList[i];
			if (playerData.clientId == clientId) {
				playerDataNetworkList.RemoveAt(i);
			}
		}
	}

	private void Singleton_OnClientConnectedCallback(ulong ClientId) {
		playerDataNetworkList.Add(new PlayerData {
			clientId = ClientId,
			ColorId = getFirstUnusedColorId(),
		});

		SetPlayerNameServerRpc(GetPlayerName());
		SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
	}

	public void StartClient() {
		OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);

		NetworkManager.Singleton.OnClientDisconnectCallback += Singleton_Client_OnClientDisconnectCallback;
		NetworkManager.Singleton.OnClientConnectedCallback += Singleton_Client_OnClientConnectedCallback1;
		NetworkManager.Singleton.StartClient();
	}

	private void Singleton_Client_OnClientConnectedCallback1(ulong clientId) {
		SetPlayerNameServerRpc(GetPlayerName());
		SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
	}

	[ServerRpc(RequireOwnership = false)]
	private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default) {
		int playerDataIndex = GetPlayerDataIndexfromClientId(serverRpcParams.Receive.SenderClientId);

		PlayerData playerData = playerDataNetworkList[playerDataIndex];

		playerData.playerName = playerName;

		playerDataNetworkList[playerDataIndex] =playerData;
	}

	[ServerRpc(RequireOwnership = false)]
	private void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default) {
		int playerDataIndex = GetPlayerDataIndexfromClientId(serverRpcParams.Receive.SenderClientId);

		PlayerData playerData = playerDataNetworkList[playerDataIndex];

		playerData.playerId = playerId;

		playerDataNetworkList[playerDataIndex] = playerData;
	}

	private void Singleton_Client_OnClientDisconnectCallback(ulong ClientId) {
		OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
	}

	private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest arg1, NetworkManager.ConnectionApprovalResponse arg2) {
		if (SceneManager.GetActiveScene().name != Loader.scenes.CharacterSelectScene.ToString()) {
			arg2.Approved = false;
			arg2.Reason = "game has already started";
			return;
		}
		if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYERS) {
			arg2.Approved = false;
			arg2.Reason = "Lobby is full";
			return;
		}
		arg2.Approved = true;
	}

	public bool IsPlayerIndexConnected(int playerIndex) {
		return playerIndex < playerDataNetworkList.Count;
	}

	public int GetPlayerDataIndexfromClientId(ulong clientId) {
		for (int i = 0; i < playerDataNetworkList.Count; i++) {
			if (playerDataNetworkList[i].clientId == clientId) {
				return i;
			}
		}
		return -1;
	}

	public PlayerData GetPlayerDatafromClientId(ulong clientId) {
		foreach (PlayerData playerData in playerDataNetworkList) {
			if (playerData.clientId == clientId) {
				return playerData;
			}
		}
		return default;
	}
	public PlayerData GetPlayerDatafromPlayerIndex(int playerIndex) {
		return playerDataNetworkList[playerIndex];
	}

	public PlayerData GetPlayerData() {
		return GetPlayerDatafromClientId(NetworkManager.Singleton.LocalClientId);
	}

	public Color getPlayerColor(int ColorId) {
		return playerColors[ColorId];
	}

	public void ChangePlayerColor(int colorId) {
		ChangePlayerColorServerRpc(colorId);
	}

	[ServerRpc(RequireOwnership = false)]
	public void sortPlayersServerRpc() {
		List<PlayerData> list = new List<PlayerData>();
		for (int i = 0; i < playerDataNetworkList.Count; i++) {
			list.Add(playerDataNetworkList[i]);
		}
		list.Sort(delegate (PlayerData item1, PlayerData item2) {
			return item2.points.CompareTo(item1.points);
		});
		playerDataNetworkList.Clear();
		foreach (PlayerData playerData in list) {
			playerDataNetworkList.Add(playerData);
		}
	}

	[ServerRpc(RequireOwnership = false)]
	public void updatePointsServerRpc(int pointsToAdd, ulong clientId) {

		int playerDataIndex = GetPlayerDataIndexfromClientId(clientId);

		PlayerData playerData = playerDataNetworkList[playerDataIndex];

		playerData.points = playerData.points + 1;

		playerDataNetworkList[playerDataIndex] = playerData;
	}

	[ServerRpc(RequireOwnership = false)]
	public void ChangePlayerColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default) {
		if (!isColorAvaliable(colorId)) {
			return;
		}
		int playerDataIndex = GetPlayerDataIndexfromClientId(serverRpcParams.Receive.SenderClientId);

		PlayerData playerData = playerDataNetworkList[playerDataIndex];

		playerData.ColorId = colorId;

		playerDataNetworkList[playerDataIndex] = playerData;
	}

	private bool isColorAvaliable(int colorId) {
		foreach (PlayerData playerData in playerDataNetworkList) {
			if (playerData.ColorId == colorId) {
				return false;
			}
		}
		return true;
	}

	private int getFirstUnusedColorId() {
		for (int i = 0; i < playerColors.Count; i++) {
			if (isColorAvaliable(i)) {
				return i;
			}
		}
		return -1;
	}

	public void KickPlayer(ulong ClientId) {
		NetworkManager.Singleton.DisconnectClient(ClientId);
		Singleton_Host_OnClientDisconnectCallback(ClientId);
	}
}
