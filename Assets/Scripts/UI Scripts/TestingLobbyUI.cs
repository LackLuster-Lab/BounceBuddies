using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestingLobbyUI : MonoBehaviour {
	[SerializeField] Button createGameButton;
	[SerializeField] Button joinGameButton;

	private void Awake() {
		createGameButton.onClick.AddListener(() => {
			MultiplayerManager.instance.StartHost();
			Loader.LoadNetwork(Loader.scenes.CharacterSelectScene);
		});

		joinGameButton.onClick.AddListener(() => {
			MultiplayerManager.instance.StartClient();
		});
	}
}
