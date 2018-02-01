using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SuspiciousNoiseResponder : MonoBehaviour {

	public InvestigatePointEvent OnHearNoise;

	void OnTriggerEnter2D(Collider2D other) {
		SuspiciousNoiseSource noiseSource = other.GetComponent<SuspiciousNoiseSource>();
		if (noiseSource != null) {
			OnHearNoise.Invoke(noiseSource.transform.position);
		}
	}
}
