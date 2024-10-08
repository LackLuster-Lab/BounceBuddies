using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {
	[SerializeField] private Player player;
	[SerializeField] private Slider HealthBarUI;
	[SerializeField] private Image powerupIcon;
	[SerializeField] private Sprite EmptyIcon;

	private void Start() {
	}

	private void Player_UpdateIcon(object sender, Player.UpdateIconArgs e) {
		if (e.Icon != null) {
			powerupIcon.sprite = e.Icon;
		} else {
			powerupIcon.sprite = EmptyIcon;
		}
	}

	private void Player_HealthChange(object sender, Player.HealthChangeEventArgs e) {
		HealthBarUI.value = e.newHealth;
	}

	public void setPlayer(Player inputPlayer) {
		player = inputPlayer;
		player.HealthChange += Player_HealthChange;
		player.UpdateIcon += Player_UpdateIcon;
	}
}
