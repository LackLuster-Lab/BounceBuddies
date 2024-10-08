using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ResetStaticdata : MonoBehaviour
{//This is for reseting static data between scene loads as to not create multiple listeners on the same static event from different scenes

	public void Awake() {
		//call function to set static event = to null
	}
}
