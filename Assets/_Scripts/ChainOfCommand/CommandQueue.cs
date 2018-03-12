using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandQueue : MonoBehaviour {
	public enum Command { LIGHT, HEAVY, SHIELD, HOLD, SWAP };

	public struct Order{
		public Ship ship;
		public Command command;
		public int target;
		public int turnIssued;
		public int turnExecuted;
		public int priority;
	}
    public Ship[] ships;

    public Queue<Order> orderQueue;

    public CommandQueue opponentQueue;

    private int kSwapDamage = 50;

    public static int kLowestPriority = 2;

    // Use this for initialization
    void Start () {
		orderQueue = new Queue<Order>();
        TurnManager.Instance.RegisterCommandQueue(this);
	}
	
	// Update is called once per frame
	void Update () {}

	public void IssueCommand(Ship ship, Command command, int targetID){
		Order order = new Order();
		order.ship = ship;
		order.command = command;
		order.target = targetID;
		order.turnIssued = TurnManager.Instance.turnNum;
		order.turnExecuted = order.turnIssued + GetTurnDelay();

        switch (command) {
            case Command.SWAP: 
                order.priority = 0;
                break;
            case Command.SHIELD: 
                order.priority = 1;
                break;
            default:
                order.priority = kLowestPriority;
                break;
        }

		orderQueue.Enqueue(order);
	}

	static public int GetTurnDelay(){
		return 1;
	}

	public List<Order> SendCommands(int turnNum){
		List<Order> ordersForThisTurn = new List<Order>();
		if(orderQueue.Count == 0){
			return null;
		}

		// JF: Pop off all expired commands
		while (orderQueue.Peek().turnExecuted < turnNum) {
			orderQueue.Dequeue();
		}

		for(int i = 0; i < ships.Length; ++i){
			if(!ships[i].alive){
				continue;
			}
			Order nextOrder = orderQueue.Peek();
			if (nextOrder.turnExecuted > turnNum) {
				// Not yet time to execute this order; leave it in queue
				continue;
			}
			if (nextOrder.turnExecuted == turnNum) {
				// Time to execute; add to list of orders
				ordersForThisTurn.Add(nextOrder);
			}
			orderQueue.Dequeue();
		}
		return ordersForThisTurn;
	}

	public void ExecuteOrder(Order order){
        Ship currentShip = order.ship;

        if (!order.ship.alive) { return; }
        Ship targetShip;
        switch (order.command) {
            case Command.LIGHT:
                //damage = 50
                targetShip = opponentQueue.ships[order.target];
                currentShip.LaserCaller(targetShip.transform.position);
                if (targetShip.alive) {
                    //Deal damage to the target ship
                    targetShip.TakeDamage(currentShip.shipData.lightDamage);
                }

                break;
            case Command.SHIELD:
                //Add temp hp = 150
                targetShip = ships[order.target];
                if (!targetShip.alive) { break; }
                //Apply a temporary hp pool
                targetShip.AddShield(currentShip.shipData.shieldHealth);

                break;
            case Command.HEAVY:
                //damage = 150
                targetShip = opponentQueue.ships[order.target];
                currentShip.HeavyLaserCaller(targetShip.transform.position);
                if (targetShip.alive) {
                    //Deal damage to the target ship
                    targetShip.TakeDamage(currentShip.shipData.heavyDamage);
                }

                break;
            case Command.SWAP:
                targetShip = ships[order.target];
                if (targetShip == currentShip) {
                    // Cannot swap to own position; does nothing
                    break;
                }
                int shipNum = GetShipNum(currentShip);
                ships[order.target] = currentShip;
                ships[shipNum] = targetShip;

                // Play warpswap animation
                currentShip.WarpIn();
                if (targetShip.alive) { targetShip.WarpIn(); }

                // Swap ship number sprites
                var number_sprite_temp = 
                    currentShip.transform.Find("Sprite/Number").GetComponent<SpriteRenderer>().sprite;
                currentShip.transform.Find("Sprite/Number").GetComponent<SpriteRenderer>().sprite =
                    targetShip.transform.Find("Sprite/Number").GetComponent<SpriteRenderer>().sprite;
                targetShip.transform.Find("Sprite/Number").GetComponent<SpriteRenderer>().sprite = 
                    number_sprite_temp;

                // Swap sprite y locations as well
                var temp = currentShip.transform.position;
                currentShip.transform.position = new Vector3(temp.x, targetShip.transform.position.y, 0);
                targetShip.transform.position = new Vector3(targetShip.transform.position.x, temp.y, 0);

                // emergency warp swapping deals damage to each ship
                currentShip.TakeDamage(kSwapDamage);
                if (targetShip.alive) { targetShip.TakeDamage(kSwapDamage); }

                break;
            case Command.HOLD:
                //Do nothing

                break;
            default:
                break;
        }
	}

	public void ApplyAllDamages(){
		foreach(Ship s in ships){
			s.ApplyDamage();
            s.TurnOffShieldInSeconds(.5f);
		}
	}

    // @return index of ship s in allied array   
    // @error: return -1
    private int GetShipNum(Ship ship_in) {
        for (int i = 0; i < ships.Length; ++i) {
            if (ship_in == ships[i]) { return i; }
        }

        return -1;
    }
}
