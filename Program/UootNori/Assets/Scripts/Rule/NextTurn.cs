using UnityEngine;
using System.Collections;
using UootNori;
using FlowContainer;
using PatternSystem;

public class NextTurn : Attribute {

    Rotation _cameraRot;
    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (_isDone)
            return;

        if(_cameraRot != null)
        {
            _cameraRot.Run();

            if (_cameraRot.IsDone)
            {
                _isDone = true;
                transform.parent.GetComponent<Attribute>().ReturnActive = "UootThrow";
                GameData.NextTurn();
            }
        }
	}

    void OnEnable()
    {   
        GameObject camera = GameObject.Find("Field_Camera");
        _cameraRot = new Rotation(camera, new Vector3(0.0f, 0.0f, 180.0f), 0.45f,Physical.Type.RELATIVE);
    }

    
}
