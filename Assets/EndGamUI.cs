using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class EndGamUI : MonoBehaviour
{
	[SerializeField] Button mainMenuButton;

	private void Awake() {
		mainMenuButton.onClick.AddListener(() => {
			GameLobby.Instance.LeaveLobby();
			NetworkManager.Singleton.Shutdown();
			Loader.Load(Loader.scenes.MainMenu);
		});
	}
	// Start is called before the first frame update
	void Start()
    {
        mainMenuButton.Select();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
