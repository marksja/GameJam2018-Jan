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
    private CommandQueue.Command[] prevprevInputs;

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

            if (prevInputs[id] != CommandQueue.Command.Heavy) {
                
                input = 4;
                while (input > 2) {
                    needInput = true;
                    needAction = true;
                    while (needInput) { yield return new WaitForEndOfFrame(); } //Blocking until we have input
                    if (input == 3 && id > 0) { orders.ships[id].HideAll(); id -= 1; orders.ships[id].ShowTargetChoice(); prevInputs = prevprevInputs; goto Target; }
                }

                attackId = FindAttack(id, input);
            
                orders.ships[id].ShowTargetChoice();

                if (displayTutorial) { 
                    orders.ships[id].ShowTargetTutorial(); 
                    displayTutorial = false;
                }
                Target:
                input = -1;
                while (!ShipIsAlive(input)) {
                    needInput = true;
                    needAction = false;
                    while (needInput) { yield return new WaitForEndOfFrame(); } //Blocking until we have input
                    if (input == 3) { attackId = CommandQueue.Command.Hold; orders.ships[id].HideAll(); goto Attack; } //Go back
                }
                
                targetId = input;
            }
            else {
                attackId = CommandQueue.Command.Hold; //Hold if we Heavy attacked last turn
            }

            orders.ships[id].HideAll();
            FancyTuple order = new FancyTuple(id, attackId, targetId);
            ordersForThisTurn[id] = order;
            prevInputs[id] = attackId; //Store the last attack
            prevprevInputs[id] = prevInputs[id];
        }

        for(int i = 0; i < 3; i++) {
            int shipId = ordersForThisTurn[i].me;
            attackId = ordersForThisTurn[i].attac;
            targetId = ordersForThisTurn[i].target;
            if(orders.ships[shipId].alive)
                orders.IssueCommand(shipId, attackId, targetId); //Give the order
        }
        TurnManager.Instance.TurnComplete();
        yield return new WaitForSeconds(0);
    }

    bool ShipIsAlive(int targetShip) {
        if (targetShip < 0 || targetShip > 2) return false;
        return orders.ships[targetShip].alive;
    }

    CommandQueue.Command FindAttack(int ship, int input) {
        //if(input == 1) Debug.Log(ship + " " + orders.ships[ship].ship.actions[input]);
        return orders.ships[ship].shipData.actions[input];
    }
}
