using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuspiciousNoiseSource : MonoBehaviour {

	public float noiseRadius = 5f;
	public float expansionTime = 0.2f;

	CircleCollider2D noiseCollider;

	void Start() {
		noiseCollider = GetComponent<CircleCollider2D>();
		noiseCollider.radius = 0f;
		StartCoroutine(ExpandNoiseHitbox());
	}
	
	IEnumerator ExpandNoiseHitbox() {
		for (float t = 0f; t < expansionTime; t += Time.deltaTime) {
			noiseCollider.radius = Mathf.Lerp(0f, noiseRadius, t / expansionTime);
			yield return new WaitForEndOfFrame();
		}
		noiseCollider.radius = noiseRadius;
		yield return new WaitForEndOfFrame();
		noiseCollider.enabled = false;
	}
}
