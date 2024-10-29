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

	public Dictionary<ulong,int> points;

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
		points = new Dictionary<ulong, int>();
		List<ulong> clientIds = NetworkManager.Singleton.ConnectedClientsIds.ToList();
		foreach (ulong clientId in clientIds) {
			points.Add(clientId, 0);
		}
	}

	public void loadScene() {
		Loader.LoadNetwork(Loader.scenes.GameScene);
		Rounds--;
	}

	public void EndRound(object sender, RoundManager.onEndgameEventArgs e) {
		//between step
		//update points
		for (int i = 0; i < e.winners.Count; i++) {
			if (e.winners[i]) {
				//points[i]++;
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
