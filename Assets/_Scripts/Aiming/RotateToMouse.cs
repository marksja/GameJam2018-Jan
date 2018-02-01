using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToMouse : MonoBehaviour {

    public float angleOffsetDeg;
    public float angle { get; set; }

	// Use this for initialization
	void Start () {
        // Init
        SetAngleToMouse();

    }
	
	// Update is called once per frame
	void Update () {
        SetAngleToMouse();
    }

    void SetAngleToMouse() {
        // Get mouse position from screen to the viewport
        Vector2 mouseScreenPos = (Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition);
        // Get the player position from the world space to the viewport
        Vector2 aimScreenPos = (Vector2)Camera.main.WorldToViewportPoint(transform.position);
        // Calculate the angle bewteen 
        angle = AngleBetweenTwoPoints(aimScreenPos, mouseScreenPos);
        // Roate the object
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle + angleOffsetDeg));
    }

    float AngleBetweenTwoPoints(Vector3 a, Vector3 b) {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }

}
