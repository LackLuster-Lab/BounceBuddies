using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour
{
	[SerializeField] private Button closeButton;
	[SerializeField] private Button createPublicButton;
	[SerializeField] private Button createPrivateButton;
	[SerializeField] private TMP_InputField lobbyNameInputField;
	public static event EventHandler OnButtonPress;

	private void Awake() {
		createPublicButton.onClick.AddListener(() => {
			GameLobby.Instance.CreateLobby(lobbyNameInputField.text, false);
			OnButtonPress?.Invoke(this, EventArgs.Empty);
		});

		createPrivateButton.onClick.AddListener(() => {
			GameLobby.Instance.CreateLobby(lobbyNameInputField.text, true);
			OnButtonPress?.Invoke(this, EventArgs.Empty);
		});

		closeButton.onClick.AddListener(() => {
			Hide();
			OnButtonPress?.Invoke(this, EventArgs.Empty);
		});
	}
	private void Start() {
		Hide();
	}

	private void Hide() {
		gameObject.SetActive(false);
	}

	public void Show() {
		gameObject.SetActive(true);
		createPrivateButton.Select();
	}
}
