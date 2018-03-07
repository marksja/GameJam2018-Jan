using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Inventory/List", order = 1)]
public class ShipData : ScriptableObject {
    public enum Type { CORVETTE, DESTROYER };
    
	public int maxHealth;
    public Type type;

	public CommandQueue.Command[] actions;

	public int lightDamage;
	public int heavyDamage;
	public int shieldHealth;

	[FMODUnity.EventRef]
	public string shieldEvent;
	[FMODUnity.EventRef]
	public string lightAttackEvent;
	[FMODUnity.EventRef]
	public string heavyAttackEvent;
}
