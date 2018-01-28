using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using DG.Tweening;

[RequireComponent(typeof(AudioSource))]
public class CutsceneManager : MonoBehaviour {

	struct BeatSyncedCommand {
		int beatToStart;
		Coroutine commandToExecute;
	}

	// Inspector references
	public GameObject p1TransmissionText;
	public GameObject p2TransmissionText;
	public Transform p1CommanderPortrait;
	public Transform p2CommanderPortrait;

    public GameObject p1MissionText;
    public GameObject p2MissionText;

	// Public variables
	public float beatsPerMinute = 144f;

	// Private variables
	float secondsPerBeat;

	void Start() {
		// Calculate secondsPerBeat for use in coroutine waits
		float beatsPerSecond = beatsPerMinute / 60f;
		secondsPerBeat = 1f / beatsPerSecond;

		DisableUIElements();
		StartCoroutine(PlayCutscene());
	}

	void DisableUIElements() {
		p1TransmissionText.SetActive(false);
		p2TransmissionText.SetActive(false);
        p1MissionText.SetActive(false);
        p2MissionText.SetActive(false);
}

	// Returns the number of seconds required for the given number of beats to pass.
	float BeatsToSeconds(int numBeats) {
		return numBeats * secondsPerBeat;
	}
	
	IEnumerator PlayCutscene() {
		yield return WaitForBeats(2);
		// Start audio
		GetComponent<AudioSource>().Play();
		// There is a beat of silence at the start of the audio clip.
		// This line may need to be removed in the future.
		yield return WaitForBeats(1);

		// Flash the first transmission text for the first "call" sound
		StartCoroutine(FlashTransmission(p1TransmissionText, 16, 2));
		yield return WaitForBeats(8);

		// Add in the second transmission text for the second "call" sound
		StartCoroutine(FlashTransmission(p2TransmissionText, 8, 2));
		yield return WaitForBeats(6);

        // Two beats before the main theme, slide in the commander portraits
        // and open their textboxes
        //p1CommanderPortrait.DOLocalMoveX(0, BeatsToSeconds(1));
        //p2CommanderPortrait.DOLocalMoveX(0, BeatsToSeconds(1));

        // Display commander text
        p1MissionText.SetActive(true);
        p2MissionText.SetActive(true);

        //for (int i = 0; i < p1MissionText.GetComponent<TMPro.TextMeshProUGUI>().textInfo.characterCount; ++i) {
        //    p1MissionText.GetComponent<TMPro.TextMeshProUGUI>().textInfo.characterInfo[i].isVisible = false;
        //    p2MissionText.GetComponent<TMPro.TextMeshProUGUI>().textInfo.characterInfo[i].isVisible = false;
        //}


        //for (int i = 0; i < p1MissionText.GetComponent<TMPro.TextMeshProUGUI>().textInfo.characterCount; ++i) {
        //    p1MissionText.GetComponent<TMPro.TextMeshProUGUI>().textInfo.characterInfo[i].isVisible = true;
        //    p2MissionText.GetComponent<TMPro.TextMeshProUGUI>().textInfo.characterInfo[i].isVisible = true;
        //    yield return new WaitForSeconds(.1f);
        //}

    }

	// Toggle the "Incoming Transmission" text on and off for an amount of time.
	// This function ensures the object will always end in the disabled state.
	// textObj is the transmission text object to toggle
	// numBeats is the number of beats to spend toggling
	// flashFreq is the frequency with which the object should switch from
	//   enabled to disabled
	IEnumerator FlashTransmission(GameObject textObj, int numBeats, int flashFreq) {
		textObj.SetActive(true);
		for (int beatsPassed = 0; beatsPassed < numBeats; beatsPassed += flashFreq) {
			yield return WaitForBeats(flashFreq);
			textObj.SetActive(!textObj.activeInHierarchy);
		}
		textObj.SetActive(false);
	}

	// Use this function to wait for a number of beats in the music track
	IEnumerator WaitForBeats(int beatsToWait) {
		float t = 0f;
		for (int beats = 0; beats < beatsToWait; ++beats) {
			while (t < secondsPerBeat) {
				yield return new WaitForEndOfFrame();
				t += Time.deltaTime;
			}
			t -= secondsPerBeat;
		}
	}
}
