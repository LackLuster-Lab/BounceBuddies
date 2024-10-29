using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class InbetweenPlayer : MonoBehaviour {

	[SerializeField] private int playerIndex;
	[SerializeField] GameObject readyGameObject;
	[SerializeField] playerVisual playerVisual;
	[SerializeField] private TextMeshPro playerNameText;
	[SerializeField] private TextMeshPro PointsEarned;

	private void Awake() {
	}

	private void Start() {
		MultiplayerManager.instance.OnPlayerDataNetworkListChanged += Instance_OnPlayerDataNetworkListChanged;
		Ready.instance.OnReadyChange += Instance_OnReadyChange;

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
			readyGameObject.SetActive(Ready.instance.IsPlayerReady(playerData.clientId));

			playerNameText.text = playerData.playerName.ToString();

			playerVisual.setPlayerColor(MultiplayerManager.instance.getPlayerColor(playerData.ColorId));
			PointsEarned.text = GameManager.instance.points[playerData.clientId].ToString();
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
