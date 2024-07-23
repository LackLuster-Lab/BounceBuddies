using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;

public class Rocket : PowerUpItem {
    public override void Use(object sender, Player.UsePowerUPEventArgs eventArgs) {
        Vector2 currentDir = eventArgs.gameInput.playerInputActions.Player.Move.ReadValue<Vector2>();
        
        Vector3 Rotation = new Vector3(0, 0, (Mathf.Rad2Deg * Mathf.Acos(Vector2.Dot(Vector2.right, currentDir))));
        if (currentDir.y < 0) {
            Rotation *= -1;

        }
        Rotation = Rotation + new Vector3(0, 0, 180);
		eventArgs.gameObject.GetComponent<Rigidbody2D>().velocity = currentDir * 50;
        GameObject particles = Instantiate(vfx, eventArgs.gameObject.transform.position, Quaternion.Euler(Rotation));
        Destroy(particles, 1);
    }
}
