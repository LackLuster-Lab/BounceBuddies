using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestingCharacterSelectUI : MonoBehaviour
{
    [SerializeField] Button ReadyButton;

	private void Start() {
		ReadyButton.onClick.AddListener(CharacterSelectReady.instance.SetPlayerReady);
	}
}
