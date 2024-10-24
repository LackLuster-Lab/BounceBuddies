using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour {

	[SerializeField] private int playerIndex;
	[SerializeField] GameObject readyGameObject;
	[SerializeField] playerVisual playerVisual;
	[SerializeField] Button KickPlayer;
	[SerializeField] private TextMeshPro playerNameText;

	private void Awake() {
		KickPlayer.onClick.AddListener(() => {
			PlayerData playerData = MultiplayerManager.instance.GetPlayerDatafromPlayerIndex(playerIndex);
			GameLobby.Instance.KickPlayer(playerData.playerId.ToString());
			MultiplayerManager.instance.KickPlayer(playerData.clientId);
		});
	}

	private void Start() {
		MultiplayerManager.instance.OnPlayerDataNetworkListChanged += Instance_OnPlayerDataNetworkListChanged;
		CharacterSelectReady.instance.OnReadyChange += Instance_OnReadyChange;

		KickPlayer.gameObject.SetActive(NetworkManager.Singleton.IsServer);
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

			playerNameText.text = playerData.playerName.ToString();

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
