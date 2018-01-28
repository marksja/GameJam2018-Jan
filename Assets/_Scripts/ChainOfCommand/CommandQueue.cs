using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandQueue : MonoBehaviour {

	public Ship[] ships;

	public enum Command {Light, Heavy, Shield, Hold};

	public struct Order{
		public int shipID;
		public Command order;
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
		Order sixtySix = new Order();
		sixtySix.shipID = shipID;
		sixtySix.order = command;
		sixtySix.target = targetID;
		sixtySix.turnIssued = TurnManager.Instance.turnNum;
		sixtySix.turnExecuted = sixtySix.turnIssued + GetTurnDelay();
		sixtySix.priority = (sixtySix.order == Command.Shield) ? 0 : 1;
		orderQueue.Enqueue(sixtySix);
	}

	public int GetTurnDelay(){
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
	public void ExecuteOrder(Order sixtySix){
        //Yes, My Lord
        int shipNum = sixtySix.shipID;
        Ship currentShip = ships[shipNum];
        Ship targetShip;
        switch (sixtySix.order) {
            case Command.Light:
                //damage = 50
                targetShip = opponentQueue.ships[sixtySix.target];
                if (!targetShip.alive) { return; }
                //Deal damage to the target ship
                targetShip.TakeDamage(currentShip.shipData.lightDamage);
                if (currentShip.gameObject.activeInHierarchy) {
                    currentShip.LaserCaller(targetShip.transform.position);
				}

                break;
            case Command.Shield:
                //Add temp hp = 150
                targetShip = ships[sixtySix.target];
                if (!targetShip.alive) { return; }
                //Apply a temporary hp pool
                targetShip.AddShield(currentShip.shipData.shieldHealth);

                break;
            case Command.Heavy:
                //damage = 150
                targetShip = opponentQueue.ships[sixtySix.target];
                if (!targetShip.alive) { return; }
                //Deal damage to the target ship
                targetShip.TakeDamage(currentShip.shipData.heavyDamage);
                //Call two lasers for visual P L A C E H O L D E R
                if (currentShip.gameObject.activeInHierarchy) {
                    currentShip.LaserCaller(targetShip.transform.position);
                    currentShip.LaserCaller(targetShip.transform.position);
                }

                break;
            case Command.Hold:
                //Do nothing

                break;
            default:
                break;
        }
	}

	public void ApplyAllDamages(){
		foreach(Ship s in ships){
			s.ApplyDamage();
		}
	}
}
