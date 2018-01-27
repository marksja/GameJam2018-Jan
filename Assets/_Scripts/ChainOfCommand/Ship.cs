using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ship : MonoBehaviour {

	public ShipData ship;

	public int health;

	public int damageTaken;
	public int shield;

	public TextMeshProUGUI text;

	public GameObject typeChoice;
	public GameObject targetChoice;

	public void Start(){
		damageTaken = 0;
        health = ship.maxHealth;
		text.text = health + "/" + ship.maxHealth;
	}

	public void TakeDamage(int damageAmnt){
		damageTaken += damageAmnt;
	}

	public void AddShield(int shieldAmnt){
		shield += shieldAmnt;
	}

	public void ApplyDamage(){
		damageTaken -= shield;
		shield = 0;
		if(damageTaken > 0){
			health -= damageTaken;
		}
		damageTaken = 0;

		text.text = health + "/" + ship.maxHealth;

		if(health <= 0){
			this.gameObject.SetActive(false);
		}
	}

	public void ShowAttackTypeChoice(){
		typeChoice.SetActive(true);
		targetChoice.SetActive(false);
	}

	public void ShowTargetChoice(){
		typeChoice.SetActive(false);
		targetChoice.SetActive(true);
	}

	public void HideAll(){
		typeChoice.SetActive(false);
		targetChoice.SetActive(false);
	}	
	
}
