using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour {
	[SerializeField] Button mainMenuButton;
	[SerializeField] Button createLobbyButton;
	[SerializeField] Button quickJoinButton;

	private void Awake() {
		mainMenuButton.onClick.AddListener(() => {
			Loader.Load(Loader.scenes.MainMenu);
		});
		createLobbyButton.onClick.AddListener(() => {
			GameLobby.Instance.CreateLobby("LobbyName", false);
		});
		quickJoinButton.onClick.AddListener(() => {
			GameLobby.Instance.QuickJoin();
		});
	}
}
