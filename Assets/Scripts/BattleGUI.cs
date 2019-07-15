using UnityEngine;
using System.Collections;

public class BattleGUI : MonoBehaviour {
	
	public GameObject activeUnit;
	public GUIStyle ButtonWait;
	public GUIStyle ButtonDef;
	public GUIStyle ButtonMenu;
	public GUIStyle LabelMenu;
	public GUIStyle LabelRound;
	public string labelRoundText;
	private short show = 0;
	private short gameOver = 0;
	
	public void ShowRoundCount (string roundCount) {
		labelRoundText = roundCount;
		show = 96;
	}
	
	public void ShowGameOver (string team) {
		labelRoundText = "Game Over\nTeam " + team + " Win";
		gameOver = 96;
		show = 96;
	}
	
	private void Update () {	
		if (show > 0) show--;
		else labelRoundText = "";
		if (gameOver > 1) gameOver--;
	}
	
	private void OnGUI () {
		
		if (activeUnit.GetComponent<UnitController>().unit.AI_Control == false) {
			if (activeUnit.GetComponent<UnitController>().unit.wait == 0)	
				if (GUI.Button(new Rect(Screen.width - 256, Screen.height - 128, 128.0F, 128.0F), "", ButtonWait))
					activeUnit.GetComponent<UnitController>().unit.Wait();
			
			if (GUI.Button(new Rect(Screen.width - 128, Screen.height - 128, 128.0F, 128.0F), "", ButtonDef))
				activeUnit.GetComponent<UnitController>().unit.Defence();
		}
		
		GUI.Label(new Rect(Screen.width / 2 - 32, 64.0F, 128.0F, 128.0F), labelRoundText, LabelRound);
		
		if (gameOver == 1) {
			GUI.Label(new Rect (0, 0, Screen.width, Screen.height), "", LabelMenu);
			if (GUI.Button(new Rect(Screen.width / 2 - 80, Screen.height / 2 - 30, 160, 40), "Try Again", ButtonMenu))
				Application.LoadLevel( "MainArena" );
			if (GUI.Button(new Rect(Screen.width / 2 - 80, Screen.height / 2 + 30, 160, 40), "Go to menu", ButtonMenu))
				Application.LoadLevel( "MainMenu" );
		}
	}
}
