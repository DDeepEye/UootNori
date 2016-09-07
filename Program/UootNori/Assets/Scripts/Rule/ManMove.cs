using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PatternSystem;
using UootNori;
using FlowContainer;

public class ManMove : Attribute {
    float _curTime = 0.0f;

    PiecesMoveContainer _mover;
    Animal _moveAnimal;

    PatternSystem.Arrange _moveProc;

    static ManMove s_inst;



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (_isDone)
            return;
        
        if (_moveProc == null)
        {  
            return;
        }

        if (_moveProc.IsDone)
        {
            _isDone = true;

            if (GameData.s_players[(int)GameData.CurTurn].GetGoalInNum() == GameData.PIECESMAX)
            {
                transform.parent.GetComponent<Attribute>().ReturnActive = "RullProcess";
                return;
            }

            if (GameData.IsShoot)
            {
                transform.parent.GetComponent<Attribute>().ReturnActive = "InGameControlerManager";
                
                GameData.ShootCheck();
                return;
            }

            if (GameData.IsOneMoreUootThrow)
            {                
                GameData.OneMoreUootThrowCheck();
                transform.parent.GetComponent<Attribute>().ReturnActive = "UootThrow";
                if(GameData.GetCurTurnOutPiecess() > 0)
                {
                    GameData.s_startPoint[(int)GameData.CurTurn].SetActive(true);
                    TextMesh tm = GameData.s_startPoint[(int)GameData.CurTurn].transform.FindChild("billboard_P").FindChild("Population_P").FindChild("Population_Label_P").GetComponent<TextMesh>();
                    tm.text = GameData.GetCurTurnOutPiecess().ToString();
                }   
                return;
            }

            if(GameData.CurAnimalCount() > 0)
            {
                transform.parent.GetComponent<Attribute>().ReturnActive = "InGameControlerManager";
                InGameControlerManager.Instance.ReadyToCharacterMode();
                if (GameData.GetCurTurnOutPiecess() > 0)
                    GameData.s_startPoint[(int)GameData.CurTurn].SetActive(true);
                return;
            }
            return;
        }
        _moveProc.Run();
	}

    void OnEnable()
    {
        InputManager.Instance.InputAttribute = this;
        if(_mover != null)
        {
            _moveProc = GameData.MoveContainer(_mover, _moveAnimal);
        }
        else
        {
            _moveProc = GameData.NewInField(_moveAnimal);
        }
    }

    static public void SetMover(PiecesMoveContainer mover, Animal movingValue)
    {
        if (s_inst == null)
        {
            s_inst = GameObject.Find("Flow").transform.FindChild("GameFlow").FindChild("InGame").FindChild("InGameFlow").FindChild("GamePlay").FindChild("ManMove").GetComponent<ManMove>();
        }

        s_inst._mover = mover;
        s_inst._moveAnimal = movingValue;
    }
}
