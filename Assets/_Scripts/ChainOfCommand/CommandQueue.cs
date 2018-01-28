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
		orderQueue.Enqueue(sixtySix);
	}

	public int GetTurnDelay(){
		return 1;
	}

	public Order[] SendCommands(int turnNum){
		Order[] ordersForThisTurn = new Order[ships.Length];
		if(orderQueue.Count == 0){
			return null;
		}
		for(int i = 0; i < ships.Length; ++i){
			if(!ships[i].gameObject.activeInHierarchy){
				continue;
			}
			Order sixtySix = orderQueue.Peek();
			if(sixtySix.turnExecuted != turnNum){
				//This order is invalid, make it so the turnManager can't execute it!
				sixtySix.shipID = -1;
			}
			else{
				orderQueue.Dequeue();
			}
			ordersForThisTurn[i] = sixtySix;
		}
		return ordersForThisTurn;
	}

	public void ExecuteOrder(Order sixtySix){
		//Yes, My Lord
		int shipNum = sixtySix.shipID;
		Ship Ship = ships[shipNum];
		Ship targetShip;
        Debug.Log(sixtySix.shipID);
		switch(sixtySix.order){
			case Command.Light:
				//damage = 50
				targetShip = opponentQueue.ships[sixtySix.target];
				//Deal damage to the target ship
				targetShip.TakeDamage(Ship.ship.lightDamage);

				break;
			case Command.Shield:
				//Add temp hp = 150
				targetShip = ships[sixtySix.target];
				//Apply a temporary hp pool
				targetShip.AddShield(Ship.ship.shieldHealth);

				break;
			case Command.Heavy:
				//damage = 150
				targetShip = opponentQueue.ships[sixtySix.target];
				//Deal damage to the target ship
				targetShip.TakeDamage(Ship.ship.heavyDamage);

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
