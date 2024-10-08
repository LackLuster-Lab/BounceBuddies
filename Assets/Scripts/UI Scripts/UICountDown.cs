using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UICountDown : MonoBehaviour {
	[SerializeField] private TextMeshProUGUI CountDownText;

	private void Start() {
		RoundManager.instance.OnStateChanged += GameManager_OnStateChanged;

		Hide();
	}

	private void GameManager_OnStateChanged(object sender, System.EventArgs e) {
		if (RoundManager.instance.isCountDownToStartActive()) {
			Show();
		} else {
			Hide();
		}
	}

	private void Update() {
		CountDownText.text = Mathf.Ceil(RoundManager.instance.GetCountDownTime()).ToString();
	}

	private void Show() {
		gameObject.SetActive(true);
	}

	private void Hide() {
		gameObject.SetActive(false);
	}
}
