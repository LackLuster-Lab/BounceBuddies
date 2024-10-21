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

	private void Awake() {
		PlayButton.onClick.AddListener(() => {
			Loader.Load(Loader.scenes.LobbyScene);
		});
		QuitButton.onClick.AddListener(() => {
			Application.Quit();
		});
		OptionsButton.onClick.AddListener(() => {
			OptionsMenu.Show(() => { PlayButton.Select(); });
		});
		PlayButton.Select();
		Time.timeScale = 1f;
	}
}
