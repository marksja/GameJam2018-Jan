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
    private int[] prevInputs;

    // Use this for initialization
    void Start() {
        prevInputs = new int[numberOfShips];
        for (int i = 0; i < numberOfShips; i++) {
            prevInputs[i] = -1;
        }

        StartCoroutine("ChooseCommands");
    }

    void Update() {
        if (needInput) {
            if (playerNumber == 0 ? Input.GetKeyDown(KeyCode.Q) : Input.GetKeyDown(KeyCode.I)) {
                //Debug.Log("Oy");
                input = 0;
                needInput = false;
            }
            else if (playerNumber == 0 ? Input.GetKeyDown(KeyCode.W) : Input.GetKeyDown(KeyCode.O)) {
                //Debug.Log("Oof");
                input = 1;
                needInput = false;
            }
            else if (playerNumber == 0 ? Input.GetKeyDown(KeyCode.E) : Input.GetKeyDown(KeyCode.P)) {
                //Debug.Log("Meh");
                input = 2;
                needInput = false;
            }
        }
    }

    public void StartTurn(int turnStart) {
        Debug.Log("Beginning New Turn");
        StartCoroutine("ChooseCommands");
    }

    IEnumerator ChooseCommands() {
        int shipId = -1;
        CommandQueue.Command attackId;
        int targetId = -1;

        //We go through all the ships we have and choose the ship Id, the attack the choose, and their target
        for (int i = 0; i < numberOfShips; i++) {
            if(!orders.ships[i].alive){
                continue;
            }

            shipId = i;

            orders.ships[i].ShowAttackTypeChoice();

            //1 is a heavy attack
            if (prevInputs[i] != 1) {
                needInput = true;
                while (needInput) { yield return new WaitForEndOfFrame(); } //Blocking until we have input
                Debug.Log("Input Attack: " + input + " for ship " + i);
                attackId = (CommandQueue.Command)input;
            
                orders.ships[i].ShowTargetChoice();

                needInput = true;
                while (needInput) { yield return new WaitForEndOfFrame(); } //Blocking until we have input
                Debug.Log("Input Target: " + input + " for ship " + i);
                targetId = input;
            }
            else {
                attackId = CommandQueue.Command.Hold; //Hold if we Heavy attacked last turn
            }

            orders.ships[i].HideAll();

            orders.IssueCommand(shipId, attackId, targetId); //Give the order
            prevInputs[i] = (int)attackId; //Store the last attack 
        }
        TurnManager.Instance.TurnComplete();
        yield return new WaitForSeconds(0);
    }
}
