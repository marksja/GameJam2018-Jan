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
		if(sixtySix.order == Command.Shield){
			//Debug.Log("Issued shield command");
		}
		sixtySix.priority = (sixtySix.order == Command.Shield) ? 0 : 1;
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
			if(!ships[i].alive){
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

    IEnumerator ExecuteCo(Order sixtySix) {
        //Yes, My Lord
        int shipNum = sixtySix.shipID;
        Ship Ship = ships[shipNum];
        Ship targetShip;
        yield return new WaitForSeconds(1f);
        switch (sixtySix.order) {
            case Command.Light:
                //damage = 50
                targetShip = opponentQueue.ships[sixtySix.target];
                if (!targetShip.alive) { yield return null; }
                //Deal damage to the target ship
                targetShip.TakeDamage(Ship.shipData.lightDamage);
                if (targetShip.gameObject.activeInHierarchy)
                    Ship.LaserCaller(targetShip.transform.position);

                break;
            case Command.Shield:
                //Add temp hp = 150
                targetShip = ships[sixtySix.target];
                if (!targetShip.alive) { yield return null; }
                //Apply a temporary hp pool
                targetShip.AddShield(Ship.shipData.shieldHealth);

                break;
            case Command.Heavy:
                //damage = 150
                targetShip = opponentQueue.ships[sixtySix.target];
                if (!targetShip.alive) { yield return null; }
                //Deal damage to the target ship
                targetShip.TakeDamage(Ship.shipData.heavyDamage);
                //Call two lasers for visual P L A C E H O L D E R
                if (targetShip.gameObject.activeInHierarchy) {
                    Ship.LaserCaller(targetShip.transform.position);
                    Ship.LaserCaller(targetShip.transform.position);
                }

                break;
            case Command.Hold:
                //Do nothing

                break;
            default:
                break;
        }
    }
	public void ExecuteOrder(Order sixtySix){
        StartCoroutine("ExecuteCo", sixtySix);
	}

	public void ApplyAllDamages(){
		foreach(Ship s in ships){
			s.ApplyDamage();
		}
	}
}
