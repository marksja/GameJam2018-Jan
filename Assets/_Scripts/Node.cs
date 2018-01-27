using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

    //Reference from A* Algorithm
    public float g;
    public float h;
    public float f;

    //Nearby nodes
    public GameObject[] nodes;

    public string uniqname;

    void Start() {
        uniqname = this.transform.GetHashCode().ToString();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, .5f);

        bool mutual = false;
        foreach(GameObject g in nodes){
            Node n = g.GetComponent<Node>();
            foreach(GameObject otherNode in n.nodes){
                if(otherNode == this.gameObject){
                    mutual = true;
                }
            }
            Gizmos.color = (mutual) ? Color.blue : Color.red;
            Gizmos.DrawLine(this.transform.position, g.transform.position);
        }
    }
}
