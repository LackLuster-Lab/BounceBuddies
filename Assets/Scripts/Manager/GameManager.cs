using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour {

	public static GameManager instance{get; private set;}

	public enum gamemode {
		All,
		Fighter,
		Race,
		KingOfTheHill,
		Demo
	}

	public enum map {
		All,
		Dungeon
	}

	map selectedMap = map.All;
	gamemode selectedGameMode = gamemode.Demo;
	int Rounds = 3;
	bool isPowerUps = true;
	float roundTimer = 60;

	public List<PlayerData> winners;

	public override void OnNetworkSpawn() {
		if (IsServer) {
			NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
		}
	}

	private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut) {
		RoundManager.instance.SetValues(isPowerUps, roundTimer);
	}

	private void Awake() {
		instance = this;
		DontDestroyOnLoad(this.gameObject);
	}

	public void startGame() {
		loadScene();
	}

	public void loadScene() {
		switch (selectedGameMode) {
			case gamemode.All:
				int gamemodeRandom = UnityEngine.Random.Range(0, 3);
				switch (gamemodeRandom) {
					case 0:
						loadFighter();
						break;
					case 1:
						loadKOTH();
						break;
					case 2:
						loadRace();
						break;
				}
				break;
			case gamemode.Fighter:
				loadFighter();
				break;
			case gamemode.Race:
				loadRace();
				break;
			case gamemode.KingOfTheHill:
				loadKOTH();
				break;
			case gamemode.Demo:
				loadDemo();
				break;

		}
		Rounds--;
	}

	private void loadDemo() {
		switch (Rounds) {
			case 3:
				loadFighter();
				break;
			case 2:
				loadKOTH();
				break;
			case 1:
				loadRace();
				break;
		}
	}

	public void EndRound(object sender, RoundManager.onEndgameEventArgs e) {
		//between step
		//update points
		foreach (KeyValuePair<ulong, bool> player in e.winners) {
			if (player.Value) {
				MultiplayerManager.instance.updatePointsServerRpc(1, player.Key);
			}
		}
		if (Rounds <= 0) {
			//load win scene
			sortWinners(e.winners.Keys.ToList<ulong>());
			Loader.LoadNetwork(Loader.scenes.WinScene);
		} else {
			//load inbetween scene
			Loader.LoadNetwork(Loader.scenes.InbetweenScene);
		}
	}

	public void nextRound() {
		loadScene();
	}

	public void sortWinners(List<ulong> list) {
		MultiplayerManager.instance.sortPlayersServerRpc();
	}

	private void loadFighter() {
		int mapRandom;
		switch (selectedMap) {
			case GameManager.map.All:
				mapRandom = UnityEngine.Random.Range(0, 0);
				switch (mapRandom) {
					case 0:
						Loader.LoadNetwork(Loader.scenes.FighterDungeon);
						break;
				}
				break;
			case GameManager.map.Dungeon:
				mapRandom = UnityEngine.Random.Range(0, 0);
				switch (mapRandom) {
					case 0:
						Loader.LoadNetwork(Loader.scenes.FighterDungeon);
						break;
				}
				break;
		}
	}

	private void loadKOTH() {
		int mapRandom;
		switch (selectedMap) {
			case GameManager.map.All:
				mapRandom = UnityEngine.Random.Range(0, 0);
				switch (mapRandom) {
					case 0:
						Loader.LoadNetwork(Loader.scenes.KOTHDungeon);
						break;
				}
				break;
			case GameManager.map.Dungeon:
				mapRandom = UnityEngine.Random.Range(0, 0);
				switch (mapRandom) {
					case 0:
						Loader.LoadNetwork(Loader.scenes.KOTHDungeon);
						break;
				}
				break;
		}
	}

	private void loadRace() {
		int mapRandom;
		switch (selectedMap) {
			case GameManager.map.All:
				mapRandom = UnityEngine.Random.Range(0, 0);
				switch (mapRandom) {
					case 0:
						Loader.LoadNetwork(Loader.scenes.RaceDungeon);
						break;
				}
				break;
			case GameManager.map.Dungeon:
				mapRandom = UnityEngine.Random.Range(0, 0);
				switch (mapRandom) {
					case 0:
						Loader.LoadNetwork(Loader.scenes.RaceDungeon);
						break;
				}
				break;
		}
	}
}
