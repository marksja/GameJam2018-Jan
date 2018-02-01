using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldHandler : MonoBehaviour {

    // Not actually sure if an enum is correct here --Cooper Riehl
    public enum ShieldState {
        Off, Regular, Super
    }

    // Animation variables
    public float shieldChangeTime = 0.5f;

    // Animation curves
    public AnimationCurve shieldGrowCurve;
    public AnimationCurve shieldShrinkCurve;
    public AnimationCurve shieldDieCurve;

    // Private state
    ShieldState shieldState;

    public void ShieldGrow() {
        if (shieldState == ShieldState.Off) {
            StartCoroutine(ChangeShieldSize(ShieldState.Regular));
        }
        else if (shieldState == ShieldState.Regular) {
            StartCoroutine(ChangeShieldSize(ShieldState.Super));
        }
    }

    public void ShieldShrink(float seconds) {
        if (shieldState != ShieldState.Off) {
            StartCoroutine(ShieldShrinkRoutine(seconds));
        }
    }

    IEnumerator ChangeShieldSize(ShieldState targetState) {
        float time = 0;
        Vector3 curSize = ShieldSizeFromState(shieldState);
        Vector3 targetSize = ShieldSizeFromState(targetState);
        
        shieldState = targetState;
        while (time < shieldChangeTime) {
            float percent = time / shieldChangeTime;
            this.transform.localScale = Vector3.LerpUnclamped(curSize, targetSize, shieldGrowCurve.Evaluate(percent));

            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
        }
    }

    IEnumerator ShieldShrinkRoutine(float seconds) {
        yield return new WaitForSeconds(seconds);
        yield return ChangeShieldSize(ShieldState.Off);
    }

    Vector3 ShieldSizeFromState(ShieldState targetState) {
        switch (targetState) {
            case ShieldState.Off:       return Vector3.zero;
            case ShieldState.Regular:   return new Vector3(2f, 2f, 2f);
            case ShieldState.Super:     return new Vector3(3f, 3f, 3f);
        }
        throw new UnityException("Invalid ShieldState!");
    }
}
