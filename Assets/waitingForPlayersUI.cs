using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waitingForPlayersUI : MonoBehaviour
{

	private void Start() {
		RoundManager.instance.OnLocalplayerReady += Round_OnLocalPlayerReady;
		RoundManager.instance.OnStateChanged += Round_onCountDown;
		Hide();
	}

	private void Round_onCountDown(object sender, System.EventArgs e) {
		if (RoundManager.instance.isCountDownToStartActive()) {
			Hide();
		}
	}

	private void Round_OnLocalPlayerReady(object sender, System.EventArgs e) {
		if (RoundManager.instance.isLocalPlayerRead()) {
			Show();
		}
	}

	// Start is called before the first frame update
	void Show() {
        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Hide() {
        gameObject.SetActive(false);
    }
}
