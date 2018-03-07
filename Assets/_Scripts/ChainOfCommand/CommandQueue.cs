using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandQueue : MonoBehaviour {
	public Ship[] ships;

	public enum Command { LIGHT, HEAVY, SHIELD, HOLD, SWAP };

	public struct Order{
		public int shipID;
		public Command command;
		public int target;
		public int turnIssued;
		public int turnExecuted;
		public int priority;
	}

	public CommandQueue opponentQueue;

	public Queue<Order> orderQueue; 

	// Use this for initialization
	void Start () {
		orderQueue = new Queue<Order>();
        TurnManager.Instance.RegisterCommandQueue(this);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void IssueCommand(int shipID, Command command, int targetID){
		Order order = new Order();
		order.shipID = shipID;
		order.command = command;
		order.target = targetID;
		order.turnIssued = TurnManager.Instance.turnNum;
		order.turnExecuted = order.turnIssued + GetTurnDelay();

        switch (command) {
            case Command.SWAP: {
                order.priority = 0;
                break;
            }
            case Command.SHIELD: {
                order.priority = 1;
                break;
            }
            default: {
                order.priority = 2;
                break;
            }
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
        int shipNum = order.shipID;
        Ship currentShip = ships[shipNum];
        Ship targetShip;
        switch (order.command) {
            case Command.LIGHT:
                //damage = 50
                targetShip = opponentQueue.ships[order.target];
                if (!targetShip.alive) { return; }
                //Deal damage to the target ship
                targetShip.TakeDamage(currentShip.shipData.lightDamage);
                if (currentShip.gameObject.activeInHierarchy) {
                    currentShip.LaserCaller(targetShip.transform.position);
				}

                break;
            case Command.SHIELD:
                //Add temp hp = 150
                targetShip = ships[order.target];
                if (!targetShip.alive) { return; }
                //Apply a temporary hp pool
                targetShip.AddShield(currentShip.shipData.shieldHealth);

                break;
            case Command.HEAVY:
                //damage = 150
                targetShip = opponentQueue.ships[order.target];
                if (!targetShip.alive) { return; }
                //Deal damage to the target ship
                targetShip.TakeDamage(currentShip.shipData.heavyDamage);
                //Call two lasers for visual P L A C E H O L D E R
                if (currentShip.gameObject.activeInHierarchy) {
                    currentShip.HeavyLaserCaller(targetShip.transform.position);
                }

                break;
            case Command.SWAP:
                targetShip = ships[order.target];
                ships[order.target] = currentShip;
                ships[shipNum] = targetShip;

                // Swap sprite locations as well
                var transform_temp = currentShip.transform.position;
                currentShip.transform.position = targetShip.transform.position;
                targetShip.transform.position = transform_temp;

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
}
