using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpHandler : MonoBehaviour {

    public float warpTime = 1f;
    public AnimationCurve warpCurve;
    public GameObject[] leftWarpees;
    private Vector3[] leftTransforms;
    public GameObject[] rightWarpees;
    private Vector3[] rightTransforms;
    public Vector3 warpDist;
	// Use this for initialization
	void Start () {
        // Store left transforms
        leftTransforms = new Vector3[leftWarpees.Length];
        for (int i = 0; i < leftWarpees.Length; i++) {
            leftWarpees[i].GetComponentInChildren<SpriteRenderer>().enabled = false;
            leftTransforms[i] = leftWarpees[i].GetComponent<Transform>().position;
            Debug.Log("Sending");
        }
        rightTransforms = new Vector3[rightWarpees.Length];
        for (int i = 0; i < rightWarpees.Length; i++) {
            rightWarpees[i].GetComponentInChildren<SpriteRenderer>().enabled = false;
            rightTransforms[i] = rightWarpees[i].GetComponent<Transform>().position;
            Debug.Log("Sending");
        }

        StartCoroutine(StartWarpRoutine());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Start bobbing until bobbing false and if ends and bobbing true the loop
    IEnumerator StartWarpRoutine() {
        // yield return new WaitForSeconds(.2f);
        float time = 0;
        while ((time < warpTime)) {

            float percent = time / warpTime;
            for (int i = 0; i < leftWarpees.Length; i++) {
                leftWarpees[i].GetComponentInChildren<SpriteRenderer>().enabled = true;
                leftWarpees[i].GetComponent<Transform>().position = Vector3.LerpUnclamped(leftTransforms[i]-warpDist, leftTransforms[i], warpCurve.Evaluate(percent));
            }
            for (int i = 0; i < rightWarpees.Length; i++) {
                rightWarpees[i].GetComponentInChildren<SpriteRenderer>().enabled = true;
                rightWarpees[i].GetComponent<Transform>().position = Vector3.LerpUnclamped(rightTransforms[i] + warpDist, rightTransforms[i], warpCurve.Evaluate(percent));
            }


            yield return new WaitForEndOfFrame();

            time += Time.deltaTime;
        }
       
    }
}
