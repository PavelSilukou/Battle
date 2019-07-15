using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour {
	
	private GameObject target;
	private int damage;
	
	public bool splash = false;
	
	public void StartFly (GameObject t, int dmg) {
		target = t;
		damage = dmg;
	}
	
	private void MoveArrow () {
		transform.position = Vector3.MoveTowards(transform.position, target.transform.position, 0.5F);
		if (transform.position == target.transform.position) {
			if (splash) {
				GameObject hex = target.GetComponent<UnitController>().unit.Hex;
				//SendMessageContext context = new SendMessageContext();
				//target.SendMessage("returnHex", context);
				//GameObject hex = context.returnGameObject;
				//GameObject hex = target.GetComponent<UnitData>().Hex;
				//GameObject hex = target.SendMessage("returnHex");
				for (int i = 0; i < 6; i++) {
					if (hex.GetComponent<Hex>().neighbor[i].GetComponent<Hex>().Unit != null) {
						//hex.GetComponent<Hex>().neighbor[i].GetComponent<Hex>().Unit.SendMessage("InDamage", damage);
						hex.GetComponent<Hex>().neighbor[i].GetComponent<Hex>().Unit.GetComponent<UnitController>().unit.InDamage(damage);
					}
				}
			}
			//target.SendMessage("InDamage", damage);
			target.GetComponent<UnitController>().unit.InDamage(damage);
			Destroy(gameObject);
		}
	}

	private void Update () {
		MoveArrow();
	}
}
