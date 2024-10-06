using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
	public static MusicManager instance { get; private set; }
	[SerializeField] private AudioSource music;
	[SerializeField] private float baseVolume;
	private float volume = 1f;
	private const string PLAYER_PREF_MUSIC_VOLUME = "MUSICVOLUME";

	private void Awake() {
		instance = this;
		volume = PlayerPrefs.GetFloat(PLAYER_PREF_MUSIC_VOLUME, 1f);
		music.volume = volume * baseVolume;
	}

	public void ChangeVolume() {
		volume += .1f;
		if (volume > 1.05f) {
			volume = 0f;
		}
		music.volume = volume * baseVolume;
		PlayerPrefs.SetFloat(PLAYER_PREF_MUSIC_VOLUME, volume);
		PlayerPrefs.Save();
	}

	public float GetVolume() {
		return volume;
	}
}
