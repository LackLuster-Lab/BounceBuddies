using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public abstract class PowerUpItem : NetworkBehaviour {
    [SerializeField]public PowerUpSO powerUpSO;

	public void collect() {
        collectServerRpc();
    }

    [ServerRpc]
    private void collectServerRpc() {
		Destroy(gameObject);
    }

    public abstract void Use(object sender, Player.UsePowerUPEventArgs eventArgs);
}
