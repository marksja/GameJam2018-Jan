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

    public InputHandler p1InputHandler;
    public InputHandler p2InputHandler;

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
        string win_text = "";
		while (win_text == "") {
			yield return new WaitForSeconds(0.1f);
			if (p1Destroyer.alive && !p2Destroyer.alive) {
                win_text = string.Format(winTextTemplate, 1.ToString());
            }
			else if (!p1Destroyer.alive && p2Destroyer.alive) {
                win_text = string.Format(winTextTemplate, 2.ToString());
            }
            else if (!p1Destroyer.alive && !p2Destroyer.alive) {
                win_text = "Draw!";
            }
		}
		turnManager.isGameOver = true;
        winText.text = win_text;
        winText.gameObject.SetActive(true);

        // disable further player-specific input
        p1InputHandler.enabled = false;
        p2InputHandler.enabled = false;
	}
}
