using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameInput : MonoBehaviour
{
    public event EventHandler onPowerUpPerformed;
    public event EventHandler pausePerformed;
    public PlayerInputActions playerInputActions;

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


}
