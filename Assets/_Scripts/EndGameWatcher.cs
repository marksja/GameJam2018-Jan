using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndGameWatcher : MonoBehaviour {

	[FMODUnity.EventRef]
	public string victoryStinger;

	public ShipAudioManager shipAudioManager;
	public TurnManager turnManager;
	public TextMeshProUGUI winText;

	public Ship p1Destroyer;
	public Ship p2Destroyer;

	string winTextTemplate = "Player {0} wins!";

	void Start() {
		StartCoroutine(EndCheckLoop());
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
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
		shipAudioManager.EndMainMusic();
		FMOD.Studio.EventInstance victoryEvent = FMODUnity.RuntimeManager.CreateInstance(victoryStinger);
		victoryEvent.start();
		winText.text = string.Format(winTextTemplate, winningPlayerNum.ToString());
		winText.gameObject.SetActive(true);
	}
}
