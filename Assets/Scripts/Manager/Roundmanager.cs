using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class RoundManager : NetworkBehaviour {

	public static RoundManager instance {get; private set;}


	//all different variables for game modes
	private enum GameState {
		WaitingToStart,
		Start,
		playing,
		Endgame
	}
	private enum Gamemode {
		Fighter,
		Race,
		KingOfHill
	}
	public enum DamageType {
		None,
		Stocks,
		Percentage
	}

	//Gamemode Settings
	[SerializeField] Gamemode gamemode = Gamemode.Fighter;//TODO implement
	[SerializeField] public DamageType damageType = DamageType.Percentage;//TODO implement
	[SerializeField] bool powerUpsEnabled;
	[SerializeField] private float powerUpSpawnTimerMax = 4f;
	[SerializeField] GameObject[] allPowerUps;
	private float powerUpSpawnTimer;
	private GameObject currentPowerup;
	private GameState currentGameState;

	//all map specific things
	[SerializeField] Vector4[] powerUpSpawnLocations;// X-Left, Y-Right, Z-Top, W-Bottom
	private List<Player> currentPlayer;
	//Timers
	private float WaitingToStartTimer = 1f;
	private float StartTimer = 3f;
	public bool isGamePaused = false;
	[SerializeField] private bool UseGameTimer = true;
	[SerializeField] private float GameTimerMax = 60f;
	[SerializeField] private float GameTimer = 60f;

	//Events
	public event EventHandler OnStateChanged;
	public event EventHandler OnPauseGame;
	public event EventHandler OnUnPauseGame;

	private PowerUpItem PowerupToManage;

	private int currentPlayers;

	public void addPlayer() {
		currentPlayers++;
	}

	public void Awake() {
		currentGameState = GameState.WaitingToStart;
		GameTimer = GameTimerMax;
		instance = this;
	}

	public void Start() {
		GameInput.instance.pausePerformed += Game_pausePerformed;
	}

	private void Game_pausePerformed(object sender, EventArgs e) {
		PauseGame();
	}

	public void Update() {
		if (!IsServer) { return; }
		switch (currentGameState) {
			case GameState.WaitingToStart:
				WaitingToStartTimer -= Time.deltaTime;
				if (WaitingToStartTimer < 0f) {
					setStateClientRpc(GameState.Start);
					OnStateChanged?.Invoke(this, EventArgs.Empty);
				}
				break;
			case GameState.Start:
				StartTimer -= Time.deltaTime;
				if (StartTimer < 0f) {
					setStateClientRpc(GameState.playing);
					OnStateChanged?.Invoke(this, EventArgs.Empty);
				}
				break;
			case GameState.playing:
				//GamemodeUpdates
				switch (gamemode) {
					case Gamemode.Fighter:
						break;
					case Gamemode.Race:
						break;
					case Gamemode.KingOfHill:
						break;
				}
				//Game Timer
				if (UseGameTimer) {
					GameTimer -= Time.deltaTime;
					if (GameTimer < 0f) {
						setStateClientRpc(GameState.Endgame);
						OnStateChanged?.Invoke(this, EventArgs.Empty);
					}
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
				break;
		}
	}

	//state sets
	[ClientRpc]
	private void setStateClientRpc(GameState newState) {
		currentGameState = newState;
	}

	//state Gets
	public bool isCountDownToStartActive() {
		return currentGameState == GameState.Start;
	}

	public bool IsGameplaying() {
		return currentGameState == GameState.playing;
	}

	public bool isGameOver() {
		return currentGameState == GameState.Endgame;
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
		isGamePaused = !isGamePaused;
		if (isGamePaused) {
			Time.timeScale = 1.0f;
			OnUnPauseGame?.Invoke(this, EventArgs.Empty);
		} else {
			Time.timeScale = 0.0f;
			OnPauseGame?.Invoke(this, EventArgs.Empty);
		}
	}

	[ClientRpc]
	private void SpawnPowerUpClientRpc(Vector3 position, int powerupInt) {
		currentPowerup = Instantiate(allPowerUps[powerupInt], position, Quaternion.identity);
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


}
