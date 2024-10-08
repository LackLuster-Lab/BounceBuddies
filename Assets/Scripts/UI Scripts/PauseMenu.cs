using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{

	[SerializeField] private Button mainMenuButton;
	[SerializeField] private Button ResumeButton;
	[SerializeField] private Button OptionsButton;
	[SerializeField] private OptionsUI OptionsMenu;

	private EventHandler OnOptionsOpen;

	private void Awake() {
		ResumeButton.onClick.AddListener(() => { 
			RoundManager.instance.PauseGame();
		});
		mainMenuButton.onClick.AddListener(() => {
			Loader.Load(Loader.scenes.MainMenu);
		});
		OptionsButton.onClick.AddListener(() => {
			Hide(this,EventArgs.Empty);
			OptionsMenu.Show(ShowWindow);
			RoundManager.instance.OnUnPauseGame += OptionsMenu.GameManger_UnpauseGame;
		});
	}

	private void Start() {
		RoundManager.instance.OnPauseGame += Show;
		RoundManager.instance.OnUnPauseGame += Hide;

		Hide(this, EventArgs.Empty);
	}

	private void Show(object sender, System.EventArgs e) {
		gameObject.SetActive(true);
		ResumeButton.Select();
	}

	private void ShowWindow() {
		Show(this, EventArgs.Empty);
	}

	private void Hide(object sender, System.EventArgs e) {
		gameObject.SetActive(false);
	}
}
