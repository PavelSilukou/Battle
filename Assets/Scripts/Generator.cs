using UnityEngine;
using System.Collections;

public class Generator : MonoBehaviour {
	
	public Hex hex;
	public NameplateText nameplateText;
	public NameplateTexture nameplateTexture;
	public Texture[] nameplateTextures = new Texture[2];
	private int typeOfGame;
	
	public System.Collections.Generic.List<UnitController> listUnits = new System.Collections.Generic.List<UnitController>();
	public System.Collections.Generic.List<GameObject> listHexes = new System.Collections.Generic.List<GameObject>();
	public System.Collections.Generic.List<GameObject> listTroops = new System.Collections.Generic.List<GameObject>();
	public System.Collections.Generic.List<GameObject> listTeam1 = new System.Collections.Generic.List<GameObject>();
	public System.Collections.Generic.List<GameObject> listTeam2 = new System.Collections.Generic.List<GameObject>();
	
	private int[][] GenerateHexesMatrix () {
		
		//Generate matrix of hexes; 0 - close move; 1 - possible move
		
		int[][] hexes = new int[7][];
		
		for (int i = 0; i < 7; i += 2)
			hexes[i] = new int[9];
		
		for (int i = 1; i < 7; i += 2)
			hexes[i] = new int[8];
		
		for (int i = 0; i < hexes.Length; i++) {
			for (int j = 0; j < hexes[i].Length; j++) {
				hexes[i][j] = 1;
			}
		}
		
		hexes[4][4] = 0;
		hexes[2][4] = 0;
		hexes[3][3] = 0;
		hexes[3][4] = 0;
		hexes[0][4] = 0;
		hexes[6][4] = 0;
		
		return hexes;
		
	}

	private void GenerateHexesObjects () {
		
		int[][] hexes = GenerateHexesMatrix();
		Object newHex = new Object();
		Vector3 sizeHex = hex.GetComponent<Renderer>().bounds.size;
		GameObject[][] objectHexes = new GameObject[7][];
		
		for (int i = 0; i < 7; i += 2)
			objectHexes[i] = new GameObject[9];
		
		for (int i = 1; i < 7; i += 2)
			objectHexes[i] = new GameObject[8];
		
		for (int i = 0; i < hexes.Length; i++) {
			for (int j = 0; j < hexes[i].Length; j++) {
				
				if (i % 2 != 0)
					newHex = Instantiate(hex, new Vector3((j + 0.5F) * sizeHex.x, 0.01F, i * sizeHex.z * 0.75F), hex.transform.rotation);
				else
					newHex = Instantiate(hex, new Vector3(j * sizeHex.x, 0.01F, i * sizeHex.z * 0.75F), hex.transform.rotation);
				newHex.name = "Hex" + i + j;
				
				objectHexes[i][j] = GameObject.Find(newHex.name);
				
				objectHexes[i][j].GetComponent<Hex>().defaultStat = hexes[i][j];
				objectHexes[i][j].GetComponent<Hex>().currentStat = hexes[i][j];
			}
		}
				
		for (int i = 0; i < objectHexes.Length; i++) {
			for (int j = 0; j < objectHexes[i].Length; j++) {
				
				Hex componentHex = objectHexes[i][j].GetComponent<Hex>();
				
				if (((i % 2 != 0) && (j < 7)) || ((i % 2 == 0) && (j < 8)))
					componentHex.neighbor[2] = objectHexes[i][j + 1];
				if (j != 0)
					componentHex.neighbor[5] = objectHexes[i][j - 1];
				
				if (i % 2 != 0) {
					if ((i < 7) && (j < 8))
						componentHex.neighbor[1] = objectHexes[i + 1][j + 1];
					if ((i > 0) && (j < 8))
						componentHex.neighbor[3] = objectHexes[i - 1][j + 1];
					if (i < 7)
						componentHex.neighbor[0] = objectHexes[i + 1][j];
					if (i > 0)
						componentHex.neighbor[4] = objectHexes[i - 1][j];
				}
				else {
					if ((i < 6) && (j < 8))
						componentHex.neighbor[1] = objectHexes[i + 1][j];
					if ((i > 0) && (j < 8))
						componentHex.neighbor[3] = objectHexes[i - 1][j];
					if ((i < 6) && (j > 0))
						componentHex.neighbor[0] = objectHexes[i + 1][j - 1];
					if ((i > 0) && (j > 0))
						componentHex.neighbor[4] = objectHexes[i - 1][j - 1];
				}
			}
		}
		
		for (int i = 0; i < objectHexes.Length; i++)
			for (int j = 0; j < objectHexes[i].Length; j++)
				listHexes.Add(objectHexes[i][j]);
	}
	
	private void GenerateTroops () {
		
		//Type PositionX PositionY
		string t1 = "None|RangeUnit 4 1|MeleeUnit 3 1|RangeUnit 2 1|None";
		string t2 = "None|RangeUnit 4 7|MeleeUnit 3 6|RangeUnit 2 7|None";
		
		//string t1 = "None|RangeUnit 4 1|None|None|None";
		//string t2 = "None|RangeUnit 4 7|None|None|None";
		
		string[] dataTeam1 = t1.Split('|');
		string[] dataTeam2 = t2.Split('|');
		
		foreach (string str in dataTeam1) {
			if (str == "None") continue;
			
			string[] measures = str.Split(' ');
			
			Object obj = new Object();
			
			UnitController prefabTestUnit = listUnits.Find(delegate (UnitController un) {
				return un.name == measures[0];});
			
			obj = Instantiate(prefabTestUnit, GameObject.Find("Hex" + measures[1] + measures[2]).transform.position, new Quaternion(0.0F, 0.7F, 0.0F, 0.7F));
			obj.name = measures[0] + measures[1] + measures[2];
			
			UnitController unit = GameObject.Find(obj.name).GetComponent<UnitController>();
			
			unit.unit.team = 1;
			
			GameObject.Find("Hex" + measures[1] + measures[2]).GetComponent<Hex>().Unit = GameObject.Find(obj.name);
			unit.unit.Hex = GameObject.Find("Hex" + measures[1] + measures[2]);
			
			listTeam1.Add(GameObject.Find(obj.name));
			listTroops.Add(GameObject.Find(obj.name));
		}
		
		foreach (string str in dataTeam2) {
			if (str == "None") continue;
			
			string[] measures = str.Split(' ');
			
			Object obj = new Object();
			
			UnitController prefabTestUnit = listUnits.Find(delegate (UnitController un) {
				return un.name == measures[0];});
			
			obj = Instantiate(prefabTestUnit, GameObject.Find("Hex" + measures[1] + measures[2]).transform.position, new Quaternion(0.0F, -0.7F, 0.0F, 0.7F));
			obj.name = measures[0] + measures[1] + measures[2];
			
			UnitController unit = GameObject.Find(obj.name).GetComponent<UnitController>();
			
			unit.unit.team = 2;
			
			if (typeOfGame == 1) unit.unit.AI_Control = true;
			
			GameObject.Find("Hex" + measures[1] + measures[2]).GetComponent<Hex>().Unit = GameObject.Find(obj.name);
			unit.unit.Hex = GameObject.Find("Hex" + measures[1] + measures[2]);
			
			listTeam2.Add(GameObject.Find(obj.name));
			listTroops.Add(GameObject.Find(obj.name));
		}
	}
	
	private void GenerateNameplates () {

		foreach (GameObject obj in listTroops) {
			Object text = Instantiate(nameplateText);
			text.name = "NPText" + obj.name;
			GameObject.Find(text.name).GetComponent<NameplateText>().target = obj;
			Object texture = Instantiate(nameplateTexture);
			texture.name = "NPTexture" + obj.name;
			
			GameObject.Find(texture.name).GetComponent<NameplateTexture>().target = obj;
			if (obj.GetComponent<UnitController>().unit.team == 1) {
				GameObject.Find(texture.name).GetComponent<GUITexture>().texture = nameplateTextures[0];
			}
			else {
				GameObject.Find(texture.name).GetComponent<GUITexture>().texture = nameplateTextures[1];
			}
		}
	}
	
	private void Awake () {
		typeOfGame = GameObject.Find("GlobalData").GetComponent<GlobalData>().typeOfGame;
		GenerateHexesObjects();
		GenerateTroops();
		GenerateNameplates();
	}
}
