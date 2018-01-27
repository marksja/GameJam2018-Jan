using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyMovement : MonoBehaviour {

    Rigidbody2D rb;
    public float jumpSpeed;
    public bool canJump = true;
    public bool lockHorizontal = false;
	// Use this for initialization
	void Start () {
        rb = this.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space) && canJump) {
            rb.velocity += new Vector2(0, jumpSpeed);
            canJump = false;
        }
        if (lockHorizontal) {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
	}

    void OnCollisionEnter2D(Collision2D col) {
        canJump = true;
    }


}
