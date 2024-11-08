using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class ReadyUI : MonoBehaviour {
	[SerializeField] Button readyButton;
	public static event EventHandler OnButtonPress;

	private void Awake() {
		readyButton.onClick.AddListener(() => {
			OnButtonPress?.Invoke(this, EventArgs.Empty);
			Ready.instance.SetPlayerReady();
		});
	}

	private void Start() {
		readyButton.Select();
	}

	private void Update() {
		Lobby lobby = GameLobby.Instance.GetLobby();
	}
}
