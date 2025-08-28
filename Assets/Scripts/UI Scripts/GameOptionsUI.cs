using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GameOptionsUI : MonoBehaviour {

	//buttons
	[SerializeField] private Button CloseButton;

	//text Values
	public static event EventHandler OnButtonPress;

	//rebind visual

	private Action closeButtonAction;

	public static GameOptionsUI instance { get; private set; }

	private void Awake() {
		CloseButton.onClick.AddListener(() => {
			Hide();
			closeButtonAction();
			OnButtonPress?.Invoke(this, EventArgs.Empty);
		});
		instance = this;
	}

	private void Start() {
		Hide();
		
	}

	public void GameManger_UnpauseGame(object sender, EventArgs e) {
		Hide();
	}

	private void updateVisual() {
		//musicText.text = "Music: " + Mathf.Round(MusicManager.instance.GetVolume() * 10f).ToString();
	}

	public void Show(Action onCloseButtonAction) {
		closeButtonAction = onCloseButtonAction;
		Show();
	}

	public void Show() {
		gameObject.SetActive(true);
		CloseButton.Select(); //update to be the first option selected for controller
	}

	public void Hide() {
		gameObject.SetActive(false);
	}

}
