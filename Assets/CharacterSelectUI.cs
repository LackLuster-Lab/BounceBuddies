using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour {
	[SerializeField] Button mainMenuButton;
	[SerializeField] Button readyButton;

	private void Awake() {
		mainMenuButton.onClick.AddListener(() => {
			NetworkManager.Singleton.Shutdown();
			Loader.Load(Loader.scenes.MainMenu);
		});
		readyButton.onClick.AddListener(() => {
			CharacterSelectReady.instance.SetPlayerReady();
		});
	}
}