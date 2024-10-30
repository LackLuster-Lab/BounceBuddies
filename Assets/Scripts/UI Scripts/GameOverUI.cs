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
		//gameObject.SetActive(true); 
		//int alivePlayers = 0;
		//int winner = 1;
		
		//for (int i = 0; i < Player.numberOfPlayers.Count; i++) {
		//	if (Player.numberOfPlayers[i]) {
		//		alivePlayers++;
		//		winner = i;
		//	}
		//}
		//if (alivePlayers > 1) {
			
		//WinnerText.text = "It was a Tie";//some thing to find player username + "Wins"
		//}
  //      else
  //      {
            
		//WinnerText.text = "Player " + (winner + 1) + " Wins";//some thing to find player username + "Wins"
  //      }
    }

	private void Hide() {
		gameObject.SetActive(false);
	}
}
