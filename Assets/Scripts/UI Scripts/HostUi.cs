using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HostUi : MonoBehaviour
{
    [SerializeField] Button MainMenuButton;

	private void Start() {
		NetworkManager.Singleton.OnClientDisconnectCallback += Singleton_OnClientDisconnectCallback;

		MainMenuButton.onClick.AddListener(() => {
			NetworkManager.Singleton.Shutdown();
			Loader.Load(Loader.scenes.MainMenu);
		});
		Hide();
	}

	private void Singleton_OnClientDisconnectCallback(ulong clientId) {
		if (clientId == NetworkManager.ServerClientId) {
			Show();
		}
	}

	private void Hide() {
        gameObject.SetActive(false);
    }
    private void Show() {
        gameObject.SetActive(true);
    }
}
