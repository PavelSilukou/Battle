using UnityEngine;
using System.Collections;

public class SendMessageContext
{
    public int returnInt;
    public GameObject returnGameObject;
}

public class UnitData {
	
	public int team = 0;
	
	public int count;
	public int MinDamage;
	public int MaxDamage;
	public int defaultHealth;
	public int currentHealth;
	public int Speed;
	public int Initiative;
	public bool AI_Control;
	
	public GameObject Hex;
	public DamageText HealthDecText;
	public DamageText UnitsDecText;
	public DamageTexture HealthDecTexture;
	public DamageTexture UnitsDecTexture;
	
	public GameObject controller;
	public GameObject mouseHex;
	
	public GameObject atackUnit;
	public Arrow arrow;
	
	public System.Collections.Generic.List<GameObject> listHexToMove = new System.Collections.Generic.List<GameObject>();
	public System.Collections.Generic.List<GameObject> listPath = new System.Collections.Generic.List<GameObject>();
	public System.Collections.Generic.List<GameObject> listAtackUnit = new System.Collections.Generic.List<GameObject>();
	
	public bool move = false;
	public bool atack = false;
	public bool arrowFly = false;
	
	public int activity = 0;
	
	public byte wait = 0;
	public bool defence = true;
	
	//
	public Animation animation;
	public GameObject gameObject;
	//
	
	/*public void returnHex(SendMessageContext context)
	{
		context.returnGameObject = Hex;
	}*/
	
	public int MovePath (GameObject obj1, GameObject obj2) {
		
		//listPath.Clear();
		
		if (obj2.GetComponent<Hex>().currentStat != 2) return -1;
		
		System.Collections.Queue queuePath = new System.Collections.Queue();
		
		queuePath.Enqueue(obj1);
		
		bool flag = false;
		
		while (flag != true) {
			int count = queuePath.Count;
			for (int i = 0; i < count; i++) {
				GameObject tempobj = (GameObject)queuePath.Dequeue();
				
				if (tempobj != obj2) {
					for (int j = 0; j < 6; j++) {
						GameObject neighbor = tempobj.GetComponent<Hex>().neighbor[j];
						if ((neighbor != null) && 
							(neighbor.GetComponent<Hex>().LabelObject == null) && 
							(listHexToMove.Contains(neighbor))) {
								neighbor.GetComponent<Hex>().LabelObject = tempobj;
								queuePath.Enqueue(neighbor);
						}
					}
				}
				else {
					flag = true;
					break;
				}
			}
		}
		
		queuePath.Clear();
		
		listPath.Add(obj2);
		obj2.GetComponent<Hex>().currentStat = 3;
		
		while (obj2 != obj1) {
			obj2 = obj2.GetComponent<Hex>().LabelObject;
			obj2.GetComponent<Hex>().currentStat = 3;
			listPath.Add(obj2);
		}
		
		listPath.Reverse();
		
		foreach (GameObject obj in listHexToMove) {
			obj.GetComponent<Hex>().LabelObject = null;
		}
		
		return listPath.Count - 1;
		
	}
	
	/*private void SearchPathToFly (GameObject obj, int count) {

		if (obj == null) return;
		if (!listHexToMove.Contains(obj))
			listHexToMove.Add(obj);
		if ((obj.GetComponent<Hex>().Unit == null) && (obj.GetComponent<Hex>().currentStat != 0))
			obj.GetComponent<Hex>().currentStat = 2;
		if (count == 0) return;
		if ((Speed != count) && (obj.GetComponent<Hex>().Unit != null)) return;
		for (int i = 0; i < 6; i++) {
			SearchPathToFly(obj.GetComponent<Hex>().neighbor[i], count - 1);
		}
	}*/
	
	public GameObject SearchNeighbor (GameObject hex) {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		Physics.Raycast(ray, out hit);
		
		if ((listHexToMove.Contains(hex.GetComponent<Hex>().neighbor[hit.triangleIndex]) || (hex.GetComponent<Hex>().neighbor[hit.triangleIndex].GetComponent<Hex>().Unit != null)))
			return hex.GetComponent<Hex>().neighbor[hit.triangleIndex];
		else return null;
	}
	
	public GameObject SearchExistNeighbor (GameObject hex) {
		foreach (GameObject obj in hex.GetComponent<Hex>().neighbor) {
			if (obj == null) return null;
			if (listHexToMove.Contains(obj)) return obj;
		}
		return null;
	}
	
	public void Move (Transform transform) {
		if (listPath.Count == 1) {
			move = false;
			if (!atackUnit) EndActivity();
			return;
		}
		transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation((listPath[1].transform.position - transform.position).normalized), 30.0F);
		animation.Play("Move");
		transform.position = Vector3.MoveTowards(transform.position, listPath[1].transform.position, 1.732F / (animation["Move"].length * 60));
		if (transform.position == listPath[1].transform.position) {
			listPath.RemoveAt(0);
		}
	}
	
	public void Atack (Transform transform) {
		while (transform.rotation != Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation((atackUnit.transform.position - transform.position).normalized), 30.0F)) {
			transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation((atackUnit.transform.position - transform.position).normalized), 30.0F);
		}
		while (atackUnit.transform.rotation != Quaternion.RotateTowards(atackUnit.transform.rotation, Quaternion.LookRotation((transform.position - atackUnit.transform.position).normalized), 30.0F)) {
			atackUnit.transform.rotation = Quaternion.RotateTowards(atackUnit.transform.rotation, Quaternion.LookRotation((transform.position - atackUnit.transform.position).normalized), 30.0F);
		}
		
		int damage = Random.Range(MinDamage, MaxDamage) * count;
		
		animation.Play("Atack");
		if ((animation["Atack"].time > (animation["Atack"].length / 2 - 0.05F)) && (animation["Atack"].time < (animation["Atack"].length / 2 + 0.05F))) {
			if (arrow != null) {
				Object arrowObject = Object.Instantiate(arrow, transform.position, transform.rotation * arrow.transform.rotation);
				arrowObject.name = "Arrow";
				GameObject.Find("Arrow").GetComponent<Arrow>().StartFly(atackUnit, damage);
				atack = false;
				arrowFly = true;
			}
			else {
				atackUnit.GetComponent<UnitController>().unit.InDamage(damage);
				atack = false;
				EndActivity();
			}
		}
	}

	public void InDamage (int dmg) {
		
		animation.Play("TakeDamage");
		
		int dead = 0;
		
		if (dmg < ((count - 1) * defaultHealth) + currentHealth) {
			dead = dmg / defaultHealth + ((dmg % defaultHealth) / currentHealth);
		}
		else {
			dead = count;
		}
		
		count -= dmg / defaultHealth;
		
		if (dmg % defaultHealth >= currentHealth) {
			count -= 1;
			currentHealth = defaultHealth - (dmg % defaultHealth - currentHealth);
		}
		else {
			currentHealth -= dmg % defaultHealth;
		}
		
		Object tHDT = Object.Instantiate(HealthDecText);
		tHDT.name = "HealthDecText" + Time.time + gameObject.name;
		GameObject.Find(tHDT.name).GetComponent<GUIText>().text = "-" + dmg;
		GameObject.Find(tHDT.name).GetComponent<DamageText>().target = gameObject.transform.position;
		
		Object tUDT = Object.Instantiate(UnitsDecText);
		tUDT.name = "UnitsDecText" + Time.time + gameObject.name;
		GameObject.Find(tUDT.name).GetComponent<GUIText>().text = "-" + dead;
		GameObject.Find(tUDT.name).GetComponent<DamageText>().target = gameObject.transform.position;
		
		Object tHDTr = Object.Instantiate(HealthDecTexture);
		tHDTr.name = "HealthDecTexture" + Time.time + gameObject.name;
		GameObject.Find(tHDTr.name).GetComponent<DamageTexture>().target = gameObject.transform.position;
		
		Object tUDTr = Object.Instantiate(UnitsDecTexture);
		tUDTr.name = "UnitsDecTexture" + Time.time + gameObject.name;
		GameObject.Find(tUDTr.name).GetComponent<DamageTexture>().target = gameObject.transform.position;
		
		if (count <= 0) DestroyUnit();
	}
	
	private void DestroyUnit () {
		animation.Play("Death");
		
		GameObject.Find("Generator").GetComponent<Generator>().listTroops.Remove(gameObject);
		if (team == 1)
			GameObject.Find("Generator").GetComponent<Generator>().listTeam1.Remove(gameObject);
		if (team == 2)
			GameObject.Find("Generator").GetComponent<Generator>().listTeam2.Remove(gameObject);
		controller.GetComponent<Controller>().listInitiative.Remove(gameObject);
		
		if (activity == 2) {
			activity = 0;
			controller.GetComponent<Controller>().act = true;
		}
		
		Hex.GetComponent<Hex>().Unit = null;
		Object.Destroy(GameObject.Find("NPText" + gameObject.name));
		Object.Destroy(GameObject.Find("NPTexture" + gameObject.name));
		//Destroy(gameObject);
	}
	
	public void Wait () {
		activity = 0;
		controller.GetComponent<Controller>().listInitiative.RemoveAt(0);
		controller.GetComponent<Controller>().listInitiative.Add(gameObject);
		controller.GetComponent<Controller>().act = true;
		wait = 1;
	}
	
	public void Defence () {
		activity = 0;
		controller.GetComponent<Controller>().listInitiative.RemoveAt(0);
		controller.GetComponent<Controller>().act = true;
		defence = false;
	}
	
	public void StartActivity () {
		listPath.Clear();
		listHexToMove.Clear();
		listAtackUnit.Clear();
		
		
		// ???????????
		if (wait == 1) wait++;
		else {
			wait = 0;
		}

		activity = 2;
	}
	
	public void StopActivity () {
		controller.GetComponent<Controller>().ZeroStat();
		activity = 1;
	}
	
	public void ResumeActivity () {
		activity = 2;
	}
	
	public void EndActivity () {
		activity = 0;
		if (listPath.Count != 0) {
			Hex.GetComponent<Hex>().Unit = null;
			Hex = listPath[0];
			Hex.GetComponent<Hex>().Unit = gameObject;
		}
		controller.GetComponent<Controller>().listInitiative.RemoveAt(0);
		controller.GetComponent<Controller>().act = true;
	}
	
	public void ActivityAI () {
		listPath.Clear();
		listHexToMove.Clear();
		listAtackUnit.Clear();
		
		activity = 2;
	}
	
	protected virtual void SearchMove (GameObject obj, int count)
	{
	}
	
	protected virtual void SearchAtack (GameObject obj, int count)
	{
	}
	
	public void InitData (int i1, int i2, int i3, int i4, int i5, int i6, 
							int i7, int i8, Arrow ar, GameObject go, Animation anim, 
							GameObject contr, DamageText t1, DamageText t2,
							DamageTexture tx1, DamageTexture tx2)
	{
		team = i1;
		count = i2;
		MinDamage = i3;
		MaxDamage = i4;
		defaultHealth = i5;
		currentHealth = i6;
		Speed = i7;
		Initiative = i8;
		arrow = ar;
		animation = anim;
		gameObject = go;
		controller = contr;
		HealthDecText = t1;
		UnitsDecText = t2;
		HealthDecTexture = tx1;
		UnitsDecTexture = tx2;
	}
	
	protected virtual int MoveAtack ()
	{
		return 0;
	}
	
	protected virtual int MoveAtackAI (int choise)
	{
		return 0;
	}
	
	public void Evaluate (Transform transform) {
		if (AI_Control == false) {
			if (activity == 2) {
				controller.GetComponent<Controller>().ZeroStat();
				atackUnit = null;
				listPath.Clear();
				
				SearchMove(Hex, Speed);
				
				SearchAtack(Hex, Speed);
				mouseHex = controller.GetComponent<Controller>().SetMouseHex();
				
				int lengthPath = MoveAtack();

				if (Input.GetMouseButtonDown(0)) {
					if (atackUnit != null) {
						atack = true;
						activity = 3;
					}
					if (lengthPath > 0) {
						move = true;
						activity = 3;
					}
				}
			}
			else if (activity == 3) {
				controller.GetComponent<Controller>().ZeroStat();
				if (move) {
					Move(transform);
				}
				else if (atack) {
					Atack(transform);
				}
			}
			if (arrowFly == true) {
				if (GameObject.Find("Arrow") == null) {
					arrowFly = false;
					EndActivity();
				}
			}
		}
		///////////////////
		else {
			if (activity == 2) {
				controller.GetComponent<Controller>().ZeroStat();
				atackUnit = null;
				listPath.Clear();
				
				SearchMove(Hex, Speed);

				SearchAtack(Hex, Speed);

				int choise;

				if (listAtackUnit.Count == 0) choise = 1;
				else choise = 0;
				if (choise == 0) {
					mouseHex = listAtackUnit[Random.Range(0, listAtackUnit.Count)];
				}
				else {
					mouseHex = listHexToMove[Random.Range(0, listHexToMove.Count)];
				}
				
				int lengthPath = MoveAtackAI(choise);
	
				if (atackUnit != null) {
					atack = true;
					activity = 3;
				}
				if (lengthPath > 0) {
					move = true;
					activity = 3;
				}
			}
			else if (activity == 3) {
				controller.GetComponent<Controller>().ZeroStat();
				if (move) {
					Move(transform);
				}
				else if (atack) {
					Atack(transform);
				}
			}
			if (arrowFly == true) {
				if (GameObject.Find("Arrow") == null) {
					arrowFly = false;
					EndActivity();
				}
			}
		}
	}
}

public class MeleeWalkUnit : UnitData {
	
	protected override void SearchMove (GameObject obj, int count) {
 	
		if ((obj == null) || (obj.GetComponent<Hex>().currentStat == 0)) return;
		if ((obj.GetComponent<Hex>().Unit == null) && (!listHexToMove.Contains(obj)))
			listHexToMove.Add(obj);
		if (obj.GetComponent<Hex>().Unit == null)
			obj.GetComponent<Hex>().currentStat = 2;
		if (count == 0) return;
		if ((Speed != count) && (obj.GetComponent<Hex>().Unit != null)) return;
		for (int i = 0; i < 6; i++) {
			SearchMove(obj.GetComponent<Hex>().neighbor[i], count - 1);
		}
	}
	
	protected override void SearchAtack (GameObject obj, int count) {
		if ((obj == null) || (obj.GetComponent<Hex>().currentStat == 0)) return;
		if ((obj.GetComponent<Hex>().Unit != null) && (!listAtackUnit.Contains(obj)) && (obj.GetComponent<Hex>().Unit.GetComponent<UnitController>().unit.team != team))
			listAtackUnit.Add(obj);
		if ((obj.GetComponent<Hex>().Unit != null) && (obj.GetComponent<Hex>().Unit.GetComponent<UnitController>().unit.team != team))
			obj.GetComponent<Hex>().currentStat = 4;
		if (count == 0) return;
		if ((Speed != count) && (obj.GetComponent<Hex>().Unit != null)) return;
		for (int i = 0; i < 6; i++) {
			SearchAtack(obj.GetComponent<Hex>().neighbor[i], count - 1);
		}
	}
	
	protected override int MoveAtack ()
	{
		if (listAtackUnit.Contains(mouseHex)) {
			atackUnit = mouseHex.GetComponent<Hex>().Unit;
			GameObject neighbor = SearchNeighbor(mouseHex);
			if (neighbor != null)
				return MovePath(Hex, neighbor);
			else
				atackUnit = null;
		}
		else if (listHexToMove.Contains(mouseHex)) {
			return MovePath(Hex, mouseHex);
		}
		return 0;
	}
	
	protected override int MoveAtackAI (int choise)
	{
		if (choise == 0) {
			atackUnit = mouseHex.GetComponent<Hex>().Unit;
			
			GameObject neighbor = SearchExistNeighbor(mouseHex);
			return MovePath(Hex, neighbor);
		}
		else if (choise == 1) {
			return MovePath(Hex, mouseHex);
		}
		return 0;
	}
}

public class RangeWalkUnit : UnitData {
	
	protected override void SearchMove (GameObject obj, int count) {
 	
		if ((obj == null) || (obj.GetComponent<Hex>().currentStat == 0)) return;
		if ((obj.GetComponent<Hex>().Unit == null) && (!listHexToMove.Contains(obj)))
			listHexToMove.Add(obj);
		if (obj.GetComponent<Hex>().Unit == null)
			obj.GetComponent<Hex>().currentStat = 2;
		if (count == 0) return;
		if ((Speed != count) && (obj.GetComponent<Hex>().Unit != null)) return;
		for (int i = 0; i < 6; i++) {
			SearchMove(obj.GetComponent<Hex>().neighbor[i], count - 1);
		}
	}
	
	protected override void SearchAtack (GameObject obj, int count) {
		foreach (GameObject unit in GameObject.Find("Generator").GetComponent<Generator>().listTroops) {
			if ((unit.GetComponent<UnitController>().unit.team != team) && (!listAtackUnit.Contains(unit.GetComponent<UnitController>().unit.Hex))) {
				listAtackUnit.Add(unit.GetComponent<UnitController>().unit.Hex);
				unit.GetComponent<UnitController>().unit.Hex.GetComponent<Hex>().currentStat = 4;
			}
			if (unit.GetComponent<UnitController>().unit.team != team)
				unit.GetComponent<UnitController>().unit.Hex.GetComponent<Hex>().currentStat = 4;
		}
	}
	
	protected override int MoveAtack ()
	{
		if (listAtackUnit.Contains(mouseHex)) {
			atackUnit = mouseHex.GetComponent<Hex>().Unit;
		}
		else if (listHexToMove.Contains(mouseHex)) {
			return MovePath(Hex, mouseHex);
		}
		return 0;
	}
	
	protected override int MoveAtackAI (int choise)
	{
		if (choise == 0) {
			atackUnit = mouseHex.GetComponent<Hex>().Unit;
		}
		else if (choise == 1) {
			return MovePath(Hex, mouseHex);
		}
		return 0;
	}
	
}