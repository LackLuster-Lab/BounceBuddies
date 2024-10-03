using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Search;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
	[SerializeField] private GameObject UI;

	private void Start() {
		GameManager.instance.OnStateChanged += GameManager_OnStateChanged;

		Hide();
	}

	private void GameManager_OnStateChanged(object sender, System.EventArgs e) {
		if (GameManager.instance.isGameOver()) {
			Show();
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
}
