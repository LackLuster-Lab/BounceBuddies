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
		Player.OnAnyPlayerHitWall += Player_OnPlayerHitWall;
		MainMenuUI.OnButtonPress += OnButtonPress;
		OptionsUI.OnButtonPress += OnButtonPress;
		LobbyUI.OnButtonPress += OnButtonPress;
		LobbyCreateUI.OnButtonPress += OnButtonPress;
		JoinCodeInput.OnButtonPress += OnButtonPress;
		LobbyMessageUI.OnButtonPress += OnButtonPress;
		HostUi.OnButtonPress += OnButtonPress;
		CharacterSelectUI.OnButtonPress += OnButtonPress;
		CharacterSelectColor.OnButtonPress += OnButtonPress;
		ReadyUI.OnButtonPress += OnButtonPress;
		EndGamUI.OnButtonPress += OnButtonPress;
		PowerUpFunctions.instance.OnExplosion += OnExplosion;
		PowerUpFunctions.instance.OnProjectile += onProjectile;
	}

	//play sounds
	private void Player_OnPlayerHitWall(object sender, System.EventArgs e) {
		Player player = sender as Player;
		PlaySound(sounds.PlayerHitWall, player.transform.position, sounds.PlayerHitWallVolume); 
	}

	private void OnButtonPress(object sender, System.EventArgs e) {
		PlaySound(sounds.buttonPress, Camera.allCameras[0].transform.position, sounds.buttonPressVolume);
	}

	private void OnExplosion(object sender, System.EventArgs e) {
		PlaySound(sounds.Explosion, Camera.allCameras[0].transform.position, sounds.ExplosionVolume);
	}

	private void onProjectile(object sender, System.EventArgs e) {
		PlaySound(sounds.Projectile, Camera.allCameras[0].transform.position, sounds.ProjectileVolume);
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

	private void OnDestroy() {

		Player.OnAnyPlayerHitWall -= Player_OnPlayerHitWall;
		MainMenuUI.OnButtonPress -= OnButtonPress;
		OptionsUI.OnButtonPress -= OnButtonPress;
		LobbyUI.OnButtonPress -= OnButtonPress;
		LobbyCreateUI.OnButtonPress -= OnButtonPress;
		JoinCodeInput.OnButtonPress -= OnButtonPress;
		LobbyMessageUI.OnButtonPress -= OnButtonPress;
		HostUi.OnButtonPress -= OnButtonPress;
		CharacterSelectUI.OnButtonPress -= OnButtonPress;
		CharacterSelectColor.OnButtonPress -= OnButtonPress;
		ReadyUI.OnButtonPress -= OnButtonPress;
		EndGamUI.OnButtonPress -= OnButtonPress;
	}
}
