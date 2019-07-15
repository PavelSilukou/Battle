using UnityEngine;
using System.Collections;

public class GlobalData : MonoBehaviour {
	
	//0 - player vs player; 1 - player vs computer;
	public int typeOfGame;
	
	void Start () {
		DontDestroyOnLoad(this);
	}
}
