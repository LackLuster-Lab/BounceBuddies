using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class GameInput : MonoBehaviour
{
	private const string PLAYER_PREF_BINDING = "InputBindings";

    public static GameInput instance { get; private set; }

    public PlayerInputActions playerInputActions;

    public enum Binding {
        Move_Up,
        Move_Down, 
        Move_Left, 
        Move_Right,
        Power_Up,
        Pause,
		GamePad_PowerUp,
		GamePad_MoveUp,
		GamePad_MoveDown,
		GamePad_MoveLeft,
		GamePad_MoveRight,
		GamePad_Pause
    }


    public event EventHandler onPowerUpPerformed;
    public event EventHandler pausePerformed;

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

	//Binding
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
			case Binding.GamePad_MoveUp:
				return playerInputActions.Player.Move.bindings[6].ToDisplayString();
			case Binding.GamePad_MoveDown:
				return playerInputActions.Player.Move.bindings[7].ToDisplayString();
			case Binding.GamePad_MoveLeft:
				return playerInputActions.Player.Move.bindings[8].ToDisplayString();
			case Binding.GamePad_MoveRight:
				return playerInputActions.Player.Move.bindings[9].ToDisplayString();


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
			case Binding.GamePad_MoveUp:
				reboundAction = playerInputActions.Player.Move;
				actionIndex = 6;
				break;
			case Binding.GamePad_MoveDown:
				reboundAction = playerInputActions.Player.Move;
				actionIndex = 7;
				break;
			case Binding.GamePad_MoveLeft:
				reboundAction = playerInputActions.Player.Move;
				actionIndex = 8;
				break;
			case Binding.GamePad_MoveRight:
				reboundAction = playerInputActions.Player.Move;
				actionIndex = 9;
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
