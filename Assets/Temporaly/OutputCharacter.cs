using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutputCharacter : MonoBehaviour {

    private Text texto1;

    // Use this for initialization
    void Start () {
        texto1 = GetComponent<Text>();
        texto1.text = "AAAAAAA";
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
