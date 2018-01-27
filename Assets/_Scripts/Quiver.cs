using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quiver : MonoBehaviour {

    [Header ("Arrows in your quiver")]
    public GameObject letterArrow;
    public GameObject[] arrows;

    private bool letter;
    private int curr_index;
    private GameObject equippedArrow;
    private bool pulled = false;
    private GameObject loadedArrow;

    [Header("FMOD Event Calls")]
    [FMODUnity.EventRef]
    public string bow_draw = "event:/SFX/Player/Bow_Draw";
    public string bow_shot = "event:/SFX/Player/Bow_Shot";

    void Start () {
        letter = false;
        curr_index = 0;
        equippedArrow = arrows[0]; //Basic arrow 
	}
	
	void Update () {
        //If we right click, switch from basic to letter arrow and back
        if (Input.GetMouseButtonDown(1)) {
            if (!letter)
                equippedArrow = letterArrow;
            else
                equippedArrow = arrows[curr_index];
        }

        //Scroll left and right through special
        if (Input.GetKey(KeyCode.E)) {
            curr_index = (curr_index + 1) % arrows.Length;
            equippedArrow = arrows[curr_index];
        }
        else if (Input.GetKey(KeyCode.Q)) {
            curr_index = (curr_index - 1) % arrows.Length;
            equippedArrow = arrows[curr_index];
        }

        // Handle arrow firing
        if (Input.GetMouseButtonDown(0) && !pulled) {
            pulled = true;
            LoadArrow();
        }

        if(Input.GetMouseButtonUp(0) && pulled) {
            pulled = false;
            FireArrow();
        }
	}

    void LoadArrow() {
        // Check no arrow loaded
        if (loadedArrow == null) {
            loadedArrow = Instantiate<GameObject>(equippedArrow, this.transform.position, Quaternion.identity);
            FMODUnity.RuntimeManager.PlayOneShot(bow_draw, GetComponent<Rigidbody>().position);
        }

    }

    void FireArrow() {
        if (loadedArrow != null) {
            loadedArrow.GetComponent<Arrow>().Fire();
            FMODUnity.RuntimeManager.PlayOneShot(bow_shot, GetComponent<Rigidbody>().position);
            // Lose arrow reference
            loadedArrow = null;
        }   

    }
}
