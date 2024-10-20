using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{

	[SerializeField] private TextMeshProUGUI WinnerText;

	private void Start() {
		RoundManager.instance.OnStateChanged += GameManager_OnStateChanged;

		Hide();
	}

	private void GameManager_OnStateChanged(object sender, System.EventArgs e) {
		if (RoundManager.instance.isGameOver()) {
			Show();
		} else {
			Hide();
		}
	}


	private void Show() {
		gameObject.SetActive(true);
		WinnerText.text = "Player 1 wins";//some thing to find player username + "Wins"
	}

	private void Hide() {
		gameObject.SetActive(false);
	}
}
