using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public float movementSpeed = 5f;
	public float sprintSpeed = 10f;
	public float sprintThreshold = 1f;

	public float jumpPower = 3f;
	public float jumpDuration = 2f;

	float sprintTime;

	Rigidbody rb;

	Vector3 movement;

	// Use this for initialization
	void Start () {
		sprintTime = 0;
		rb = GetComponent<Rigidbody>();
        StartCoroutine(WaitingForJump());
    }

    void Update(){
		if(Input.GetKey(KeyCode.D)){
			sprintTime += Time.deltaTime;
			if(sprintTime > sprintThreshold){
				movement = Vector3.right * sprintSpeed;
			}
			else{
				movement = Vector3.right * movementSpeed;
			}
		}
		else if(Input.GetKey(KeyCode.A)){
			sprintTime += Time.deltaTime;
			if(sprintTime > sprintThreshold){
				movement = Vector3.left * sprintSpeed;
			}
			else{
				movement = Vector3.left * movementSpeed;
			}
		}
		else{
			movement = Vector3.zero;
		}

		if(Input.GetKeyUp(KeyCode.D)){
			sprintTime = 0;
		}
		if(Input.GetKeyUp(KeyCode.A)){
			sprintTime = 0;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		rb.velocity = new Vector3(0, rb.velocity.y, 0);
		rb.velocity += movement;
	}

	IEnumerator WaitingForJump(){
		while(true){
			if(Input.GetKey(KeyCode.W)){
				StartCoroutine(Jumping());
				yield break;
			}
			yield return new WaitForEndOfFrame();
		}
	}

	IEnumerator Jumping(){
		//Debug.Log("Jumping");
		rb.AddForce(Vector3.up * jumpPower);

		yield return new WaitForSeconds(jumpDuration);

		StartCoroutine(WaitingForJump());
	}

}
