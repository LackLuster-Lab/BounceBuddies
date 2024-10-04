using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
	[SerializeField] private Image timerImage;

	private void Update() {
		timerImage.fillAmount = GameManager.instance.GetPLayingTimerNormalized();
	}
}
