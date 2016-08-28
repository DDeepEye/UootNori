using UnityEngine;
using System.Collections;
using UootNori;
using FlowContainer;
using PatternSystem;

public class NextTurnCheck : Attribute {

    Rotation _cameraRot;
    Rotation _uiCameraRot;
    PatternSystem.Move _player1Mover;
    PatternSystem.Move _player2Mover;

    GameObject [] _players = new GameObject[(int)PLAYER_KIND.MAX];

    static public NextTurnCheck s_instance;
    static public NextTurnCheck Instance 
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = GameObject.Find("Flow").transform.FindChild("GameFlow").FindChild("InGame").FindChild("InGameFlow").FindChild("GamePlay").FindChild("NextTurn").GetComponent<NextTurnCheck>();
            }
            return s_instance;
        }
    }

    public void GameTurnMarking(PLAYER_KIND kind)
    {
        if (_players[0] == null)
        {
            GameObject uiroot = GameObject.Find("UI Root");
            Transform gp = uiroot.transform.FindChild("Size").FindChild("GamePlay");
            _players[0] = gp.FindChild("Play01").gameObject;
            _players[1] = gp.FindChild("Play02").gameObject;
        }

        PLAYER_KIND offMarking = (kind == PLAYER_KIND.PLAYER_1 ? PLAYER_KIND.PLAYER_2 : PLAYER_KIND.PLAYER_1);
        _players[(int)offMarking].transform.FindChild("Select_P").gameObject.SetActive(false);
        _players[(int)kind].transform.FindChild("Select_P").gameObject.SetActive(true);

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
                GameData.NextTurn();
                GameTurnMarking(GameData.CurTurn);
            }
        }
	}

    void OnEnable()
    {   
        GameObject camera = GameObject.Find("Field_Camera");
        _cameraRot = new Rotation(camera, new Vector3(0.0f, 0.0f, 180.0f), 1.2f,Physical.Type.RELATIVE);
        camera = GameObject.Find("UI Root").transform.FindChild("Camera").gameObject;
        _uiCameraRot = new Rotation(camera, new Vector3(0.0f, 0.0f, 180.0f), 1.2f,Physical.Type.RELATIVE);

        Vector3 moveOffset = _players[1].transform.position - _players[0].transform.position;
        _player1Mover = new Move(_players[0], moveOffset, 1.2f);
        _player2Mover = new Move(_players[1], -moveOffset, 1.2f);
    }
}
