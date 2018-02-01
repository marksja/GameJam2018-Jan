using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowards : MonoBehaviour {

    Rigidbody2D rb;
    public float angle;
    public float speed;
    public float aliveTime;
	// Use this for initialization
	void Start () {
        rb = this.GetComponent<Rigidbody2D>();
        StartCoroutine(AliveTime());
	}
	
	// Update is called once per frame
	void Update () {
        rb.velocity = new Vector2(speed * Mathf.Cos(angle * Mathf.Deg2Rad), speed * Mathf.Sin(angle * Mathf.Deg2Rad));
	}

    IEnumerator AliveTime() {
        yield return new WaitForSeconds(aliveTime);
        Destroy(this.gameObject);
    }
}
