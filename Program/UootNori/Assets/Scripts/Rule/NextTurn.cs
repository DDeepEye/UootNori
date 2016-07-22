using UnityEngine;
using System.Collections;
using UootNori;
using FlowContainer;

public class NextTurn : Attribute {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (_isDone)
            return;

        _isDone = true;
        transform.parent.GetComponent<Attribute>().ReturnActive = "UootThrow";
        GameData.NextTurn();
	}

    
}
