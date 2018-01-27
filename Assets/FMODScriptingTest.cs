using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FMODScriptingTest : MonoBehaviour {


[FMODUnity.EventRef]
	public string Bow_Draw =  "event:/SFX/Player/Bow_Draw";

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0))
		{
			FMODUnity.RuntimeManager.PlayOneShot(Bow_Draw, GetComponent<Rigidbody>().position);
		}
	}
}
