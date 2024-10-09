using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	public static SoundManager instance { get; private set; }
	[SerializeField] private SoundsSO sounds;
	private float volume = 1f;
	private const string PLAYER_PREF_SFX_VOLUME = "SFXVOLUME";

	private void Awake() {
		instance = this;
		volume = PlayerPrefs.GetFloat(PLAYER_PREF_SFX_VOLUME, 1f);
	}

	private void Start() {
		if (Player.Instance != null) {
			Player.Instance.OnPlayerHitWall += Player_OnPlayerHitWall;
		}
	}

	//play sounds
	private void Player_OnPlayerHitWall(object sender, System.EventArgs e) {
		Player player = (Player)sender;
		PlaySound(sounds.PlayerHitWall, player.transform.position, sounds.PlayerHitWallVolume); 
	}

	//sound manager functions
	private void PlaySound(AudioClip audio, Vector3 position, float VolumeMultiplier = 1f) {
		AudioSource.PlayClipAtPoint(audio, position, VolumeMultiplier * volume);
	}

	public void ChangeVolume() {
		volume += .1f;
		if (volume > 1.05f) {
			volume = 0f;
		}

		PlayerPrefs.SetFloat(PLAYER_PREF_SFX_VOLUME, volume);
		PlayerPrefs.Save();
	}

	public float GetVolume() {
		return volume;
	}
}
