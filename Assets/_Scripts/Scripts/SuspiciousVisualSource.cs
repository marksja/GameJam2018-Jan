using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SuspiciousVisualSource : MonoBehaviour {

	public InvestigatePointEvent OnStopMoving;

	Rigidbody2D rb2d;

	public bool IsMoving { get { return rb2d.velocity.magnitude > 0f; } }

	void Start() {
		StartCoroutine(MoveCheck());
		rb2d = GetComponent<Rigidbody2D>();
	}

	IEnumerator MoveCheck() {
		yield return new WaitForEndOfFrame();
		while (rb2d.velocity.magnitude > 0f) {
			yield return new WaitForSeconds(0.1f);
		}
		OnStopMoving.Invoke(transform.position);
	}
}
