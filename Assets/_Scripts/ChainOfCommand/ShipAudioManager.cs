using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipAudioManager : MonoBehaviour {

	[FMODUnity.EventRef]
	public string mainMusic;
    public FMOD.Studio.EventInstance mainThemeEvent;

    FMOD.Studio.ParameterInstance FMODshipCount;
	int p1ShipCount;
	int p2ShipCount;

	void Start() {
		p1ShipCount = 3;	// We assume each player starts with 3 ships
		p2ShipCount = 3;
		mainThemeEvent = FMODUnity.RuntimeManager.CreateInstance(mainMusic);
		mainThemeEvent.getParameter("numShips", out FMODshipCount);
        mainThemeEvent.start();
	}

	// Called when one of a player's ships dies
	public void ShipDied(int playerNum) {
		if (playerNum == 1) {
			p1ShipCount--;
		}
		else if (playerNum == 2) {
			p2ShipCount--;
		}
		int minShipCount = Mathf.Min(p1ShipCount, p2ShipCount);
		float curShipCount;
		FMODshipCount.getValue(out curShipCount);
		if (minShipCount < (int)curShipCount) {
			FMODshipCount.setValue(minShipCount);
		}
	}
}
