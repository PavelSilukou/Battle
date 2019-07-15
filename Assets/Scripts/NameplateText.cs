using UnityEngine;
using System.Collections;

public class NameplateText : MonoBehaviour {
	
	public GameObject target;

	// Update is called once per frame
	private void LateUpdate () {
		if (target == null) {
			Destroy(gameObject); 
			return; 
		}
		GetComponent<GUIText>().text = "" + target.GetComponent<UnitController>().unit.count;
		Vector3 position = Camera.main.WorldToScreenPoint(target.transform.position);
		
		GetComponent<GUIText>().pixelOffset = new Vector2(position.x - Screen.width / 2, position.y - Screen.height / 2);
	}
}
