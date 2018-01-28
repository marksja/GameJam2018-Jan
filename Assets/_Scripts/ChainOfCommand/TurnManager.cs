using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TurnManager : MonoBehaviour {

    [System.Serializable]
    public class IntEvent : UnityEvent<int>
    {

    }

	public IntEvent onTurnStart;
	public IntEvent onTurnEnd;

	public int turnNum;
	int playersWithTurnsCompleted;

	public static TurnManager Instance;

	public List<CommandQueue> registeredCommandQueues;

	// Use this for initialization
	void Awake() {
		Instance = this;
		turnNum = 0;
        playersWithTurnsCompleted = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void RegisterCommandQueue(CommandQueue queue){
		registeredCommandQueues.Add(queue);
	}

	public void TurnComplete(){
		playersWithTurnsCompleted++;
		if(playersWithTurnsCompleted >= registeredCommandQueues.Count){
			ResolveTurn();
		}
	}

	public void ResolveTurn(){
		List<CommandQueue.Order> orders = new List<CommandQueue.Order>();
		
		//Get all orders for this current turn;
		foreach(CommandQueue q in registeredCommandQueues){
			CommandQueue.Order[] ordersFromSingleQueue = q.SendCommands(turnNum);
			foreach(CommandQueue.Order o in ordersFromSingleQueue){
				if(o.shipID == -1) continue;
				
				q.ExecuteOrder(o);
				orders.Add(o);
			}
		}

		foreach(CommandQueue q in registeredCommandQueues){
			q.ApplyAllDamages();
		}

		//onTurnEnd.Invoke(turnNum);
		turnNum ++;
        playersWithTurnsCompleted = 0;
        onTurnStart.Invoke(turnNum);

        foreach (CommandQueue q in registeredCommandQueues) {
            q.ApplyAllDamages();
        }
    }
}
