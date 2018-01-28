using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ship : MonoBehaviour {

	public ShipData shipData;

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
    // Effects
    public GameObject bobObject;

    private ShieldHandler shieldHandler;
    private BobbingHandler bobbingHandler;

	public void Awake(){
		lineRenderer = GetComponentInChildren<LineRenderer>();
		alive = true;
		damageTaken = 0;
		shield = 0;
        health = shipData.maxHealth;
        text.text = health + "/" + shipData.maxHealth;
        shieldHandler = shieldObject.GetComponent<ShieldHandler>();
        bobbingHandler = bobObject.GetComponent<BobbingHandler>();

        

	}

	public void TakeDamage(int damageAmnt){
		damageTaken += damageAmnt;
	}

	public void AddShield(int shieldAmnt){
		shield += shieldAmnt;
        
        shieldHandler.ShieldGrow(shield);
        
	}

	public void ApplyDamage(){
		damageTaken -= shield;
		if(shield > 0) TurnOffShieldInSeconds(1);
        shield = 0;
		
		if(damageTaken > 0){
			health -= damageTaken;
		}
		damageTaken = 0;

		text.text = health + "/" + shipData.maxHealth;

		if(health <= 0){
			alive = false;
			this.gameObject.SetActive(false);
		}
        TurnOffShieldInSeconds(0);

    }

    public void TurnOffShieldInSeconds(float seconds) {
        shieldHandler.ShieldShrink(seconds);
    }

    public void ShowAttackTypeChoice() {       
        typeChoice.SetActive(true);
        targetChoice.SetActive(false);
        // Juice
        bobbingHandler.StartBobbing();
    }

	public void ShowTargetChoice(){
        typeChoice.SetActive(false);
        targetChoice.SetActive(true); 
	}

	public void HideAll(){
        
		typeChoice.SetActive(false);
		targetChoice.SetActive(false);
        if (targetTutorial != null) {
            targetTutorial.SetActive(false);
        }
        // Juice
        bobbingHandler.StopBobbing();
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

	
	
	/* Tutorial blurbs - JF */
	public void ShowTypeTutorial() {
		if (targetTutorial != null) {
			targetTutorial.SetActive(false);
		}
		if (typeTutorial != null) {
			typeTutorial.SetActive(true);
		}
	}

	public void ShowTargetTutorial() {
		if (typeTutorial != null) {
			typeTutorial.SetActive(false);
		}
		if (targetTutorial != null) {
			targetTutorial.SetActive(true);
		}
	}
}
