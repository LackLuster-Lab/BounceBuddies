using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
	[SerializeField] private Button PlayButton;
	[SerializeField] private Button QuitButton;
	[SerializeField] private Button OptionsButton;
	[SerializeField] private OptionsUI OptionsMenu;
	public static event EventHandler OnButtonPress;
	private void Awake() {
		PlayButton.onClick.AddListener(() => {
			OnButtonPress?.Invoke(this, EventArgs.Empty);
			Loader.Load(Loader.scenes.LobbyScene);
		});
		QuitButton.onClick.AddListener(() => {
			Application.Quit();
		});
		OptionsButton.onClick.AddListener(() => {
			OnButtonPress?.Invoke(this, EventArgs.Empty);
			OptionsMenu.Show(() => { PlayButton.Select(); });
		});
		PlayButton.Select();
		Time.timeScale = 1f;
	}
}
