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
    public LineRenderer lineRendererBig;
    public GameObject bigLaser;

	public GameObject shieldObject;
	// Tutorial text
	public GameObject typeTutorial;
	public GameObject targetTutorial;
    // Effects
    public float bobbingPeriod = 1f;
    public AnimationCurve bobbingCurve;

    private float bobbingTime = 0;
    private bool isBobbing = false;
    private Vector3 originalTransform;

    public ShipAudioManager shipAudioManager;
	public int playerNum;

    private ShieldHandler shieldHandler;

    private Transform mainSpriteTransform;
    private float origScaleY;

	void Awake() {
        shieldHandler = shieldObject.GetComponent<ShieldHandler>();
        mainSpriteTransform = transform.Find("Sprite");
        origScaleY = mainSpriteTransform.localScale.y;
        lineRenderer = GetComponentInChildren<LineRenderer>();
    }

	public void Start(){
		alive = true;
		damageTaken = 0;
		shield = 0;
        health = shipData.maxHealth;
        text.text = health + "/" + shipData.maxHealth;

        if (bigLaser != null) { bigLaser.SetActive(false); }

        WarpIn();
	}

    public void Update() {
        if (isBobbing) {
            if (bobbingTime < bobbingPeriod) {
                float percent = bobbingTime / bobbingPeriod;
                transform.position = Vector3.LerpUnclamped(
                    originalTransform,
                    new Vector3(originalTransform.x, originalTransform.y + 1, originalTransform.z),
                    bobbingCurve.Evaluate(percent)
                    );

                bobbingTime += Time.deltaTime;
            }
            else {
                bobbingTime = 0;
            }
        }
    }

    public void TakeDamage(int damageAmnt){
		damageTaken += damageAmnt;
	}

	public void AddShield(int shieldAmnt){
		FMODUnity.RuntimeManager.PlayOneShot(shipData.shieldEvent, transform.position);
		shield += shieldAmnt;
        
        shieldHandler.ShieldGrow();
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
			shipAudioManager.ShipDied(playerNum);
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
        StartBobbing();
    }

	public void ShowTargetChoice(){
        typeChoice.SetActive(false);
        targetChoice.SetActive(true);
        StartBobbing();
    }

	public void HideAll(){
        
		typeChoice.SetActive(false);
		targetChoice.SetActive(false);
        if (targetTutorial != null) {
            targetTutorial.SetActive(false);
        }
        // Juice
        StopBobbing();
    }

    // If not yet bobbing then start bobbing
    public void StartBobbing() {
        if (!isBobbing) {
            originalTransform = transform.position;
            isBobbing = true;
        }
    }

    // Stop bobbing immediately
    public void StopBobbing() {
        transform.position = originalTransform;
        bobbingTime = 0;
        isBobbing = false;
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

    IEnumerator LaserBig(Vector3 target) {
        Vector3 start = transform.position;

        bigLaser.SetActive(true);
        bigLaser.GetComponent<LaserAnimator>().AnimateLaser();
        lineRendererBig.SetPosition(0, start);
        lineRendererBig.SetPosition(1, start);

        bigLaser.transform.position = start;

        // Get target location
        Vector3 targetDir = target - bigLaser.transform.position;
        // Get angle
        float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
        bigLaser.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        
        float time = 0;
        while (time < 1f) { yield return new WaitForEndOfFrame(); }
         
        bigLaser.SetActive(false);
    }

    IEnumerator HeavyLaser(Vector3 target) {
		lineRendererBig.widthMultiplier *= 2;
		yield return LaserBig(target);
		lineRendererBig.widthMultiplier /=2;
	}

	public void LaserCaller(Vector3 target){
		FMODUnity.RuntimeManager.PlayOneShot(shipData.lightAttackEvent, transform.position);
        StartCoroutine(Laser(target));
	}

	public void HeavyLaserCaller(Vector3 target){
		FMODUnity.RuntimeManager.PlayOneShot(shipData.heavyAttackEvent, transform.position);
		StartCoroutine(HeavyLaser(target));
	}

    public void WarpIn() {
        StartCoroutine(WarpInAnimation());
    }


    IEnumerator WarpInAnimation() {
        float time = 0f;
        while (time < 1) {
            mainSpriteTransform.localScale = new Vector3(mainSpriteTransform.localScale.x, time * origScaleY, 0);
            time += Time.deltaTime * 1.5f;
            yield return null;
        }
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
