using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RaceUI : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    [SerializeField] private float finish;
    [SerializeField] private float start;
    [SerializeField] private Image playercolor;
    private ulong clientID;

    // Start is called before the first frame update
    void Start()
    {
        if (MultiplayerManager.instance.IsPlayerIndexConnected(playerIndex)) {
            show();
			PlayerData playerData = MultiplayerManager.instance.GetPlayerDatafromPlayerIndex(playerIndex);
			playercolor.color = MultiplayerManager.instance.getPlayerColor(playerData.ColorId);
            clientID = playerData.clientId;
		} else {
            hide();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.LocalInstance.OwnerClientId == clientID) {
            float position = Player.LocalInstance.gameObject.transform.position.x;
            float total = finish - start;
            if (total < 0)
            {
                total = total * -1;
            }
            position = position - start;

            position = position / total;
            position = position * 600;

            updateUIServerRpc(position);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void updateUIServerRpc(float position) {
        updateUIClientRpc(position);
    }

    [ClientRpc]
    public void updateUIClientRpc(float position) {
        gameObject.transform.position = new Vector3(position, 0, 0);
    }


    private void show() {
        gameObject.SetActive(true);
    }

    private void hide() {
        gameObject.SetActive(false);
    }
}
