using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : NetworkBehaviour
{
	[SerializeField] private Image timerImage;

	private void Update() {
		if (!IsServer) return;
		UpdateTimerClientRpc();
	}

	[ClientRpc]
	private void UpdateTimerClientRpc() {
		timerImage.fillAmount = RoundManager.instance.GetPLayingTimerNormalized();
		
	}
}
