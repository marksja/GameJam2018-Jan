using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavController : MonoBehaviour {

	public Dictionary<string, Node> nodeDictionary;

	// Use this for initialization
	void Start () {
		//Get all the nodes in the dictionary
		Node[] n = GetComponentsInChildren<Node>();
		nodeDictionary = new Dictionary<string, Node>();
		foreach(Node node in n){
			nodeDictionary.Add(node.uniqname, node);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public Node FindNearestNode(Vector3 target){
		//Iterate through all nodes and find the smallest distance
		//Return that node
		Node nearest = null;
		float closestDistance = Mathf.Infinity;
		
		foreach(Node node in nodeDictionary.Values){
			float dist = Vector3.Distance(node.transform.position, target);
			if(dist < closestDistance){
				closestDistance = dist;
				nearest = node;
			}
		}

		return nearest;
	}
	
	public List<Vector3> FindPath(Vector3 start, Vector3 target){
		SortedList openNodes = new SortedList(); 
		List<string> closedNodes = new List<string>();
		Node startNode = FindNearestNode(start);
		Node targetNode = FindNearestNode(target);

		if(startNode == null || targetNode == null){
			Debug.LogWarning("Unable to path: Could not find the starting or target node");
			return null;
		}

		string finalPath = "";
		
		float pointValue = Heuristic(startNode.transform.position, targetNode.transform.position);
		string pathID = startNode.uniqname;
		Node currentNode;
		bool foundPath = false;
		openNodes.Add(pointValue, pathID);
		while(!foundPath){
			pathID = (string)openNodes.GetByIndex(0);
			currentNode = nodeDictionary[GetEdgeNode(pathID)];
			if(currentNode == null){
				Debug.Log("Uh Oh, found an invalid node somehow");
				return null;
			}
			closedNodes.Add(currentNode.uniqname);

			foreach(GameObject g in currentNode.nodes){
				Node node = g.GetComponent<Node>();
				float distanceFromStart = (float)openNodes.GetKey(0) - Heuristic(currentNode.transform.position, targetNode.transform.position);
				if(node.uniqname == targetNode.uniqname){
					finalPath = node.uniqname + "," + pathID;
					foundPath = true;
					break;
				}
				if(!closedNodes.Contains(node.uniqname)){
					if(currentNode.gameObject.name == "Node (3)"){
						Debug.Log(node.gameObject.name);
					}
					float distanceFromLast = Vector3.Distance(g.transform.position, currentNode.transform.position);
					float hueristic = Heuristic(g.transform.position, targetNode.transform.position);
					Debug.Log(distanceFromStart + hueristic + distanceFromLast);
					openNodes.Add(hueristic + distanceFromStart + distanceFromLast, node.uniqname + "," + pathID);
				}
			}

			openNodes.RemoveAt(0);
		}

		List<Vector3> returnList = new List<Vector3>();

		returnList.Add(start);
		string[] path = GetStringList(finalPath);
		for(int i = path.Length - 1; i >= 0; i--){
			returnList.Add(nodeDictionary[path[i]].transform.position);
		}

		return returnList;
	}

	[ContextMenu("Let's try this shitty function!")]
	public void CallFromMenu(){
		List<Vector3> path = FindPath(Vector3.one * 10, Vector3.zero);
		foreach(Vector3 v in path){
			Debug.Log("Node: " + v);
		}
	}

	public float Heuristic(Vector3 current, Vector3 target){
		return Vector3.Distance(current, target);
	}

	public string[] GetStringList(string list){
		return list.Split(',');
	}

	public string GetEdgeNode(string List){
		return List.Split(',')[0];

	}
}
