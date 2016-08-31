using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UootNori;
using PatternSystem;

public class Initialize : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Screen.fullScreen = true;
        GameData.Init();
	}
	
	// Update is called once per frame
	void Update () {
	}
}
