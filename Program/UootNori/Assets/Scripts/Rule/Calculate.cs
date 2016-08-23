using UnityEngine;
using System.Collections;
using FlowContainer;
using UootNori;

public class Calculate : Attribute {

    static Calculate s_instance;
    static public Calculate Instance{get{return s_instance;}}

    Calculate()
    {
        s_instance = this;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void Event(KeyEvent key)
    {

    }
}
