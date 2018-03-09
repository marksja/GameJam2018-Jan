using System.Collections;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [Range(0, 1)] //There's two players, its hard coded, go cry about it
    public int playerNumber;
    public int numberOfShips = 3;
    public CommandQueue orders;

    private bool needInput = false;
    private bool destroyerOnCooldown = false;
    private int input;
    private CommandQueue.Command[] prevInputs;
    private CommandQueue.Command[] prevprevInputs;

    // JF: Hacky way to control when tutorials appear
    private bool displayTutorial = true;

    class CommandData {
        public Ship me;
        public CommandQueue.Command action;
        public int target;

        public CommandData(Ship x, CommandQueue.Command y, int z) {
            me = x;
            action = y;
            target = z;
        }
    };

    private CommandData[] ordersForThisTurn;

    // Use this for initialization
    void Start() {
        ordersForThisTurn = new CommandData[numberOfShips];
        prevInputs = new CommandQueue.Command[numberOfShips];
        prevprevInputs = new CommandQueue.Command[numberOfShips];
        for (int i = 0; i < numberOfShips; i++) {
            prevInputs[i] = CommandQueue.Command.HOLD;
            prevprevInputs[i] = CommandQueue.Command.HOLD;
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
            else if (playerNumber == 0 ? Input.GetKeyDown(KeyCode.R) : Input.GetKeyDown(KeyCode.U)) {
                //Undo Button
                input = 3;
                needInput = false;
            }
        }
    }

    public void StartTurn(int turnStart) { StartCoroutine("ChooseCommands"); }

    IEnumerator ChooseCommands() {
        CommandQueue.Command actionId = CommandQueue.Command.HOLD;
        int targetId = -1;

        //We go through all the ships we have and choose the ship Id, the attack the choose, and their target
        for (int idx = 0; idx < numberOfShips; idx++) {
            if (!orders.ships[idx].alive) { continue; }

            Attack:
            orders.ships[idx].ShowAttackTypeChoice();
            
            // Display tutorial blurbs first time that commands are requested
            if (displayTutorial) { orders.ships[idx].ShowTypeTutorial(); }

            if (destroyerOnCooldown && orders.ships[idx].shipData.type == ShipData.Type.DESTROYER) {
                actionId = CommandQueue.Command.HOLD;
            }
            else {
                input = 4;
                while (input > 2) {
                    needInput = true;
                    while (needInput) { yield return null; } //Blocking until we have input
                    if (input == 3 && idx > 0) {
                        // undo cascade
                        orders.ships[idx].HideAll();
                        idx -= 1; // last ship
                        if(destroyerOnCooldown && 
                            orders.ships[idx].shipData.type == ShipData.Type.DESTROYER) {
                            idx -= 1;
                        }
                        orders.ships[idx].ShowTargetChoice();
                        prevInputs = prevprevInputs;
                        goto Target;
                    } // Go Back
                }

                actionId = GetAction(idx, input);
            
                orders.ships[idx].ShowTargetChoice();

                if (displayTutorial) { orders.ships[idx].ShowTargetTutorial(); }

                Target:
                input = -1;
                needInput = true;
                while (needInput) { yield return null; } //Blocking until we have input
                if (input == 3) { 
                    actionId = CommandQueue.Command.HOLD;
                    orders.ships[idx].HideAll();
                    goto Attack;
                } // Go back
                
                targetId = input;
            }

            orders.ships[idx].HideAll();
            CommandData order = new CommandData(orders.ships[idx], actionId, targetId);
            ordersForThisTurn[idx] = order;
            prevprevInputs[idx] = prevInputs[idx];
            prevInputs[idx] = actionId; //Store the last attack
        }

        // Command tutorials are only displayed the first time
        displayTutorial = false;

        for(int i = 0; i < ordersForThisTurn.Length; i++) {
            Ship currentShip = ordersForThisTurn[i].me;

            actionId = ordersForThisTurn[i].action;
            targetId = ordersForThisTurn[i].target;
            if (currentShip.alive) {
                if (actionId == CommandQueue.Command.HEAVY) { destroyerOnCooldown = true; }
                if (actionId == CommandQueue.Command.HOLD && 
                    currentShip.shipData.type == ShipData.Type.DESTROYER) { destroyerOnCooldown = false; }
                orders.IssueCommand(currentShip, actionId, targetId); //Give the order
            }
        }
        TurnManager.Instance.TurnComplete();
        yield return null;
    }

    CommandQueue.Command GetAction(int ship, int input) {
        return orders.ships[ship].shipData.actions[input];
    }
}
