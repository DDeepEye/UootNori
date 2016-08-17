using UnityEngine;
using System.Collections;
using UootNori;
using FlowContainer;
using PatternSystem;

public class NextTurn : Attribute {

    Rotation _cameraRot;
    Rotation _uiCameraRot;
    PatternSystem.Move _player1Mover;
    PatternSystem.Move _player2Mover;

    GameObject [] _players = new GameObject[(int)PLAYER_KIND.MAX];

    void Awake()
    {
        GameObject uiroot = GameObject.Find("UI Root");
        Transform gp = uiroot.transform.FindChild("Size").FindChild("GamePlay");

        if (_players[0] == null)
        {
            _players[0] = gp.FindChild("Play01").gameObject;
            _players[0].transform.FindChild("Select_P").gameObject.SetActive(true);
        }

        if (_players[1] == null)
        {
            _players[1] = gp.FindChild("Play02").gameObject;
            _players[1].transform.FindChild("Select_P").gameObject.SetActive(false);
        }

    }
    
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
            _uiCameraRot.Run();
            _player1Mover.Run();
            _player2Mover.Run();

            if (_cameraRot.IsDone)
            {
                _isDone = true;
                transform.parent.GetComponent<Attribute>().ReturnActive = "UootThrow";
                _players[(int)GameData.CurTurn].transform.FindChild("Select_P").gameObject.SetActive(false);
                GameData.NextTurn();
                _players[(int)GameData.CurTurn].transform.FindChild("Select_P").gameObject.SetActive(true);
            }
        }
	}

    void OnEnable()
    {   
        GameObject camera = GameObject.Find("Field_Camera");
        _cameraRot = new Rotation(camera, new Vector3(0.0f, 0.0f, 180.0f), 0.45f,Physical.Type.RELATIVE);
        camera = GameObject.Find("UI Root").transform.FindChild("Camera").gameObject;
        _uiCameraRot = new Rotation(camera, new Vector3(0.0f, 0.0f, 180.0f), 0.45f,Physical.Type.RELATIVE);

        Vector3 moveOffset = _players[1].transform.position - _players[0].transform.position;
        _player1Mover = new Move(_players[0], moveOffset, 0.45f);
        _player2Mover = new Move(_players[1], -moveOffset, 0.45f);
    }
}
