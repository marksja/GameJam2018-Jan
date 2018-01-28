using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameWatcher : MonoBehaviour {

	public Text winText;

	public Ship p1Destroyer;
	public Ship p2Destroyer;

	string winTextTemplate = "Player {0} wins!";

	void Start() {
		StartCoroutine(EndCheckLoop());
	}

	IEnumerator EndCheckLoop() {
		while (true) {
			if (p1Destroyer.alive && !p2Destroyer.alive) {
				winText.text = string.Format(winTextTemplate, "1");
				winText.gameObject.SetActive(true);
				yield break;
			}
			else if (!p1Destroyer.alive && p2Destroyer.alive) {
				winText.text = string.Format(winTextTemplate, "2");
				winText.gameObject.SetActive(true);
				yield break;
			}
			yield return new WaitForSeconds(0.1f);
		}
	}
}
