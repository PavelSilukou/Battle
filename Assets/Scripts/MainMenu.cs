using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {
	
	public Texture BackgroundTexture;
	//public GUIStyle MenuStyle;
	public GUIStyle ButtonStyle;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI () {
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), BackgroundTexture, ScaleMode.StretchToFill, true, 0.0f);

		if (GUI.Button (new Rect (100, Screen.height - 200, 160, 40), "Player vs Player", ButtonStyle)) {
			GameObject.Find("GlobalData").GetComponent<GlobalData>().typeOfGame = 0;
			Application.LoadLevel ("MainArena");
		}
		
		if (GUI.Button (new Rect (100, Screen.height - 140, 160, 40), "Player vs Computer", ButtonStyle)) {
			GameObject.Find("GlobalData").GetComponent<GlobalData>().typeOfGame = 1;
			Application.LoadLevel ("MainArena");
		}
	
		if (GUI.Button (new Rect (100, Screen.height - 80, 160, 40), "Exit", ButtonStyle)) {
			Application.Quit();
		}
	}
}
