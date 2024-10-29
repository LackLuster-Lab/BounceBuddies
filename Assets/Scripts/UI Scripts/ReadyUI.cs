using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class ReadyUI : MonoBehaviour {
	[SerializeField] Button readyButton;

	private void Awake() {
		readyButton.onClick.AddListener(() => {
			Ready.instance.SetPlayerReady();
		});
	}

	private void Update() {
		Lobby lobby = GameLobby.Instance.GetLobby();
	}
}
