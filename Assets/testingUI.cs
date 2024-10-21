using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class testingUI : MonoBehaviour
{
	[SerializeField] Button HostButton;
	[SerializeField] Button ClientButton;
	[SerializeField] Button parent;

	private void Awake() {
		HostButton.onClick.AddListener(() => {
			MultiplayerManager.instance.StartHost();
			Hide();
		});
		ClientButton.onClick.AddListener(() => {
			MultiplayerManager.instance.StartClient();
			Hide();
		});
	}

	private void Hide() {
		gameObject.SetActive(false);
	}
}
