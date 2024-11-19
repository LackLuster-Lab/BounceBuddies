using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class WinPlayer : MonoBehaviour {

	[SerializeField] private int playerIndex;
	[SerializeField] GameObject readyGameObject;
	[SerializeField] playerVisual playerVisual;
	[SerializeField] private TextMeshPro playerNameText;
	[SerializeField] private TextMeshPro pointsText;
	private void Awake() {
	}

	private void Start() {
		MultiplayerManager.instance.OnPlayerDataNetworkListChanged += Instance_OnPlayerDataNetworkListChanged;


		UpdatePlayer();
	}

	private void Instance_OnReadyChange(object sender, System.EventArgs e) {
		UpdatePlayer();
	}

	private void Instance_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e) {
		UpdatePlayer();
	}

	private void UpdatePlayer() {
		if (MultiplayerManager.instance.IsPlayerIndexConnected(playerIndex)) {
			Show();
			PlayerData playerData = MultiplayerManager.instance.GetPlayerDatafromPlayerIndex(playerIndex);


			playerNameText.text = playerData.playerName.ToString();

			pointsText.text = "(" + playerData.points.ToString()+ " points)";

			playerVisual.setPlayerColor(MultiplayerManager.instance.getPlayerColor(playerData.ColorId));

		} else {
			Hide();
		}
	}

	private void Show() {
		gameObject.SetActive(true);
	}

	private void Hide() {
		gameObject.SetActive(false);
	}

	private void OnDestroy() {

		MultiplayerManager.instance.OnPlayerDataNetworkListChanged -= Instance_OnPlayerDataNetworkListChanged;
	}
}
