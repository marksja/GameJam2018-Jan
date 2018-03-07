using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobbingHandler : MonoBehaviour {

    public GameObject bobbee;
    public float bobbingPeriod = 1f;
    public AnimationCurve bobbingCurve;

    private bool isBobbing = false;
    private Vector3 originalTransform;
    private bool waited = false;
	// Use this for initialization
	void Awake () {
        originalTransform = bobbee.GetComponent<Transform>().position;
        StartCoroutine(SetWait());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    // If not yet bobbing then start bobbing
    public void StartBobbing() {
        if (!isBobbing) {
            isBobbing = true;
            StartCoroutine(StartBobbingRoutine());
        }
    }

    // Stop bobbing immediately
    public void StopBobbing() {
        isBobbing = false;
    }

    IEnumerator SetWait() {
        if (!waited) {
            
            yield return new WaitForSeconds(1.1f);
            waited = true;
        }
    }

    // Start bobbing until bobbing false and if ends and bobbing true the loop
    IEnumerator StartBobbingRoutine() {
        if (!waited) {
            
            yield return new WaitForEndOfFrame();
            waited = true;
        }
        float time = 0;
        while ((time < bobbingPeriod) && isBobbing) {

            float percent = time / bobbingPeriod;
            bobbee.GetComponent<Transform>().position = Vector3.LerpUnclamped(originalTransform, new Vector3(originalTransform.x,originalTransform.y+1,originalTransform.z), bobbingCurve.Evaluate(percent));

            yield return new WaitForEndOfFrame();

            time += Time.deltaTime;
        }
        if (isBobbing) {
            StartCoroutine(StartBobbingRoutine());
        }
    }
}
