using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class InputHandler : MonoBehaviour {
    [Range(0,1)] //There's two players, its hard coded, go cry about it
    public int playerNumber;
    public int numberOfShips = 3;
    public CommandQueue orders;

    private bool controllersIn = false;
    private int[] inputs;
    private InputDevice device;
    
	// Use this for initialization
	void Start () {
        device = InputManager.Devices[playerNumber];
        inputs = new int[numberOfShips];
        for(int i = 0; i < numberOfShips; i++) {
            inputs[i] = -1;
        }
        StartCoroutine("ChooseCommands");
    }
	
    int WaitForInput() {
        while (true) {
            if (device.Action1) {
                Debug.Log("Oy");
                return 0;
            }
            else if (device.Action2) {
                Debug.Log("Oof");
                return 1;
            }
            else if (device.Action3) {
                Debug.Log("Meh");
                return 2;
            }
        }
    }

    IEnumerator ChooseCommands() {
        int shipId = -1;
        CommandQueue.Command attackId;
        int targetId = -1;

        for (int i = 0; i < numberOfShips; i++) {
            shipId = WaitForInput();
            if (inputs[i] != 1) {
                attackId = (CommandQueue.Command)WaitForInput();
            }
            else {
                attackId = (CommandQueue.Command)3; //Hold if we Heavy attacked last turn
            }
            targetId = WaitForInput();

            orders.IssueCommand(shipId, attackId, targetId); //Give the order
            inputs[i] = (int)attackId; //Store the last attack 
        }
        yield return new WaitForSeconds(0);
    }
}
