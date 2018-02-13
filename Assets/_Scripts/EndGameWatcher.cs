using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndGameWatcher : MonoBehaviour {

	public TurnManager turnManager;
	public TextMeshProUGUI winText;

	public Ship p1Destroyer;
	public Ship p2Destroyer;

	string winTextTemplate = "Player {0} wins!";
    ShipAudioManager audioManager;

	void Start() {
		StartCoroutine(EndCheckLoop());
        audioManager = GameObject.Find("ShipAudioManager").GetComponent<ShipAudioManager>();
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
            audioManager.mainThemeEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            SceneManager.LoadScene("MainMenu");
		}
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
		turnManager.isGameOver = true;
		winText.text = string.Format(winTextTemplate, winningPlayerNum.ToString());
		winText.gameObject.SetActive(true);
	}
}
