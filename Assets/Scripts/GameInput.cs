using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class GameInput : MonoBehaviour
{
    public event EventHandler onPowerUpPerformed;
    public event EventHandler pausePerformed;
    public PlayerInputActions playerInputActions;

    public enum Binding {
        Move_Up,
        Move_Down, 
        Move_Left, 
        Move_Right,
        Power_Up,
        Pause
    }

    public static GameInput instance { get; private set; }
    private void Awake() {
        playerInputActions = new PlayerInputActions();
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
			case Binding.Pause:
				return playerInputActions.Player.Pause.bindings[0].ToDisplayString();

		}
    }

    public void RebindBinding(Binding binding) {
        playerInputActions.Player.Disable();

        switch (binding) {
            case Binding.Move_Up:
                playerInputActions.Player.Move.PerformInteractiveRebinding(1).OnComplete(callback => {
                    callback.Dispose();
                    playerInputActions.Player.Enable();
                }).Start(); 
                break;
			case Binding.Move_Down:
				playerInputActions.Player.Move.PerformInteractiveRebinding(2).OnComplete(callback => {
					callback.Dispose();
					playerInputActions.Player.Enable();
				}).Start();
				break;
			case Binding.Move_Left:
				playerInputActions.Player.Move.PerformInteractiveRebinding(3).OnComplete(callback => {
					callback.Dispose();
					playerInputActions.Player.Enable();
				}).Start();
				break;
			case Binding.Move_Right:
				playerInputActions.Player.Move.PerformInteractiveRebinding(4).OnComplete(callback => {
					callback.Dispose();
					playerInputActions.Player.Enable();
				}).Start();
				break;
			case Binding.Power_Up:
				playerInputActions.Player.PowerUp.PerformInteractiveRebinding(0).OnComplete(callback => {
					callback.Dispose();
					playerInputActions.Player.Enable();
				}).Start();
				break;
			case Binding.Pause:
				playerInputActions.Player.Pause.PerformInteractiveRebinding(0).OnComplete(callback => {
					callback.Dispose();
					playerInputActions.Player.Enable();
				}).Start();
				break;
		}
    }
}
