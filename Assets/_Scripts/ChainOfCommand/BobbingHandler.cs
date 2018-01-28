﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobbingHandler : MonoBehaviour {

    public GameObject bobbee;
    public float bobbingPeriod = 1f;
    public AnimationCurve bobbingCurve;

    private bool isBobbing = false;
    private Vector3 originalTransform;
    
	// Use this for initialization
	void Start () {
        originalTransform = bobbee.GetComponent<Transform>().position;
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

    // Start bobbing until bobbing false and if ends and bobbing true the loop
    IEnumerator StartBobbingRoutine() {
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
