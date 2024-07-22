using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class PowerUpItem : MonoBehaviour
{

    public void collect() {
        Destroy(gameObject);
    }
    public abstract void Use(object sender, Player.UsePowerUPEventArgs eventArgs);
}
