using UnityEngine;
using System.Collections;

public class Hex : MonoBehaviour {
	
	public int defaultStat = 0;
	public int currentStat = 0;
	
	public GameObject[] neighbor = new GameObject[6];
	
	public GameObject Unit;
	
	public GameObject LabelObject = null; 
}
