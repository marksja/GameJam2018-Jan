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

    // Use this for initialization
    void Start() {
        inputs = new int[numberOfShips];
        for (int i = 0; i < numberOfShips; i++) {
            inputs[i] = -1;
        }
        StartCoroutine("ChooseCommands");
    }

    void Update() {
        if (needInput) {
            if (playerNumber == 0 ? Input.GetKeyDown(KeyCode.Q) : Input.GetKeyDown(KeyCode.LeftArrow)) {
                Debug.Log("Oy");
                input = 0;
                needInput = false;
            }
            else if (playerNumber == 0 ? Input.GetKeyDown(KeyCode.W) : Input.GetKeyDown(KeyCode.DownArrow)) {
                Debug.Log("Oof");
                input = 1;
                needInput = false;
            }
            else if (playerNumber == 0 ? Input.GetKeyDown(KeyCode.E) : Input.GetKeyDown(KeyCode.RightArrow)) {
                Debug.Log("Meh");
                input = 2;
                needInput = false;
            }
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
                while (needInput) { yield return new WaitForEndOfFrame(); } //Blocking until we have input
                attackId = (CommandQueue.Command)input;
            }
            else {
                attackId = (CommandQueue.Command)3; //Hold if we Heavy attacked last turn
            }

            needInput = true;
            while (needInput) { yield return new WaitForEndOfFrame(); } //Blocking until we have input
            targetId = input;

            orders.IssueCommand(shipId, attackId, targetId); //Give the order
            inputs[i] = (int)attackId; //Store the last attack 
        }
        yield return new WaitForSeconds(0);
    }
}
