using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
	[SerializeField] private Button PlayButton;
	[SerializeField] private Button QuitButton;

	private void Awake() {
		PlayButton.onClick.AddListener(() => {
			Loader.Load(Loader.scenes.GameScene);
		});
		QuitButton.onClick.AddListener(() => {
			Application.Quit();
		});
	}
}
