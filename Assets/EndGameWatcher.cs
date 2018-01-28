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
		int winningPlayerNum = 0;
		while (winningPlayerNum == 0) {
			yield return new WaitForSeconds(0.1f);
			if (p1Destroyer.alive && !p2Destroyer.alive) {
				winningPlayerNum = 1;
			}
			else if (!p1Destroyer.alive && p2Destroyer.alive) {
				winningPlayerNum = 2;
			}
		}
		winText.text = string.Format(winTextTemplate, winningPlayerNum.ToString());
		winText.gameObject.SetActive(true);
	}
}
