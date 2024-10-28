using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectColor : MonoBehaviour {
	[SerializeField] int ColorId;
	[SerializeField] Image image;
	[SerializeField] GameObject selectedGameObject;

	private void Awake() {
		GetComponent<Button>().onClick.AddListener(() => {
			MultiplayerManager.instance.ChangePlayerColor(ColorId);
		});
	}

	private void Start() {
		MultiplayerManager.instance.OnPlayerDataNetworkListChanged += Instance_OnPlayerDataNetworkListChanged;
		image.color = MultiplayerManager.instance.getPlayerColor(ColorId);
		UpdateIsSelected();
		 
	}

	private void Instance_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e) {
		UpdateIsSelected();
	}

	private void UpdateIsSelected() {
		if (MultiplayerManager.instance.GetPlayerData().ColorId == ColorId) {
			selectedGameObject.SetActive(true);
		} else {
			selectedGameObject.SetActive(false);
		}
    }

	private void OnDestroy() {

		MultiplayerManager.instance.OnPlayerDataNetworkListChanged -= Instance_OnPlayerDataNetworkListChanged;
	}
}
