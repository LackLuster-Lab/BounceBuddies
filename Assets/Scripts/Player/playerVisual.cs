using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerVisual : MonoBehaviour
{
	[SerializeField] SpriteRenderer Body;
	[SerializeField] Sprite BodySprite;
	[SerializeField] Sprite HeavySprite;
	[SerializeField] SpriteRenderer Mouth;
	[SerializeField] SpriteRenderer Eyes;

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

	public void Heavy() {
		Body.sprite = HeavySprite;

	}

	public void Light() {
		Body.color = new Color(color.r, color.g, color.b, .3f);
		Mouth.color = new Color(color.r, color.g, color.b, .3f);
		Eyes.color = new Color(1, 1, 1, .3f);
	}
		
	public void Normal() {
		Body.sprite = BodySprite;
		Body.color = new Color(color.r, color.g, color.b, 1);
		Mouth.color = new Color(color.r, color.g, color.b, 1);
		Eyes.color = new Color(1, 1, 1, 1);
	}
}
