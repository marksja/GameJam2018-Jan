using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserAnimator : MonoBehaviour {
    public float widthScale = 1f;
    public float xShake = 0f;
    public float yShake = 0f;
    public float fireTime = 1f;
    public AnimationCurve widthCurve;
    public AnimationCurve opacityCurve;
    public Vector3 start;
	// Use this for initialization
	void Start () {
        Color c = this.GetComponent<SpriteRenderer>().material.color;
        this.GetComponent<SpriteRenderer>().material.color = new Color(c.r, c.g, c.b, 0);

    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void AnimateLaser() {
        StartCoroutine(fireLaser());
    }

    IEnumerator fireLaser() {
        start = this.transform.position;
        float time = 0;
        Color c = this.GetComponent<SpriteRenderer>().material.color;
        // Handle animation curve interpolation
        while (time < fireTime) {

            this.transform.localScale = new Vector2(1, widthScale*Mathf.LerpUnclamped(0,1, widthCurve.Evaluate(time / fireTime)));
            
            this.GetComponent<SpriteRenderer>().material.color = new Color(c.r, c.g, c.b, 1);
            this.GetComponent<SpriteRenderer>().material.color = new Color(c.r,c.g,c.b, Mathf.LerpUnclamped(0,1,opacityCurve.Evaluate(time / fireTime)));
            this.transform.position = new Vector3(start.x + Random.Range(-xShake, xShake), start.y + Random.Range(-yShake, yShake), start.z);
            yield return new WaitForEndOfFrame();
            this.transform.position = start;
            time += Time.deltaTime;
        }
        c = this.GetComponent<SpriteRenderer>().material.color;
        this.GetComponent<SpriteRenderer>().material.color = new Color(c.r, c.g, c.b, 0);

    }
}
