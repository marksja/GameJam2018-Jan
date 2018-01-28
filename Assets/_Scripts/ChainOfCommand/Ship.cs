using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ship : MonoBehaviour {

	public ShipData ship;

	public int health;

	public int damageTaken;
	public int shield;

	public bool alive = true;

	public TextMeshProUGUI text;

	public GameObject typeChoice;
	public GameObject targetChoice;

	public LineRenderer lineRenderer;
	public GameObject shieldObject;
	// Tutorial text
	public GameObject typeTutorial;
	public GameObject targetTutorial;

	public void Start(){
		lineRenderer = GetComponentInChildren<LineRenderer>();
		alive = true;
		damageTaken = 0;
		shield = 0;
        health = ship.maxHealth;
		text.text = health + "/" + ship.maxHealth;
	}

	public void TakeDamage(int damageAmnt){
		damageTaken += damageAmnt;
	}

	public void AddShield(int shieldAmnt){
		shield += shieldAmnt;
		Debug.Log("Hello? Added a shield");
		StartCoroutine(ShieldGrow());
	}

	public void ApplyDamage(){
		damageTaken -= shield;
		if(shield > 0) StartCoroutine(ShieldShrink());
		shield = 0;
		
		if(damageTaken > 0){
			health -= damageTaken;
		}
		damageTaken = 0;

		text.text = health + "/" + ship.maxHealth;

		if(health <= 0){
			alive = false;
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
		targetTutorial.SetActive(false);
	}	

	IEnumerator Laser(Vector3 target){
		
		Vector3 start = transform.position;

		lineRenderer.SetPosition(0, start);
		lineRenderer.SetPosition(1, start);

		float time = 0;
		while(time < 0.6f){
			
			lineRenderer.SetPosition(1, Vector3.Lerp(start, target, time / 0.6f));

			yield return new WaitForEndOfFrame();

			time += Time.deltaTime;

		}

		time = 0;
		while(time < 0.4f){
			lineRenderer.SetPosition(0, Vector3.Lerp(start, target, time / 0.4f));

			yield return new WaitForEndOfFrame();

			time += Time.deltaTime;
		}

		lineRenderer.SetPosition(0, start);
		lineRenderer.SetPosition(1, start);
	}

	public void LaserCaller(Vector3 target){
		StartCoroutine(Laser(target));
	}
	
	public void Shield(){

	}

	IEnumerator ShieldGrow(){
		float time = 0;
		while(time < 0.5f){
			shieldObject.transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(1, .5f, 0), time / 0.5f);

			yield return new WaitForEndOfFrame();

			time += Time.deltaTime;
		}
	}

	IEnumerator ShieldShrink(){
		float time = 0;
		while(time < 0.5f){
			shieldObject.transform.localScale = Vector3.Lerp(new Vector3(1, .5f, 0), Vector3.zero, time / 0.5f);

			yield return new WaitForEndOfFrame();

			time += Time.deltaTime;
		}
	}
	
	/* Tutorial blurbs - JF */
	public void ShowTypeTutorial() {
		typeTutorial.SetActive(true);
		targetTutorial.SetActive(false);
	}

	public void ShowTargetTutorial() {
		typeTutorial.SetActive(false);
		targetTutorial.SetActive(true);
	}
}
