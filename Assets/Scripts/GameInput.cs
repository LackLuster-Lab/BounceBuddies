using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class GameInput : MonoBehaviour
{
	private const string PLAYER_PREF_BINDING = "InputBindings";

    public event EventHandler onPowerUpPerformed;
    public event EventHandler pausePerformed;
    public PlayerInputActions playerInputActions;

    public enum Binding {
        Move_Up,
        Move_Down, 
        Move_Left, 
        Move_Right,
        Power_Up,
        Pause,
		GamePad_PowerUp,
		GamePad_Pause
    }

    public static GameInput instance { get; private set; }
    private void Awake() {
        playerInputActions = new PlayerInputActions();

		if (PlayerPrefs.HasKey(PLAYER_PREF_BINDING)) {
			playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREF_BINDING));
		}

        playerInputActions.Player.Enable();

        playerInputActions.Player.PowerUp.performed += PowerUp_performed;
        playerInputActions.Player.Pause.performed += Pause_performed;
        instance = this;
    }

	private void OnDestroy() {
		playerInputActions.Player.PowerUp.performed -= PowerUp_performed;
		playerInputActions.Player.Pause.performed -= Pause_performed;

        playerInputActions.Dispose();
	}

	private void PowerUp_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        onPowerUpPerformed?.Invoke(this, EventArgs.Empty);
    }

    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        pausePerformed?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 getMovementVectorNormalized() {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }

    public string GetBindingText(Binding binding) {
        switch (binding) {
            default:
			case Binding.Move_Up:
				return playerInputActions.Player.Move.bindings[1].ToDisplayString();
			case Binding.Move_Down:
				return playerInputActions.Player.Move.bindings[2].ToDisplayString();
			case Binding.Move_Left:
				return playerInputActions.Player.Move.bindings[3].ToDisplayString();
			case Binding.Move_Right:
				return playerInputActions.Player.Move.bindings[4].ToDisplayString();
			case Binding.Power_Up:
                return playerInputActions.Player.PowerUp.bindings[0].ToDisplayString();
			case Binding.GamePad_PowerUp:
				return playerInputActions.Player.PowerUp.bindings[2].ToDisplayString();
			case Binding.Pause:
				return playerInputActions.Player.Pause.bindings[0].ToDisplayString();
			case Binding.GamePad_Pause:
				return playerInputActions.Player.Pause.bindings[1].ToDisplayString();


		}
    }

    public void RebindBinding(Binding binding, Action onActionRebound) {
        playerInputActions.Player.Disable();

		InputAction reboundAction;
		int actionIndex;
        switch (binding) {
			default:
            case Binding.Move_Up:
				reboundAction = playerInputActions.Player.Move;
				actionIndex = 1;
                break;
			case Binding.Move_Down:
				reboundAction = playerInputActions.Player.Move;
				actionIndex = 2;
				break;
			case Binding.Move_Left:
				reboundAction = playerInputActions.Player.Move;
				actionIndex = 3;
				break;
			case Binding.Move_Right:
				reboundAction = playerInputActions.Player.Move;
				actionIndex = 4;
				break;
			case Binding.Power_Up:
				reboundAction = playerInputActions.Player.PowerUp;
				actionIndex = 0;
				break;
			case Binding.Pause:
				reboundAction = playerInputActions.Player.Pause;
				actionIndex = 0;
				break;
			case Binding.GamePad_PowerUp:
				reboundAction = playerInputActions.Player.PowerUp;
				actionIndex = 2;
				break;
			case Binding.GamePad_Pause:
				reboundAction = playerInputActions.Player.Pause;
				actionIndex = 1;
				break;
		}

		reboundAction.PerformInteractiveRebinding(actionIndex).OnComplete(callback => {
			callback.Dispose();
			playerInputActions.Player.Enable();
			onActionRebound();

			PlayerPrefs.SetString(PLAYER_PREF_BINDING, playerInputActions.SaveBindingOverridesAsJson());
			PlayerPrefs.Save();
		}).Start();
	}
}
