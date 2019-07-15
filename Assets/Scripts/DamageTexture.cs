using UnityEngine;
using System.Collections;

public class DamageTexture : MonoBehaviour {

	public Vector3 target;
	public float height;
	private float targetHeight;
	
	private void Awake () {
		targetHeight = height + 48;
	}
	
	private void LateUpdate () {
		if (height == targetHeight) {Destroy(gameObject); return;}
		Vector3 position = Camera.main.WorldToScreenPoint(target);
		GetComponent<GUITexture>().pixelInset = new Rect(position.x - 60 - Screen.width / 2, position.y + height - Screen.height / 2 - 15, 32.0F, 32.0F);
		height += 0.5F;
	}
}
