using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {
	
	public Generator generator;
	public BattleGUI battleGUI;
	public Texture[] textures;
	private int typeOfGame;
	
	public short roundCount = 0;
	
	public System.Collections.Generic.List<GameObject> listInitiative = new System.Collections.Generic.List<GameObject>();
	
	public bool act = true;
	public GameObject activeTestUnit;
	
	static int CompareInitiative (GameObject x, GameObject y) {
		
		int x1 = 0, y1 = 0;
		
		if (x != null)
			x1 = x.GetComponent<UnitController>().unit.Initiative;
		
		if (y != null)
			y1 = y.GetComponent<UnitController>().unit.Initiative;
		
		if (x1 == 0) {
			if (y1 == 0) {
				return 0;
			}
			else {
				return 1;
			}
		}
		else {
			if (y1 == 0) {
				return -1;
			}
			else {
				if (x1 > y1) return -1;
				else return 1;
			}
		}
	}	
	
	public GameObject SetActiveTestUnit () {
		
		if (listInitiative.Count == 0) {
			listInitiative.AddRange(generator.listTroops);
			listInitiative.Sort(CompareInitiative);
			roundCount++;
			battleGUI.GetComponent<BattleGUI>().ShowRoundCount("Round " + roundCount);
		}
		
		return listInitiative[0];
	}
	
	public GameObject SetMouseHex () {
		
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit)) {
			return hit.collider.gameObject;
		}
		else return null;
	}
	
	public void ZeroStat () {
		foreach (GameObject obj in generator.listHexes) {
			obj.GetComponent<Hex>().currentStat = obj.GetComponent<Hex>().defaultStat;
		}
	}
	
	private void ChangeHexTexture () {

		foreach (GameObject obj in generator.listHexes) {
			switch (obj.GetComponent<Hex>().currentStat) {
				case 0 : obj.GetComponent<Renderer>().material.mainTexture = textures[0]; break;
				case 1 : obj.GetComponent<Renderer>().material.mainTexture = textures[1]; break;
				case 2 : obj.GetComponent<Renderer>().material.mainTexture = textures[2]; break;
				case 3 : obj.GetComponent<Renderer>().material.mainTexture = textures[3]; break;
				case 4 : obj.GetComponent<Renderer>().material.mainTexture = textures[4]; break;
			}
		}
	}
	
	private void Awake () {
		typeOfGame = GameObject.Find("GlobalData").GetComponent<GlobalData>().typeOfGame;
	}
	
	private void Update () {
		
		if (act) {
			
			if (generator.listTeam1.Count == 0) {
				if (typeOfGame == 1)
					battleGUI.GetComponent<BattleGUI>().ShowGameOver("Computer");
				else
					battleGUI.GetComponent<BattleGUI>().ShowGameOver("2");
				//return null;
				act = false;
				return;
			}
		
			if (generator.listTeam2.Count == 0) {
				battleGUI.GetComponent<BattleGUI>().ShowGameOver("1");
				//return null;
				act = false;
				return;
			}
			
			activeTestUnit = SetActiveTestUnit();
			battleGUI.GetComponent<BattleGUI>().activeUnit = activeTestUnit;
			if (activeTestUnit.GetComponent<UnitController>().unit.AI_Control == false) {
				activeTestUnit.GetComponent<UnitController>().unit.StartActivity();
			}
			else {
				activeTestUnit.GetComponent<UnitController>().unit.ActivityAI();
			}
			act = false;
		}
		else {
			ChangeHexTexture();
		}		
	}
}
