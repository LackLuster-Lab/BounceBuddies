using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour {
	[SerializeField] private Button sFXButton;
	[SerializeField] private Button musicButton;
	[SerializeField] private Button CloseButton;
	[SerializeField] private TextMeshProUGUI sFXText;
	[SerializeField] private TextMeshProUGUI musicText;

	public static OptionsUI instance { get; private set; }

	private void Awake() {
		sFXButton.onClick.AddListener(() => {
			SoundManager.instance.ChangeVolume();
			updateVisual();
		});
		musicButton.onClick.AddListener(() => {
			MusicManager.instance.ChangeVolume();
			updateVisual();
		});
		CloseButton.onClick.AddListener(() => {
			Hide();
		});
		instance = this;
	}

	private void Start() {
		GameManager.instance.OnUnPauseGame += GameManger_UnpauseGame;
		updateVisual();
		Hide();
		
	}

	private void GameManger_UnpauseGame(object sender, EventArgs e) {
		Hide();
	}

	private void updateVisual() {
		musicText.text = "Music: " + Mathf.Round(MusicManager.instance.GetVolume() * 10f).ToString();
		sFXText.text = "Sound Effects: " + Mathf.Round(SoundManager.instance.GetVolume() * 10f).ToString();
	}

	public void Show() {
		gameObject.SetActive(true);
	}

	public void Hide() {
		gameObject.SetActive(false);
	}
}
