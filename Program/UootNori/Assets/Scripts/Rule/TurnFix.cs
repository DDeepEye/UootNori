using UnityEngine;
using System.Collections;
using FlowContainer;
using UootNori;

public class TurnFix : Attribute {

	// Use this for initialization
	void Start () {
        _isDone = true;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnEnable()
    {
        GameData.TurnSave();
        _isDone = true;
    }
}
