using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class RoundManager : NetworkBehaviour {

	public static RoundManager instance { get; private set; }


	//all different variables for game modes
	private enum GameState {
		WaitingToStart,
		Start,
		playing,
		Endgame
	}
	public enum Gamemode {
		Fighter,
		Race,
		KingOfHill
	}
	public enum DamageType {
		None,
		Stocks,
		Percentage
	}
	public bool isLocalPlayerReady;
	//Gamemode Settings
	[SerializeField] Gamemode gamemode = Gamemode.Fighter;//TODO implement
	[SerializeField] public DamageType damageType = DamageType.Percentage;//TODO implement
	[SerializeField] bool powerUpsEnabled;
	[SerializeField] private float powerUpSpawnTimerMax = 4f;
	[SerializeField] GameObject[] allPowerUps;
	[SerializeField] public List<Vector3> spawnPositions;
	private float powerUpSpawnTimer;
	private GameObject currentPowerup;
	private NetworkVariable<GameState> currentGameState = new NetworkVariable<GameState>(GameState.WaitingToStart);

	//all map specific things
	[SerializeField] Vector4[] powerUpSpawnLocations;// X-Left, Y-Right, Z-Top, W-Bottom
	private List<Player> currentPlayer;
	//Timers
	private float WaitingToStartTimer = 1f;
	private float StartTimer = 3f;
	public bool isLocalGamePaused = false;
	private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);
	[SerializeField] private bool UseGameTimer = true;
	[SerializeField] private float GameTimerMax = 10f;
	[SerializeField] private float GameTimer = 10f;
	private bool autoTestGamePauseState = false;

	[SerializeField] public Transform PlayerPrefab;
	private Dictionary<ulong, bool> PlayerReadyDictionary;
	private Dictionary<ulong, bool> PlayerPauseDictionary;


	//Events
	public event EventHandler OnStateChanged;
	public event EventHandler OnLocalPauseGame;
	public event EventHandler OnMultiplayerPauseGame;
	public event EventHandler OnMultiplayerUnPauseGame;
	public event EventHandler OnLocalUnPauseGame;
	public event EventHandler OnLocalplayerReady;
	public event EventHandler<onEndgameEventArgs> OnEndGame;
	public class onEndgameEventArgs : EventArgs {
		public Dictionary<ulong, bool> winners;
	}
	private PowerUpItem PowerupToManage;

	private int currentPlayers;

	public void addPlayer() {
		currentPlayers++;
	}

	public void SetValues(bool IsPowerups, float gameTimer) {
		powerUpsEnabled = IsPowerups;
		GameTimer = gameTimer;
		GameTimerMax = gameTimer;
	}

	public override void OnNetworkSpawn() {
		currentGameState.OnValueChanged += CurrentGameState_OnValueChanged;
		isGamePaused.OnValueChanged += isGamePaused_OnValueChange;

		if (IsServer) {
			OnEndGame += GameManager.instance.EndRound;
			NetworkManager.Singleton.OnClientDisconnectCallback += Singleton_OnClientDisconnectCallback;
			NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
		}
	}

	private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut) {
		foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
			Transform playerTransform = Instantiate(PlayerPrefab);
			playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
		}
	}

	private void Singleton_OnClientDisconnectCallback(ulong obj) {
		autoTestGamePauseState = true;
	}

	private void isGamePaused_OnValueChange(bool previousValue, bool newValue) {
		if (isGamePaused.Value) {
			Time.timeScale = 0f;
			OnMultiplayerPauseGame?.Invoke(this, EventArgs.Empty);
		} else {
			Time.timeScale = 1f;
			OnMultiplayerUnPauseGame?.Invoke(this, EventArgs.Empty);
		}
	}


	private void CurrentGameState_OnValueChanged(GameState previousValue, GameState newValue) {
		OnStateChanged?.Invoke(this, EventArgs.Empty);
	}
	public void Awake() {
		currentGameState.Value = GameState.WaitingToStart;
		GameTimer = GameTimerMax;
		PlayerReadyDictionary = new Dictionary<ulong, bool>();
		PlayerPauseDictionary = new Dictionary<ulong, bool>();
		instance = this;
	}

	public void Start() {
		GameInput.instance.pausePerformed += Game_pausePerformed;
		GameInput.instance.onPowerUpPerformed += GameInput_OnInteractAction;
	}

	private void Game_pausePerformed(object sender, EventArgs e) {
		PauseGame();
	}

	private void GameInput_OnInteractAction(object sender, EventArgs e) {
		if (currentGameState.Value == GameState.WaitingToStart) {
			isLocalPlayerReady = true;
			OnLocalplayerReady?.Invoke(this, EventArgs.Empty);

			SetPlayerReadyServerRpc();
		}
	}
	[ServerRpc(RequireOwnership = false)]
	private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default) {
		PlayerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;
		bool allClientsReady = true;
		foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
			if (!PlayerReadyDictionary.ContainsKey(clientId) || !PlayerReadyDictionary[clientId]) {
				// This player is NOT ready
				allClientsReady = false;
				break;
			}
		}

		if (allClientsReady) {
			currentGameState.Value = GameState.Start;
		}
	}
	private void LateUpdate() {
		if (autoTestGamePauseState) {
			autoTestGamePauseState = false;
			TestGamePaused();
		}
	}

	public void Update() {
		if (!IsServer) { return; }
		switch (currentGameState.Value) {
			case GameState.WaitingToStart:
				break;
			case GameState.Start:
				startTimerClientRpc(Time.deltaTime);
				if (StartTimer < 0f) {
					setState(GameState.playing);
				}
				break;
			case GameState.playing:
				//GamemodeUpdates
				switch (gamemode) {
					case Gamemode.Fighter:
						int alivePlayers = 0;
						foreach (int b in Player.numberOfPlayers.Values) {
							if (b == 1) {
								alivePlayers++;
							}
						}
						if (alivePlayers == 1) {

							setState(GameState.Endgame);
							OnStateChanged?.Invoke(this, EventArgs.Empty);
						}
						break;
					case Gamemode.Race:
						break;
					case Gamemode.KingOfHill:
						break;
				}
				//Game Timer
				GameTimerClientRpc(Time.deltaTime);
				if (GameTimer < 0f) {
					setState(GameState.Endgame);
					OnStateChanged?.Invoke(this, EventArgs.Empty);
				}
				//Powerups
				if (powerUpsEnabled) {
					if (powerUpSpawnTimer >= powerUpSpawnTimerMax && currentPowerup == null) {
						//spawn PowerUp;
						//get random powerup
						int powerupInt = UnityEngine.Random.Range(0, allPowerUps.Length);
						//get random location
						int randomSpawnArea = UnityEngine.Random.Range(0, powerUpSpawnLocations.Length);
						float randomX = UnityEngine.Random.Range(powerUpSpawnLocations[randomSpawnArea].x, powerUpSpawnLocations[randomSpawnArea].y);
						float randomY = UnityEngine.Random.Range(powerUpSpawnLocations[randomSpawnArea].w, powerUpSpawnLocations[randomSpawnArea].z);
						Vector3 position = new Vector3(randomX, randomY, 0);
						SpawnPowerUpClientRpc(position, powerupInt);
						//instatiate powerup
						powerUpSpawnTimer = 0;
					} else {
						powerUpSpawnTimer += Time.deltaTime;
					}
				}
				break;
			case GameState.Endgame:
				Dictionary<ulong, bool> winners = new Dictionary<ulong, bool>(); ;
				switch (gamemode) {
					case Gamemode.Fighter:
						foreach (KeyValuePair<ulong, int> value in Player.numberOfPlayers) {
							winners.Add(value.Key, value.Value == 1);
						}
						break;
					case Gamemode.Race:
						break;
					case Gamemode.KingOfHill:
						int highestInt = 0;
						foreach (KeyValuePair<ulong, int> value in Player.numberOfPlayers) {
							if (value.Value > highestInt) {
								highestInt = value.Value;
							}
						}

						foreach (KeyValuePair<ulong, int> value in Player.numberOfPlayers) {
							winners.Add(value.Key, value.Value == highestInt);
						}
						break;
				}

				OnEndGame?.Invoke(this, new onEndgameEventArgs {
					winners = winners
				});
				break;
		}
	}


[ClientRpc]
	private void startTimerClientRpc(float deltaTime) {
		StartTimer -= deltaTime;
	}

	[ClientRpc]
	private void GameTimerClientRpc(float deltaTime) {
		if (UseGameTimer) {
			GameTimer -= deltaTime;

		}
	}

	//state sets
	private void setState(GameState newState) {
		currentGameState.Value = newState;
	}

	//state Gets
	public bool isCountDownToStartActive() {
		return currentGameState.Value == GameState.Start;
	}

	public bool IsGameplaying() {
		return currentGameState.Value == GameState.playing;
	}

	public bool isGameOver() {
		return currentGameState.Value == GameState.Endgame;
	}

	public bool isLocalPlayerRead() {
		return isLocalPlayerReady;
	}

	//Timer Gets
	public float GetCountDownTime() {
		return StartTimer;
	}

	public float GetPLayingTimerNormalized() {
		return 1 - (GameTimer / GameTimerMax);
	}

	//Pause Game
	public void PauseGame() {
		isLocalGamePaused = !isLocalGamePaused;
		if (isLocalGamePaused) {
			PauseServerRpc();
			OnLocalPauseGame?.Invoke(this, EventArgs.Empty);
		} else {
			UnPauseServerRpc();
			OnLocalUnPauseGame?.Invoke(this, EventArgs.Empty);
		}
	}

	[ClientRpc]
	private void SpawnPowerUpClientRpc(Vector3 position, int powerupInt) {
		currentPowerup = Instantiate(allPowerUps[powerupInt], position, Quaternion.identity);
	}

	public bool isWaitingToStart() {
		return currentGameState.Value == GameState.WaitingToStart;
	}

	public void collectPowerup() {
		collectPowerupServerRpc();
	}

	[ServerRpc]
	private void collectPowerupServerRpc() {
		collectPoweripClientRpc();
	}

	[ClientRpc]
	private void collectPoweripClientRpc() {
		currentPowerup.TryGetComponent<PowerUpItem>(out var powerUpItem);
		powerUpItem.collect();
	}

	[ServerRpc(RequireOwnership = false)]
	private void PauseServerRpc(ServerRpcParams serverRpcParams = default) {
		PlayerPauseDictionary[serverRpcParams.Receive.SenderClientId] = true;
		TestGamePaused();
	}
	[ServerRpc(RequireOwnership = false)]
	private void UnPauseServerRpc(ServerRpcParams serverRpcParams = default) {
		PlayerPauseDictionary[serverRpcParams.Receive.SenderClientId] = false;
		TestGamePaused();
	}

	private void TestGamePaused() {
		foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
			if (PlayerPauseDictionary.ContainsKey(clientId) && PlayerPauseDictionary[clientId]) {
				isGamePaused.Value = true;
				return;
			}
		}

		isGamePaused.Value = false;
	}

	public Gamemode GetGamemode() {
		return gamemode;
	}

	public override void OnDestroy() {

		GameInput.instance.pausePerformed -= Game_pausePerformed;
		GameInput.instance.onPowerUpPerformed -= GameInput_OnInteractAction;
		currentGameState.OnValueChanged -= CurrentGameState_OnValueChanged;
		isGamePaused.OnValueChanged -= isGamePaused_OnValueChange;

		if (IsServer) {

			OnEndGame -= GameManager.instance.EndRound;
			NetworkManager.Singleton.OnClientDisconnectCallback -= Singleton_OnClientDisconnectCallback;
			NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManager_OnLoadEventCompleted;
		}
	}
}
