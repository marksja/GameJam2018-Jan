using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPanel : MonoBehaviour {

    public float spawnTime;
    public GameObject[] panels;

	// Use this for initialization
	void Start () {
        Clone();
        StartCoroutine(GeneratePanel());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator GeneratePanel() {
        yield return new WaitForSeconds(spawnTime);
        Debug.Log("Brap");
        Clone();
        StartCoroutine(GeneratePanel());
    }

    void Clone() {
        GameObject jerry = Instantiate<GameObject>(panels[Mathf.RoundToInt((float) panels.Length)-1 ], this.transform.position, Quaternion.identity);
        jerry.transform.rotation = this.transform.rotation;
    }
}
