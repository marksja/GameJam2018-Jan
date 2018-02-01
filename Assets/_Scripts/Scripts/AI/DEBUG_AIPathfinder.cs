using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Placeholder class for A* pathfinding
public class DEBUG_AIPathfinder : MonoBehaviour {

	public float speed;

	Rigidbody2D rb2d;
	Vector3 targetPoint;

	void Start() {
		rb2d = GetComponent<Rigidbody2D>();
		targetPoint = transform.position;
		StartCoroutine(MovementLoop());
	}

	IEnumerator MovementLoop() {
		while (true) {
			if (Vector3.Distance(transform.position, targetPoint) > 0.05f) {
				Vector3 directionToTarget = (targetPoint - transform.position);
				rb2d.velocity = speed * directionToTarget.normalized;
			}
			else {
				rb2d.velocity = Vector3.zero;
			}
			yield return new WaitForEndOfFrame();
		}
	}

	public void GoToPoint(Vector3 point) {
		transform.LookAt(point);
		targetPoint = point;
	}
}
