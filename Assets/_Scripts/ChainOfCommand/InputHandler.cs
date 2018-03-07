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

    public void StartTurn(int turnStart) {
        StartCoroutine("ChooseCommands");
    }

    IEnumerator ChooseCommands() {
        CommandQueue.Command attackId = CommandQueue.Command.HOLD;
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

            if (destroyerOnCooldown && orders.ships[id].shipData.type == ShipData.Type.DESTROYER) {
                attackId = CommandQueue.Command.HOLD;
            }
            else {
                input = 4;
                while (input > 2) {
                    needInput = true;
                    while (needInput) { yield return new WaitForEndOfFrame(); } //Blocking until we have input
                    if (input == 3 && id > 0) {
                        // undo cascade
                        orders.ships[id].HideAll();
                        id -= 1;
                        if(destroyerOnCooldown && orders.ships[id].shipData.type == ShipData.Type.DESTROYER) {
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
                    while (needInput) { yield return new WaitForEndOfFrame(); } //Blocking until we have input
                    if (input == 3) { attackId = CommandQueue.Command.HOLD;
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

        for(int i = 0; i < ordersForThisTurn.Length; i++) {
            int shipId = ordersForThisTurn[i].me;
            Ship currentShip = orders.ships[shipId];

            attackId = ordersForThisTurn[i].attac;
            targetId = ordersForThisTurn[i].target;
            if (currentShip.alive) {
                if (attackId == CommandQueue.Command.HEAVY) { destroyerOnCooldown = true; }
                if (attackId == CommandQueue.Command.HOLD && 
                    currentShip.shipData.type == ShipData.Type.DESTROYER) { destroyerOnCooldown = false; }
                orders.IssueCommand(shipId, attackId, targetId); //Give the order
            }
        }
        TurnManager.Instance.TurnComplete();
        yield return null;
    }

    bool ShipIsAlive(int targetId, CommandQueue.Command attackId) {
        if (targetId < 0) return false;
        switch (attackId) {
            case CommandQueue.Command.SHIELD: {
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
