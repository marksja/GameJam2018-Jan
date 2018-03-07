﻿using System.Collections;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [Range(0, 1)] //There's two players, its hard coded, go cry about it
    public int playerNumber;
    public int numberOfShips = 3;
    public CommandQueue orders;

    private bool needAction = true;
    private bool needInput = false;
    private bool onCooldown = false;
    private int input;
    private CommandQueue.Command[] prevInputs;
    private CommandQueue.Command[] prevprevInputs;

    // JF: Hacky way to control when tutorials appear
    private bool displayTutorial = true;
    private bool displayDelayTutorial = false;

    class FancyTuple
    {
        public int me;
        public CommandQueue.Command attac;
        public int target;

        public FancyTuple(int x, CommandQueue.Command y, int z) {
            me = x;
            attac = y;
            target = z;
        }
    };

    private FancyTuple[] ordersForThisTurn;

    // Use this for initialization
    void Start() {
        ordersForThisTurn = new FancyTuple[3];
        prevInputs = new CommandQueue.Command[numberOfShips];
        prevprevInputs = new CommandQueue.Command[numberOfShips];
        for (int i = 0; i < numberOfShips; i++) {
            prevInputs[i] = CommandQueue.Command.Hold;
            prevprevInputs[i] = CommandQueue.Command.Hold;
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
            else if (!needAction && (playerNumber == 0 ? Input.GetKeyDown(KeyCode.E) : Input.GetKeyDown(KeyCode.P))) {
                //Debug.Log("Meh");
                input = 2;
                needInput = false;
                needAction = true;
            }
            else if (playerNumber == 0 ? Input.GetKeyDown(KeyCode.R) : Input.GetKeyDown(KeyCode.U)) {
                //Undo Button
                input = 3;
                needInput = false;
            }
        }
    }

    public void StartTurn(int turnStart) {
        StartCoroutine("ChooseCommands");
    }

    IEnumerator ChooseCommands() {
        CommandQueue.Command attackId = CommandQueue.Command.Hold;
        int targetId = -1;

        //We go through all the ships we have and choose the ship Id, the attack the choose, and their target
        for (int id = 0; id < numberOfShips; id++) {
            if(!orders.ships[id].alive){
                continue;
            }

            Attack:
            orders.ships[id].ShowAttackTypeChoice();
            
            // Display tutorial blurbs first time that commands are requested
            if (displayTutorial) { orders.ships[id].ShowTypeTutorial(); }

            if (onCooldown && id == 1) {
                attackId = CommandQueue.Command.Hold;
            }
            else {
                input = 4;
                while (input > 2) {
                    needInput = true;
                    needAction = true;
                    while (needInput) { yield return new WaitForEndOfFrame(); } //Blocking until we have input
                    if (input == 3 && id > 0) {
                        orders.ships[id].HideAll();
                        id -= 1;
                        if(onCooldown && id == 1) {
                            id -= 1;
                        }
                        orders.ships[id].ShowTargetChoice();
                        prevInputs = prevprevInputs;
                        goto Target;
                    } //Go Back
                }

                attackId = FindAttack(id, input);
            
                orders.ships[id].ShowTargetChoice();

                if (displayTutorial) { 
                    orders.ships[id].ShowTargetTutorial();
                }
                Target:
                input = -1;
                while (!ShipIsAlive(input, attackId)) {
                    needInput = true;
                    needAction = false;
                    while (needInput) { yield return new WaitForEndOfFrame(); } //Blocking until we have input
                    if (input == 3) { attackId = CommandQueue.Command.Hold;
                        orders.ships[id].HideAll();
                        goto Attack;
                    } //Go back
                }
                
                targetId = input;
            }

            orders.ships[id].HideAll();
            FancyTuple order = new FancyTuple(id, attackId, targetId);
            ordersForThisTurn[id] = order;
            prevprevInputs[id] = prevInputs[id];
            prevInputs[id] = attackId; //Store the last attack
        }

        // Command tutorials are only displayed the first time
        // Display the delay tutorial on the next turn
        displayTutorial = false;
        displayDelayTutorial = true;

        for(int i = 0; i < 3; i++) {
            int shipId = ordersForThisTurn[i].me;
            attackId = ordersForThisTurn[i].attac;
            targetId = ordersForThisTurn[i].target;
            if (orders.ships[shipId].alive) {
                if (attackId == CommandQueue.Command.Heavy) { onCooldown = true; }
                if (attackId == CommandQueue.Command.Hold && shipId == 1) { onCooldown = false; }
                orders.IssueCommand(shipId, attackId, targetId); //Give the order
            }
        }
        TurnManager.Instance.TurnComplete();
        yield return new WaitForSeconds(0);
    }

    bool ShipIsAlive(int targetId, CommandQueue.Command attackId) {
        if (targetId < 0) return false;
        switch (attackId) {
            case CommandQueue.Command.Shield: {
                // Ensure requested allied ship is alive
                return orders.ships[targetId].alive;
            }
            default: {
                // Offensive; ensure requested enemy ship is alive
                return orders.opponentQueue.ships[targetId].alive;
            }
        }
    }

    CommandQueue.Command FindAttack(int ship, int input) {
        return orders.ships[ship].shipData.actions[input];
    }
}
