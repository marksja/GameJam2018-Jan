using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalSender : MonoBehaviour {

	public GameObject signalPrefab;

	GameObject startSignal;
	GameObject midSignal;
	GameObject finalSignal;

	void Start() {
		startSignal = null;
		midSignal = null;
		finalSignal = null;
	}

	[ContextMenu("Editor shit!")]
	public void Editor(){

		SendNewSignal(0);
	}

	public void SendNewSignal(int turn){
		finalSignal = midSignal;
		midSignal = startSignal;
		startSignal = GameObject.Instantiate(signalPrefab, this.transform.position, Quaternion.identity);
		
		StartCoroutine(LerpSignalStart());
		StartCoroutine(LerpSignalMiddle());
		//StartCoroutine(LerpSignalFinal());
	}

	IEnumerator LerpSignalStart(){
		float timeStart = 0;
		Vector3 ogScale = startSignal.transform.localScale;
		while(timeStart < 1f){
			

			startSignal.transform.localScale = Vector3.Lerp(ogScale, new Vector3(1.5f, 2f, 1f), timeStart / 1f);
			
			yield return new WaitForEndOfFrame();

			timeStart += Time.deltaTime;
		}
	}

	IEnumerator LerpSignalMiddle(){
		yield return new WaitForSeconds(.1f);
		if(midSignal == null){
			yield break;
		}
		float timeMid = 0;
		Vector3 ogScale = midSignal.transform.localScale;
		SpriteRenderer sr = midSignal.GetComponent<SpriteRenderer>();
		Color startAlpha = sr.color;
		
		while(timeMid < 2f){
			//midSignal.transform.localScale = Vector3.Lerp(ogScale, new Vector3(3.5f, 5f, 1f), timeMid / 1f);
			midSignal.transform.localScale = Vector3.Lerp(ogScale, new Vector3(8f, 6f, 1f), timeMid / 2f);
			if (timeMid >= 1f) {
				sr.color = Color.Lerp(startAlpha, new Color(startAlpha.r, startAlpha.g, startAlpha.b, 0), (timeMid - 1f) / 1f);
			}
			
			yield return new WaitForEndOfFrame();

			timeMid += Time.deltaTime;
		}
		Destroy(midSignal);
	}

	IEnumerator LerpSignalFinal(){
		yield return new WaitForSeconds(.2f);
		if(finalSignal == null){
			yield break;
		}
		float timeFinal = 0;
		Vector3 ogScale = finalSignal.transform.localScale;
		SpriteRenderer sr = finalSignal.GetComponent<SpriteRenderer>();
		Color startAlpha = sr.color;
		
		while(timeFinal < 1f){
			finalSignal.transform.localScale = Vector3.Lerp(ogScale, new Vector3(8f, 6f, 1f), timeFinal / 1f);
			sr.color = Color.Lerp(startAlpha, new Color(startAlpha.r, startAlpha.g, startAlpha.b, 0), timeFinal / 1f);
			
			yield return new WaitForEndOfFrame();

			timeFinal += Time.deltaTime;
		}
		Destroy(finalSignal);
	}
}
