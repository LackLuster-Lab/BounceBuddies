using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMultiplayerUI : MonoBehaviour
{

	private void Start() {
		RoundManager.instance.OnMultiplayerPauseGame += Instance_OnMultiplayerPauseGame;
		RoundManager.instance.OnMultiplayerUnPauseGame += Instance_OnMultiplayerUnPauseGame;
		hide();
	}

	private void Instance_OnMultiplayerUnPauseGame(object sender, System.EventArgs e) {
		hide();
	}

	private void Instance_OnMultiplayerPauseGame(object sender, System.EventArgs e) {
		show();
	}

	private void show() {
		gameObject.SetActive(true);
	}
	private void hide() {
		gameObject.SetActive(false);
	}
}
