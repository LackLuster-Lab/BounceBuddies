using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerVisual : MonoBehaviour
{
	[SerializeField] SpriteRenderer Body;
	[SerializeField] SpriteRenderer Mouth;

	private Color color;

	private void Awake() {
		color = new Color(Body.color.r, Body.color.g, Body.color.b);
		Body.color = color;
		Mouth.color = color;
	}



	public void setPlayerColor(Color color) {
		this.color = color;
		Body.color = this.color;
		Mouth.color = this.color;
	}

}
