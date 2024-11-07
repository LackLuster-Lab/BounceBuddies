using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractUI : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI Interact;


	private void Start() {
		RoundManager.instance.OnLocalplayerReady += KitchenGameManager_OnLocalPlayerReadyChanged;

		UpdateVisual();

		Show();
	}

	private void Show() {
		gameObject.SetActive(true);
	}

	private void UpdateVisual() {
		Interact.text = "Press " + GameInput.instance.GetBindingText(GameInput.Binding.Power_Up) + " to Continue";
	}

	private void KitchenGameManager_OnLocalPlayerReadyChanged(object sender, System.EventArgs e) {
		if (RoundManager.instance.isLocalPlayerRead()) {
			Hide();
		}
	}

	private void Hide() {
		gameObject.SetActive(false);
	}

	private void GameInput_OnBindingRebind(object sender, System.EventArgs e) {
		UpdateVisual();
	}
}
