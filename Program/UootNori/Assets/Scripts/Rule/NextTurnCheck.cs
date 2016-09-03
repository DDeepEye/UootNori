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
    GameObject _left;
    GameObject _right;
    GameObject _curArrow;

    GameObject [] _players = new GameObject[(int)PLAYER_KIND.MAX];

    static public NextTurnCheck s_instance;
    static public NextTurnCheck Instance 
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = GameObject.Find("Flow").transform.FindChild("GameFlow").FindChild("InGame").FindChild("InGameFlow").FindChild("GamePlay").FindChild("NextTurn").GetComponent<NextTurnCheck>();

                if (s_instance._players[0] == null)
                {
                    GameObject uiroot = GameObject.Find("UI Root");
                    Transform gp = uiroot.transform.FindChild("Size").FindChild("GamePlay");
                    s_instance._players[0] = gp.FindChild("Play01").gameObject;
                    s_instance._players[1] = gp.FindChild("Play02").gameObject;
                    s_instance._left = gp.transform.FindChild("S_Uoot_P").FindChild("Left_p").gameObject;
                    s_instance._left.SetActive(true);
                    s_instance._curArrow = s_instance._left;
                    s_instance._right = gp.transform.FindChild("S_Uoot_P").FindChild("right_p").gameObject;
                    s_instance._right.SetActive(false);
                }
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
            _left = gp.transform.FindChild("S_Uoot_P").FindChild("Left_p").gameObject;
            _left.SetActive(true);
            _curArrow = _left;
            _right = gp.transform.FindChild("S_Uoot_P").FindChild("right_p").gameObject;
            _right.SetActive(false);
        }

        /*
        PLAYER_KIND offMarking = (kind == PLAYER_KIND.PLAYER_1 ? PLAYER_KIND.PLAYER_2 : PLAYER_KIND.PLAYER_1);
        _players[(int)offMarking].transform.FindChild("Select_P").gameObject.SetActive(false);
        _players[(int)kind].transform.FindChild("Select_P").gameObject.SetActive(true);
        */
    }

    public void Left()
    {
        _left.SetActive(true);
        _curArrow = _left;
        _right.SetActive(false);
    }

    public void Right()
    {
        _left.SetActive(false);
        _curArrow = _right;
        _right.SetActive(true);
    }

    public void ArrowVisible(bool isVisible)
    {
        _curArrow.SetActive(isVisible);
    }

    public void reverseCamera()
    {
        GameObject camera = GameObject.Find("Field_Camera");
        camera.transform.localEulerAngles = new Vector3(camera.transform.localEulerAngles.x, camera.transform.localEulerAngles.y, 180.0f);
        camera = GameObject.Find("UI Root").transform.FindChild("Camera").gameObject;
        camera.transform.localEulerAngles = new Vector3(camera.transform.localEulerAngles.x, camera.transform.localEulerAngles.y, 180.0f);
    }

    public void intactlyCamera()
    {
        GameObject camera = GameObject.Find("Field_Camera");
        camera.transform.localEulerAngles = new Vector3(camera.transform.localEulerAngles.x, camera.transform.localEulerAngles.y, 0.0f);
        camera = GameObject.Find("UI Root").transform.FindChild("Camera").gameObject;
        camera.transform.localEulerAngles = new Vector3(camera.transform.localEulerAngles.x, camera.transform.localEulerAngles.y, 0.0f);
    }

    public void GoalIn(PLAYER_KIND kind, int goalInNum)
    {   
        ///PLAYER_KIND offMarking = (kind == PLAYER_KIND.PLAYER_1 ? PLAYER_KIND.PLAYER_2 : PLAYER_KIND.PLAYER_1);
        /// 
        string view;
        if (kind == PLAYER_KIND.PLAYER_1)
        {
            if (GameData.IsPlayer1IsCharacter1)
                view = "CH_01";
            else
                view = "CH_02";
        }
        else
        {
            if (GameData.IsPlayer1IsCharacter1)
                view = "CH_02";
            else
                view = "CH_01";
        }

        GameObject character = _players[(int)kind].transform.FindChild("Texture (1)").FindChild(view).gameObject;
        character.SetActive(true);
        character.transform.FindChild("billboard_P").FindChild("Population_P").FindChild("Population_Label_P").GetComponent<TextMesh>().text = goalInNum.ToString(); 
        character.transform.FindChild("billboard_P").FindChild("Population_P").gameObject.SetActive(true);
    }

    public void Reset()
    {   
        for (int i = 0; i < (int)PLAYER_KIND.MAX; ++i)
        {
            for (int j = 0; j < (int)PLAYER_KIND.MAX; ++j)
            {
                GameObject character = _players[i].transform.FindChild("Texture (1)").FindChild("CH_0"+(j+1).ToString()).gameObject;
                character.SetActive(false);
                character.transform.FindChild("billboard_P").FindChild("Population_P").FindChild("Population_Label_P").GetComponent<TextMesh>().text = "0"; 
                character.transform.FindChild("billboard_P").FindChild("Population_P").gameObject.SetActive(false);
            }
        }

        GameObject camera = GameObject.Find("Field_Camera");
        camera.transform.localEulerAngles = new Vector3(camera.transform.localEulerAngles.x, camera.transform.localEulerAngles.y, 0.0f);
        camera = GameObject.Find("UI Root").transform.FindChild("Camera").gameObject;
        camera.transform.localEulerAngles = new Vector3(camera.transform.localEulerAngles.x, camera.transform.localEulerAngles.y, 0.0f);
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
        }

        if (_player1Mover != null)
        {
            _player1Mover.Run();
            _player2Mover.Run();

            if (_player1Mover.IsDone)
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
        if (GameData._is4p)
        {
            if (InputManager.Instance.CurPlayer == PlayerControl.Player2 || InputManager.Instance.CurPlayer == PlayerControl.Player4)
            {
                if (!GameData.s_IsNotControlChange)
                {
                    GameObject camera = GameObject.Find("Field_Camera");
                    _cameraRot = new Rotation(camera, new Vector3(0.0f, 0.0f, 180.0f), 1.8f, Physical.Type.RELATIVE);
                    camera = GameObject.Find("UI Root").transform.FindChild("Camera").gameObject;
                    _uiCameraRot = new Rotation(camera, new Vector3(0.0f, 0.0f, 180.0f), 1.8f, Physical.Type.RELATIVE);
                }
                else
                {
                    _cameraRot = null;
                    _uiCameraRot = null;
                }
            }
        }

        Vector3 moveOffset = _players[1].transform.position - _players[0].transform.position;
        _player1Mover = new Move(_players[0], moveOffset, 1.8f);
        _player2Mover = new Move(_players[1], -moveOffset, 1.8f);
    }


}
