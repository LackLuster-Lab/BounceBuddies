using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	[SerializeField] private SoundsSO sounds;
	private void Start() {
		Player.Instance.OnPlayerHitWall += Player_OnPlayerHitWall;
	}

	private void Player_OnPlayerHitWall(object sender, System.EventArgs e) {
		Player player = (Player)sender;
		PlaySound(sounds.PlayerHitWall, player.transform.position, sounds.PlayerHitWallVolume); 
	}

	private void PlaySound(AudioClip audio, Vector3 position, float Volume = 1f) {
		AudioSource.PlayClipAtPoint(audio, position, Volume);
	}
}
