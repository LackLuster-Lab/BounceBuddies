using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour {

	public static GameManager instance{get; private set;}

	int Rounds = 5;
	bool isPowerUps = true;
	float roundTimer = 60;


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
		Loader.LoadNetwork(Loader.scenes.GameScene);
		Rounds--;
	}

	public void EndRound(object sender, RoundManager.onEndgameEventArgs e) {
		//between step
		//update points
		foreach (KeyValuePair<ulong, bool> player in e.winners) {
			if (player.Value) {
				PlayerData data = MultiplayerManager.instance.GetPlayerDatafromClientId(player.Key);
				data.points = data.points + 1;
			}
		}
		if (Rounds <= 0) {
			//load win scene
		} else {
			//load inbetween scene
			Loader.LoadNetwork(Loader.scenes.InbetweenScene);
		}
	}

	public void nextRound() {
		loadScene();
	}
}
