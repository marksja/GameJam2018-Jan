using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bob : MonoBehaviour
{
    bool run = true;
    public float offset = 3f;
    public float speed = 2f;
    public float journeyLength = 4;
    public UnityEngine.AnimationCurve animCurve;
    private Vector3 start;
    private float startTime;

    void Start() {
        startTime = Time.time;
        start = this.transform.position;
        StartCoroutine("StartBobbing");
    }

    //I long for the sweet embrace of death
    IEnumerator StartBobbing() {
        while (true) {
            if (run) {
                float distCovered = (Time.time - startTime) * speed;
                float fracJourney = distCovered / journeyLength;
                float new_y = Mathf.Lerp(start.y - offset, start.y + offset, animCurve.Evaluate(fracJourney));
                Vector3 newPosition = new Vector3(start.x, new_y, start.z);
                transform.position = newPosition;
            }
        }
    }
}