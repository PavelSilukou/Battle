using UnityEngine;
using System.Collections;

public class NameplateTexture : MonoBehaviour {

	public GameObject target;
	
	private void LateUpdate () {
		if (target == null) { Destroy(gameObject); return;}
		Vector3 position = Camera.main.WorldToScreenPoint(target.transform.position);
		
		GetComponent<GUITexture>().pixelInset = new Rect(-16 + position.x - Screen.width / 2, -16 + position.y - Screen.height / 2, GetComponent<GUITexture>().pixelInset.width, GetComponent<GUITexture>().pixelInset.height);
	}
}
