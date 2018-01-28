using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TurnManager : MonoBehaviour {

    [System.Serializable]
    public class IntEvent : UnityEvent<int>
    {

    }

	struct QueueOrdersForTurn {
		public CommandQueue queue;
		public List<CommandQueue.Order> orders;
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
		StartCoroutine(GrossCoroutine());
    }

	IEnumerator GrossCoroutine(){
		List<CommandQueue.Order> orders = new List<CommandQueue.Order>();

        onTurnEnd.Invoke(turnNum);

        //Get all orders for this current turn;

		// TODO: Not this.
		List<QueueOrdersForTurn> incomingCommands = new List<QueueOrdersForTurn>();
		foreach (CommandQueue q in registeredCommandQueues) {
			QueueOrdersForTurn queueOrders = new QueueOrdersForTurn();
			queueOrders.queue = q;
			queueOrders.orders = q.SendCommands(turnNum);
			incomingCommands.Add(queueOrders);
		}
		
		for (int currPriority = 0; currPriority < 2; ++currPriority) {
			foreach (QueueOrdersForTurn queueOrders in incomingCommands) {
				foreach (CommandQueue.Order o in queueOrders.orders) {
					if (o.priority == currPriority) { 
						queueOrders.queue.ExecuteOrder(o);
						yield return new WaitForSeconds(0.4f);
						orders.Add(o);
					}
				}
			}
		}
        
        foreach (CommandQueue q in registeredCommandQueues){
			q.ApplyAllDamages();
		}

        turnNum++;
        playersWithTurnsCompleted = 0;
        onTurnStart.Invoke(turnNum);
	}
}
