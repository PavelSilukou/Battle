using UnityEngine;
using System.Collections;

public class UnitController : MonoBehaviour {
	
	public int typeUnit;
	
	public int team = 0;
	
	public int count;
	public int MinDamage;
	public int MaxDamage;
	public int defaultHealth;
	public int currentHealth;
	public int Speed;
	public int Initiative;
	
	public DamageText HealthDecText;
	public DamageText UnitsDecText;
	public DamageTexture HealthDecTexture;
	public DamageTexture UnitsDecTexture;
	public Arrow arrow;

	public UnitData unit;
	
	private void Awake () {
		if (typeUnit == 0) unit = new MeleeWalkUnit();
		else if (typeUnit == 1) unit = new RangeWalkUnit();
		unit.InitData(team, count, MinDamage, MaxDamage, defaultHealth, 
						currentHealth, Speed, Initiative, arrow, gameObject, 
						GetComponent<Animation>(), GameObject.Find("Controller"), HealthDecText, UnitsDecText, HealthDecTexture, UnitsDecTexture);
	}
	
	private void Update () {
		
		unit.Evaluate(transform);
		
	}
	
}