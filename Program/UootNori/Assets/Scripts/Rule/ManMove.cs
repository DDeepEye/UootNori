using UnityEngine;
using System.Collections;
using FlowContainer;

public class ManMove : Attribute {
    float _curTime = 0.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (_isDone)
            return;
        
        if (_curTime < 3.0f)
        {
            _curTime += Time.deltaTime;
        }
        else
        {
            _curTime = 0.0f;
            _isDone = true;
            transform.parent.GetComponent<Attribute>().ReturnActive = "UootThrow";
        }
	}
}
