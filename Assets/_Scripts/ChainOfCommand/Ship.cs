using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour {

	public ShipData ship;

	public int health;

	public int damageTaken;

	public void Start(){
		damageTaken = 0;
        health = ship.maxHealth;
	}

	public void TakeDamage(int damageAmnt){
		damageTaken += damageAmnt;
	}

	public void ApplyDamage(){
		if(damageTaken > 0){
			health -= damageTaken;
		}
		damageTaken = 0;

		if(health <= 0){
			this.gameObject.SetActive(false);
		}
	}
	
}
