using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : PowerUpItem {
    public override void Use(object sender, Player.UsePowerUPEventArgs eventArgs) {
        eventArgs.gameObject.GetComponent<Rigidbody2D>().velocity = eventArgs.gameInput.playerInputActions.Player.Move.ReadValue<Vector2>() * 50;
    }
}
