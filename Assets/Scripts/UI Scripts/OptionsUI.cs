using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour {
	[SerializeField] private Button sFXButton;
	[SerializeField] private Button musicButton;
	[SerializeField] private Button CloseButton;
	[SerializeField] private Button MoveUpButton;
	[SerializeField] private Button MoveDownButton;
	[SerializeField] private Button MoveLeftButton;
	[SerializeField] private Button MoveRightButton;
	[SerializeField] private Button PowerUpButton;
	[SerializeField] private Button PauseButton;
	[SerializeField] private TextMeshProUGUI sFXText;
	[SerializeField] private TextMeshProUGUI musicText;
	[SerializeField] private TextMeshProUGUI MoveUpText;
	[SerializeField] private TextMeshProUGUI MoveDownText;
	[SerializeField] private TextMeshProUGUI MoveLeftText;
	[SerializeField] private TextMeshProUGUI MoveRightText;
	[SerializeField] private TextMeshProUGUI PowerUpText;
	[SerializeField] private TextMeshProUGUI PauseText;
	[SerializeField] private Transform PressToRebindVisualTransform;

	public static OptionsUI instance { get; private set; }

	private void Awake() {
		sFXButton.onClick.AddListener(() => {
			SoundManager.instance.ChangeVolume();
			updateVisual();
		});
		musicButton.onClick.AddListener(() => {
			MusicManager.instance.ChangeVolume();
			updateVisual();
		});
		CloseButton.onClick.AddListener(() => {
			Hide();
		});
		MoveUpButton.onClick.AddListener(() => {
			GameInput.instance.RebindBinding(GameInput.Binding.Move_Up);
			updateVisual();
		}); 
		MoveDownButton.onClick.AddListener(() => {
			GameInput.instance.RebindBinding(GameInput.Binding.Move_Down);
			updateVisual();
		});
		MoveLeftButton.onClick.AddListener(() => {
			GameInput.instance.RebindBinding(GameInput.Binding.Move_Left);
			updateVisual();
		});
		MoveRightButton.onClick.AddListener(() => {
			GameInput.instance.RebindBinding(GameInput.Binding.Move_Right);
			updateVisual();
		});
		PowerUpButton.onClick.AddListener(() => {
			GameInput.instance.RebindBinding(GameInput.Binding.Power_Up);
			updateVisual();
		});
		PauseButton.onClick.AddListener(() => {
			GameInput.instance.RebindBinding(GameInput.Binding.Pause);
			updateVisual();
		});
		instance = this;
	}

	private void Start() {
		GameManager.instance.OnUnPauseGame += GameManger_UnpauseGame;
		updateVisual();
		Hide();
		
	}

	private void GameManger_UnpauseGame(object sender, EventArgs e) {
		Hide();
	}

	private void updateVisual() {
		musicText.text = "Music: " + Mathf.Round(MusicManager.instance.GetVolume() * 10f).ToString();
		sFXText.text = "Sound Effects: " + Mathf.Round(SoundManager.instance.GetVolume() * 10f).ToString();
		PowerUpText.text = GameInput.instance.GetBindingText(GameInput.Binding.Power_Up);
		PauseText.text = GameInput.instance.GetBindingText(GameInput.Binding.Pause);
		MoveUpText.text = GameInput.instance.GetBindingText(GameInput.Binding.Move_Up);
		MoveDownText.text = GameInput.instance.GetBindingText(GameInput.Binding.Move_Down);
		MoveLeftText.text = GameInput.instance.GetBindingText(GameInput.Binding.Move_Left);
		MoveRightText.text = GameInput.instance.GetBindingText(GameInput.Binding.Move_Right);
	}

	public void Show() {
		gameObject.SetActive(true);
	}

	public void Hide() {
		gameObject.SetActive(false);
	}

	public void ShowRebind() {
		PressToRebindVisualTransform.gameObject.SetActive(true);
	}

	public void HideRebind() {
		PressToRebindVisualTransform.gameObject.SetActive(false);
	}
}
