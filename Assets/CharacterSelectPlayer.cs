using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectPlayer : MonoBehaviour {

	[SerializeField] private int playerIndex;
	[SerializeField] GameObject readyGameObject;
	[SerializeField] playerVisual playerVisual;

	private void Start() {
		MultiplayerManager.instance.OnPlayerDataNetworkListChanged += Instance_OnPlayerDataNetworkListChanged;
		CharacterSelectReady.instance.OnReadyChange += Instance_OnReadyChange;
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
			readyGameObject.SetActive(CharacterSelectReady.instance.IsPlayerReady(playerData.clientId));

			playerVisual.setPlayerColor(MultiplayerManager.instance.getPlayerColor(playerIndex));
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
}
