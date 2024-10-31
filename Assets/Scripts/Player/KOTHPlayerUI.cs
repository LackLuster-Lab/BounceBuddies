using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class KOTHPlayerUI : PlayerUI {
	[SerializeField] private TextMeshProUGUI text;




	private void Player_updatePoints(object sender, Player.PointsChangeEventArgs e) {
		text.text = e.newPoints.ToString();
	}

	public override void setPlayer(Player inputPlayer) {
		player = inputPlayer;
		player.PointsChange += Player_updatePoints;
		player.UpdateIcon += Player_UpdateIcon;
	}
}
