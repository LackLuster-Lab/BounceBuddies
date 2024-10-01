using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
	[SerializeField] private Player player;
	[SerializeField] private Slider HealthBarUI;
	[SerializeField] private Image powerupIcon;
	private void Start() {
		player.HealthChange += Player_HealthChange;
		player.UpdateIcon += Player_UpdateIcon;
	}

	private void Player_UpdateIcon(object sender, Player.UpdateIconArgs e) {
		powerupIcon.sprite = e.Icon;
	}

	private void Player_HealthChange(object sender, Player.HealthChangeEventArgs e) {
		HealthBarUI.value = e.newHealth;
	}
}
