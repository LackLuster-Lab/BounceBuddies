using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
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

	public void EndRound(object sender, System.EventArgs e) {
		//between step
		if (Rounds <= 0) {
			//load win scene
		} else {
			Loader.LoadNetwork(Loader.scenes.GameScene2);
		}
	}

	public void nextRound() {
		loadScene();
	}
}
