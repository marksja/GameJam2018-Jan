using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class InputHandler : MonoBehaviour
{
    [Range(0, 1)] //There's two players, its hard coded, go cry about it
    public int playerNumber;
    public int numberOfShips = 3;
    public CommandQueue orders;

    private bool controllersIn = false;
    private bool needInput = false;
    private int input;
    private int[] inputs;
    private InputDevice device;

    // Use this for initialization
    void Start() {
        device = InputManager.Devices[playerNumber];
        inputs = new int[numberOfShips];
        for (int i = 0; i < numberOfShips; i++) {
            inputs[i] = -1;
        }
        StartCoroutine("ChooseCommands");
    }

    void Update() {
        while (needInput) {
            if (device.Action1) {
                Debug.Log("Oy");
                input = 0;
                needInput = false;
            }
            else if (device.Action2) {
                Debug.Log("Oof");
                input = 1;
                needInput = false;
            }
            else if (device.Action3) {
                Debug.Log("Meh");
                input = 2;
                needInput = false;
            }
        }
    }

    IEnumerator WaitForInput() {
        while (true) {
            if (device.Action1) {
                Debug.Log("Oy");
                input = 0;
                needInput = false;
            }
            else if (device.Action2) {
                Debug.Log("Oof");
                input = 1;
                needInput = false;
            }
            else if (device.Action3) {
                Debug.Log("Meh");
                input = 2;
                needInput = false;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator ChooseCommands() {
        int shipId = -1;
        CommandQueue.Command attackId;
        int targetId = -1;

        //We go through all the ships we have and choose the ship Id, the attack the choose, and their target
        for (int i = 0; i < numberOfShips; i++) {
            shipId = i;

            if (inputs[i] != 1) {
                needInput = true;
                StartCoroutine("WaitForInput");
                while (needInput) { yield return new WaitForEndOfFrame(); } //Blocking until we have input
                attackId = (CommandQueue.Command)input;
            }
            else {
                attackId = (CommandQueue.Command)3; //Hold if we Heavy attacked last turn
            }

            needInput = true;
            StartCoroutine("WaitForInput");
            while (needInput) { yield return new WaitForEndOfFrame(); } //Blocking until we have input
            targetId = input;

            orders.IssueCommand(shipId, attackId, targetId); //Give the order
            inputs[i] = (int)attackId; //Store the last attack 
        }
        yield return new WaitForSeconds(0);
    }
}
