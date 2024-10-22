using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ErrorMessageUI : MonoBehaviour{

	[SerializeField] private TextMeshProUGUI ErrorMessageText;
	[SerializeField] private Button closeButton;

	private void Awake() {
		closeButton.onClick.AddListener(Hide);
	}

	private void Start() {
		MultiplayerManager.instance.OnFailedToJoinGame += Instance_OnFailedToJoinGame;
		Hide();
	}

	private void Instance_OnFailedToJoinGame(object sender, System.EventArgs e) {
		Show();

		ErrorMessageText.text = NetworkManager.Singleton.DisconnectReason;

		if (ErrorMessageText.text == "") {
			ErrorMessageText.text = "Failed to connect";
		}
	}

	private void Show() {
		gameObject.SetActive(true);
	}

	private void Hide() {
		gameObject.SetActive(false);
	}

	private void OnDestroy() {
		MultiplayerManager.instance.OnFailedToJoinGame -= Instance_OnFailedToJoinGame;
	}
}
