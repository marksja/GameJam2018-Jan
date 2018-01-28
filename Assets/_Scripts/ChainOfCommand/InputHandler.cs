using System.Collections;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [Range(0, 1)] //There's two players, its hard coded, go cry about it
    public int playerNumber;
    public int numberOfShips = 3;
    public CommandQueue orders;

    private bool needAction = true;
    private bool needInput = false;
    private int input;
    private CommandQueue.Command[] prevInputs;

    private bool displayTutorial = true;

    // Use this for initialization
    void Start() {
        prevInputs = new CommandQueue.Command[numberOfShips];
        for (int i = 0; i < numberOfShips; i++) {
            prevInputs[i] = CommandQueue.Command.Hold;
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
        }
    }

    public void StartTurn(int turnStart) {
        StartCoroutine("ChooseCommands");
    }

    IEnumerator ChooseCommands() {
        CommandQueue.Command attackId;
        int targetId = -1;

        //We go through all the ships we have and choose the ship Id, the attack the choose, and their target
        for (int id = 0; id < numberOfShips; id++) {
            if(!orders.ships[id].alive){
                continue;
            }

            orders.ships[id].ShowAttackTypeChoice();
            
            // Display tutorial blurbs first time that commands are requested
            if (displayTutorial) { orders.ships[id].ShowTypeTutorial(); }

            if (prevInputs[id] != CommandQueue.Command.Heavy) {
                needInput = true;
                needAction = true;
                while (needInput) { yield return new WaitForEndOfFrame(); } //Blocking until we have input
                attackId = FindAttack(id, input);
            
                orders.ships[id].ShowTargetChoice();

                if (displayTutorial) { 
                    orders.ships[id].ShowTargetTutorial(); 
                    displayTutorial = false;
                }

                input = -1;
                while (!ShipIsAlive(input, attackId)) {
                    needInput = true;
                    needAction = false;
                    while (needInput) { yield return new WaitForEndOfFrame(); } //Blocking until we have input
                }
                targetId = input;
            }
            else {
                attackId = CommandQueue.Command.Hold; //Hold if we Heavy attacked last turn
            }

            orders.ships[id].HideAll();
            orders.IssueCommand(id, attackId, targetId); //Give the order
            prevInputs[id] = attackId; //Store the last attack 
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

    CommandQueue.Command FindAttack(int shipId, int input) {
        //if(input == 1) Debug.Log(shipId + " " + orders.ships[ship].ship.actions[input]);
        return orders.ships[shipId].shipData.actions[input];
    }
}
