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
		StartCoroutine(GrossCoroutine());
    }

	IEnumerator GrossCoroutine(){
		List<CommandQueue.Order> orders = new List<CommandQueue.Order>();

        onTurnEnd.Invoke(turnNum);

        //Get all orders for this current turn;
        foreach (CommandQueue q in registeredCommandQueues){
			CommandQueue.Order[] ordersFromSingleQueue = q.SendCommands(turnNum);
			for(int i = 0; i < 2; ++i){
				foreach(CommandQueue.Order o in ordersFromSingleQueue){
					if(o.priority == i){ 
						if(o.shipID == -1) continue;
						q.ExecuteOrder(o);
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
