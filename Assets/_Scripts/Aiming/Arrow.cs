using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class has a rigidbody 2D and defines it's launch trajectory,
// impact condition, and passive effect
public class Arrow : MonoBehaviour {

    // Public members
    public float speed;
    public int numBounces = 0;
    public float angle { get; set; }
    public float angleOffsetDeg = 0f;
    public GameObject body;
    // Private members
    private bool stuck = false;
    private bool fired = false;
    Rigidbody2D rb;
    RotateToMouse rtm;

	// Use this for initialization
	void Start () {
        rb = this.GetComponent<Rigidbody2D>();
        rtm = this.GetComponent<RotateToMouse>();
	}
	
	// Update is called once per frame
	void Update () {
        
        angle = rtm.angle;
		if(fired && !stuck) { // If fired then disable rotate to mouse and rotate by speed
            rtm.enabled = false;
            SetSpeedByAngle();
            this.GetComponent<Rigidbody2D>().transform.parent = null;
            //SetDirectionBySpeed();
            stuck = true;
        }
        if (!fired) { // If not fired then 
            // Use rotate to mouse to aim
            this.GetComponent<Rigidbody2D>().transform.parent = body.GetComponent<Rigidbody2D>().transform;
        }
	}

    void LateUpdate() {
        if (!fired) { // If not fired then 
            // Use rotate to mouse to aim
            this.GetComponent<Rigidbody2D>().transform.position = body.GetComponent<Rigidbody2D>().transform.position;
        }
    }

    // Call fire to launch arrow at speed
    public void Fire() { // Shoot and set angle
        fired = true;
    }

    void SetSpeedByAngle() {
        rb.velocity = new Vector2(-speed*Mathf.Cos(angle*Mathf.Deg2Rad), -speed*Mathf.Sin(angle * Mathf.Deg2Rad));
        rb.velocity += body.GetComponent<Rigidbody2D>().velocity;
    }

    void SetDirectionBySpeed() {
        //angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x);
        rb.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, (float) angle + angleOffsetDeg));
    }

}
