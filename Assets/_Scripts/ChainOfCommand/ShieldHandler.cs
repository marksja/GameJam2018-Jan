using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldHandler : MonoBehaviour {

    // Animation variables
    public float shieldGrowTime = 1f;
    public float shieldShrinkTime = 1f;
    public float shieldDieTime = 1f;

    // Animation curves
    public AnimationCurve shieldGrowCurve;
    public AnimationCurve shieldShrinkCurve;
    public AnimationCurve shieldDieCurve;

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ShieldGrow(int shieldNum) {
        StartCoroutine(ShieldGrowRoutine(shieldNum));
    }

    public void ShieldShrink(float seconds) {
        StartCoroutine(ShieldShrinkRoutine(seconds));
    }

    public void ShieldSuperGrow() {
        StartCoroutine(ShieldSuperGrowRoutine());
    }

    IEnumerator ShieldGrowRoutine(int shieldNum) {
        float time = 0;
        while (time < shieldGrowTime) {

            float percent = time / shieldGrowTime;
            this.transform.localScale = Vector3.LerpUnclamped(Vector3.zero, new Vector3(2f, 2f, 0), shieldGrowCurve.Evaluate(percent));

            yield return new WaitForEndOfFrame();

            time += Time.deltaTime;
        }
        if(shieldNum > 1) {
            ShieldSuperGrow();
        }
    }

    IEnumerator ShieldShrinkRoutine(float seconds) {
        yield return new WaitForSeconds(seconds);
        float time = 0;
        while (time < shieldShrinkTime) {

            float percent = time / shieldShrinkTime;
            this.transform.localScale = Vector3.LerpUnclamped(this.transform.localScale, Vector3.zero, shieldShrinkCurve.Evaluate(percent));

            yield return new WaitForEndOfFrame();

            time += Time.deltaTime;
        }
    }

    IEnumerator ShieldSuperGrowRoutine() {
        float time = 0;
        while (time < shieldGrowTime) {

            float percent = time / shieldGrowTime;
            this.transform.localScale = Vector3.LerpUnclamped(new Vector3(2f, 2f, 0), new Vector3(3f, 3f, 0), shieldGrowCurve.Evaluate(percent));

            yield return new WaitForEndOfFrame();

            time += Time.deltaTime;
        }
    }


}
