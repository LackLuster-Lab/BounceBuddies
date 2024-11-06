using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class RacePlayerUI : PlayerUI {






	public override void setPlayer(Player inputPlayer) {
		player = inputPlayer;
		player.UpdateIcon += Player_UpdateIcon;
	}
}
