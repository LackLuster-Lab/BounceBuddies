using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour {

	//buttons
	[SerializeField] private Button sFXButton;
	[SerializeField] private Button musicButton;
	[SerializeField] private Button CloseButton;
	[SerializeField] private Button MoveUpButton;
	[SerializeField] private Button MoveDownButton;
	[SerializeField] private Button MoveLeftButton;
	[SerializeField] private Button MoveRightButton;
	[SerializeField] private Button PowerUpButton;
	[SerializeField] private Button GamePadPowerUpButton;
	[SerializeField] private Button PauseButton;
	[SerializeField] private Button GamePadPauseButton;
	[SerializeField] private Button GamepadMoveUpButton;
	[SerializeField] private Button GamepadMoveDownButton;
	[SerializeField] private Button GamepadMoveLeftButton;
	[SerializeField] private Button GamepadMoveRightButton;

	//text Values
	[SerializeField] private TextMeshProUGUI sFXText;
	[SerializeField] private TextMeshProUGUI musicText;
	[SerializeField] private TextMeshProUGUI MoveUpText;
	[SerializeField] private TextMeshProUGUI MoveDownText;
	[SerializeField] private TextMeshProUGUI MoveLeftText;
	[SerializeField] private TextMeshProUGUI MoveRightText;
	[SerializeField] private TextMeshProUGUI PowerUpText;
	[SerializeField] private TextMeshProUGUI GamePadPowerUpText;
	[SerializeField] private TextMeshProUGUI PauseText;
	[SerializeField] private TextMeshProUGUI GamePadPauseText;
	[SerializeField] private TextMeshProUGUI GamepadMoveUpText;
	[SerializeField] private TextMeshProUGUI GamepadMoveDownText;
	[SerializeField] private TextMeshProUGUI GamepadMoveLeftText;
	[SerializeField] private TextMeshProUGUI GamepadMoveRightText;
	public static event EventHandler OnButtonPress;

	//rebind visual
	[SerializeField] private Transform PressToRebindVisualTransform;

	private Action closeButtonAction;

	public static OptionsUI instance { get; private set; }

	private void Awake() {
		sFXButton.onClick.AddListener(() => {
			SoundManager.instance.ChangeVolume();
			updateVisual();
			OnButtonPress?.Invoke(this, EventArgs.Empty);
		});
		musicButton.onClick.AddListener(() => {
			MusicManager.instance.ChangeVolume();
			updateVisual();
			OnButtonPress?.Invoke(this, EventArgs.Empty);
		});
		CloseButton.onClick.AddListener(() => {
			Hide();
			closeButtonAction();
			OnButtonPress?.Invoke(this, EventArgs.Empty);
		});
		MoveUpButton.onClick.AddListener(() => {
			rebindKey(GameInput.Binding.Move_Up);
			OnButtonPress?.Invoke(this, EventArgs.Empty);
		}); 
		MoveDownButton.onClick.AddListener(() => {
			rebindKey(GameInput.Binding.Move_Down);
			OnButtonPress?.Invoke(this, EventArgs.Empty);
		});
		MoveLeftButton.onClick.AddListener(() => {
			rebindKey(GameInput.Binding.Move_Left);
			OnButtonPress?.Invoke(this, EventArgs.Empty);
		});
		MoveRightButton.onClick.AddListener(() => {
			rebindKey(GameInput.Binding.Move_Right);
			OnButtonPress?.Invoke(this, EventArgs.Empty);
		});
		PowerUpButton.onClick.AddListener(() => {
			rebindKey(GameInput.Binding.Power_Up);
			OnButtonPress?.Invoke(this, EventArgs.Empty);
		});
		PauseButton.onClick.AddListener(() => {
			rebindKey(GameInput.Binding.Pause);
			OnButtonPress?.Invoke(this, EventArgs.Empty);
		});
		GamePadPauseButton.onClick.AddListener(() => {
			rebindKey(GameInput.Binding.GamePad_Pause);
			OnButtonPress?.Invoke(this, EventArgs.Empty);
		});
		GamePadPowerUpButton.onClick.AddListener(() => {
			rebindKey(GameInput.Binding.GamePad_PowerUp);
			OnButtonPress?.Invoke(this, EventArgs.Empty);
		});//change binding
		GamepadMoveUpButton.onClick.AddListener(() => {
			rebindKey(GameInput.Binding.GamePad_MoveUp);
			OnButtonPress?.Invoke(this, EventArgs.Empty);
		});
		GamepadMoveDownButton.onClick.AddListener(() => {
			rebindKey(GameInput.Binding.GamePad_MoveDown);
			OnButtonPress?.Invoke(this, EventArgs.Empty);
		});
		GamepadMoveLeftButton.onClick.AddListener(() => {
			rebindKey(GameInput.Binding.GamePad_MoveLeft);
			OnButtonPress?.Invoke(this, EventArgs.Empty);
		});
		GamepadMoveRightButton.onClick.AddListener(() => {
			rebindKey(GameInput.Binding.GamePad_MoveRight);
			OnButtonPress?.Invoke(this, EventArgs.Empty);
		});
		instance = this;
	}

	private void Start() {
		Hide();
		HideRebind();
		
	}

	public void GameManger_UnpauseGame(object sender, EventArgs e) {
		Hide();
	}

	private void updateVisual() {
		musicText.text = "Music: " + Mathf.Round(MusicManager.instance.GetVolume() * 10f).ToString();
		sFXText.text = "Sound Effects: " + Mathf.Round(SoundManager.instance.GetVolume() * 10f).ToString();
		PowerUpText.text = GameInput.instance.GetBindingText(GameInput.Binding.Power_Up);
		GamePadPowerUpText.text = GameInput.instance.GetBindingText(GameInput.Binding.GamePad_PowerUp);
		GamePadPauseText.text = GameInput.instance.GetBindingText(GameInput.Binding.GamePad_Pause);
		PauseText.text = GameInput.instance.GetBindingText(GameInput.Binding.Pause);
		MoveUpText.text = GameInput.instance.GetBindingText(GameInput.Binding.Move_Up);
		MoveDownText.text = GameInput.instance.GetBindingText(GameInput.Binding.Move_Down);
		MoveLeftText.text = GameInput.instance.GetBindingText(GameInput.Binding.Move_Left);
		MoveRightText.text = GameInput.instance.GetBindingText(GameInput.Binding.Move_Right);
		GamepadMoveUpText.text = GameInput.instance.GetBindingText(GameInput.Binding.GamePad_MoveUp);
		GamepadMoveDownText.text = GameInput.instance.GetBindingText(GameInput.Binding.GamePad_MoveDown);
		GamepadMoveLeftText.text = GameInput.instance.GetBindingText(GameInput.Binding.GamePad_MoveLeft);
		GamepadMoveRightText.text = GameInput.instance.GetBindingText(GameInput.Binding.GamePad_MoveRight);
	}

	public void Show(Action onCloseButtonAction) {
		closeButtonAction = onCloseButtonAction;
		gameObject.SetActive(true);
		sFXButton.Select();
	}

	public void Hide() {
		gameObject.SetActive(false);
	}

	public void ShowRebind() {
		PressToRebindVisualTransform.gameObject.SetActive(true);
	}

	public void HideRebind() {
		PressToRebindVisualTransform.gameObject.SetActive(false);
		updateVisual();
	}

	private void rebindKey(GameInput.Binding binding) {
		ShowRebind();
		GameInput.instance.RebindBinding(binding, HideRebind);
	}
}
