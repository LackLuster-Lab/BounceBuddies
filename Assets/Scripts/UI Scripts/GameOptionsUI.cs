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
	[SerializeField] private TMP_Dropdown MapDropdown;
	[SerializeField] private TMP_Dropdown ModeDropdown;
	[SerializeField] private Button RoundsButtonUp;
	[SerializeField] private Button RoundsButtonDown;
	[SerializeField] private Button TimeButtonUp;
	[SerializeField] private Button TimeButtonDown;
	[SerializeField] private Button PowerUpButton;

	[SerializeField] private TextMeshProUGUI RoundsText;
	[SerializeField] private TextMeshProUGUI TimeText;
	[SerializeField] private GameObject powerupCheck;

	[SerializeField] private int RoundsMax = 100;
	[SerializeField] private float timerInc = 30;
	[SerializeField] private float timerMax = 180;
	//text Values
	public static event EventHandler OnButtonPress;

	//rebind visual

	private Action closeButtonAction;

	public static GameOptionsUI instance { get; private set; }

	private void Awake() {
		CloseButton.onClick.AddListener(() => {
			Hide();
			//closeButtonAction();
			OnButtonPress?.Invoke(this, EventArgs.Empty);
		});
		MapDropdown.onValueChanged.AddListener((int value) => {
			//update map value
			updateMap(value);
			OnButtonPress?.Invoke(this, EventArgs.Empty);
		});
		ModeDropdown.onValueChanged.AddListener((int value) => {
			updateMode(value);
			OnButtonPress?.Invoke(this, EventArgs.Empty);
		});
		RoundsButtonUp.onClick.AddListener(() => {
			UpdateRounds(true);
			RoundsText.text = "Rounds: " + GameManager.instance.getRounds();
			OnButtonPress?.Invoke(this, EventArgs.Empty);
		});
		RoundsButtonDown.onClick.AddListener(() => {
			UpdateRounds(false);
			RoundsText.text = "Rounds: " + GameManager.instance.getRounds();
			OnButtonPress?.Invoke(this, EventArgs.Empty);
		});
		TimeButtonUp.onClick.AddListener(() => {
			UpdateTimer(true);
			TimeText.text = "Time: " + GameManager.instance.getRoundTimer() + " Secs";
			OnButtonPress?.Invoke(this, EventArgs.Empty);
		});
		TimeButtonDown.onClick.AddListener(() => {
			UpdateTimer(false);
			TimeText.text = "Time: " + GameManager.instance.getRoundTimer() + " Secs";
			OnButtonPress?.Invoke(this, EventArgs.Empty);
		});
		PowerUpButton.onClick.AddListener(() => {
			updatePower();
			powerupCheck.SetActive(GameManager.instance.getPower());
			OnButtonPress?.Invoke(this, EventArgs.Empty);
		});
		instance = this;
	}

	public void updateMap(int value) {
		GameManager.instance.updateMap(value);
	}

	public void updateMode(int value) {
		GameManager.instance.updateMode(value);
	}

	public void UpdateRounds(bool Up) {
		int curRounds = GameManager.instance.getRounds();
		if (Up) {
			if (curRounds == RoundsMax) {
				GameManager.instance.updateRounds(1);
			} else {
				GameManager.instance.updateRounds(curRounds + 1);
			}
		} else {
			if (curRounds == 1) {
				GameManager.instance.updateRounds(RoundsMax);
			} else {
				GameManager.instance.updateRounds(curRounds - 1);
			}
		}
	}

	public void UpdateTimer(bool Up) {
		float curRounds = GameManager.instance.getRoundTimer();
		if (Up) {
			if (curRounds == timerMax) {
				GameManager.instance.updateTimer(timerInc);
			} else {
				GameManager.instance.updateTimer(curRounds + timerInc);
			}
		} else {
			if (curRounds == timerInc) {
				GameManager.instance.updateTimer(timerMax);
			} else {
				GameManager.instance.updateTimer(curRounds - timerInc);
			}
		}
	}

	public void updatePower() {
		GameManager.instance.updatePowerUps(!GameManager.instance.getPower());
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
