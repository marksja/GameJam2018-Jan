using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AILineOfSight : MonoBehaviour {

	public DEBUG_AIPathfinder aIPathfinder;

	void OnTriggerEnter2D(Collider2D other) {
		SuspiciousVisualSource suspiciousVisualSource = other.GetComponent<SuspiciousVisualSource>();
		if (suspiciousVisualSource != null) {
			if (suspiciousVisualSource.IsMoving) {
				suspiciousVisualSource.OnStopMoving.AddListener(aIPathfinder.GoToPoint);
			}
			else {
				aIPathfinder.GoToPoint(suspiciousVisualSource.transform.position);
			}
		}
	}
}
