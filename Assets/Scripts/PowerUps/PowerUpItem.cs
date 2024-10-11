using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PowerUpItem : MonoBehaviour {
    [SerializeField]public PowerUpSO powerUpSO;
    [SerializeField]public PowerUpFunctions.powerup powerup;

	public void collect() {
        Destroy(gameObject);
    }

 //   [ServerRpc]
 //   private void collectServerRpc() {
 //       CollectClientRpc();
 //   }
 //   [ClientRpc]
 //   private void CollectClientRpc() {
 //       Destroy(this.gameObject, 1);
	//	Debug.Log("powerup");
	//}

}
