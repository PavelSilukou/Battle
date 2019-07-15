using UnityEngine;
using System.Collections;

public class DamageText : MonoBehaviour {

	public Vector3 target;
	public float height;
	private float targetHeight;
	
	private void Awake () {
		targetHeight = height + 48;
	}
	
	private void LateUpdate () {
		if (height == targetHeight) {Destroy(gameObject); return;}
		Vector3 position = Camera.main.WorldToScreenPoint(target);
		GetComponent<GUIText>().pixelOffset = new Vector2(position.x - 6 - Screen.width / 2, position.y + height - Screen.height / 2);
		height += 0.5F;
	}
}
