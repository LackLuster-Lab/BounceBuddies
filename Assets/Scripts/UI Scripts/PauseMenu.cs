using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{

	[SerializeField] private Button mainMenuButton;
	[SerializeField] private Button ResumeButton;

	private void Awake() {
		ResumeButton.onClick.AddListener(() => { 
			GameManager.instance.PauseGame();
		});
		mainMenuButton.onClick.AddListener(() => {
			Loader.Load(Loader.scenes.MainMenu);
		});
	}

	private void Start() {
		GameManager.instance.OnPauseGame += Show;
		GameManager.instance.OnUnPauseGame += Hide;

		Hide(this, EventArgs.Empty);
	}

	private void Show(object sender, System.EventArgs e) {
		gameObject.SetActive(true);
	}

	




	private void Hide(object sender, System.EventArgs e) {
		gameObject.SetActive(false);
	}
}
