using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HostOptions : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(NetworkManager.Singleton.IsServer);
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
