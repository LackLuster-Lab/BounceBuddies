using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HostUi : MonoBehaviour
{
    [SerializeField] Button MainMenuButton;

	private void Awake() {
		MainMenuButton.onClick.AddListener(() => {
			Loader.Load(Loader.scenes.MainMenu);
		});
	}

	private void Start() {
		NetworkManager.Singleton.OnClientDisconnectCallback += Singleton_OnClientDisconnectCallback;


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

	private void OnDestroy() {

		NetworkManager.Singleton.OnClientDisconnectCallback -= Singleton_OnClientDisconnectCallback;
	}
}
