using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PatternSystem;
using UootNori;
using FlowContainer;

public class InGameControlerManager : Attribute {

    static InGameControlerManager s_instance;
    static public InGameControlerManager Instance{get{ return s_instance; }}

    delegate void Step(KeyEvent key);
    Step _curStep = null;

    float _curTime;
    const float LimitTime = 10.0f;
    ControlMode _mode = ControlMode.CharcterMove;
    PatternSystem.Arrange _shootEffect = null;

    PiecesMoveContainer _choiceMover = null;
    Animal              _choiceAnimal = Animal.NONE;

    InGameControlerManager()
    {
        s_instance = this;
    }

    public void ReadyToCharacterMode()
    {
        _mode = ControlMode.CharcterMove;
        CollectChoiceObject();
    }

    public void ReadyToShootMode()
    {
        _mode = ControlMode.Shoot;
        CollectChoiceObject();
    }

    public void CharacterChoice(KeyEvent key)
    {
        switch (key)
        {
            case KeyEvent.LEFT_EVENT:
                break;
            case KeyEvent.RIGHT_EVENT:
                break;
            case KeyEvent.ENTER_EVENT:
                if (_mode == ControlMode.CharcterMove)
                {
                    _curStep = AnimalChoice;
                }
                else
                {
                }
                break;
        }
    }

    public void AnimalChoice(KeyEvent key)
    {
        
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (IsDone)
            return;
        
        _isDone = true;

        
        if (_mode == ControlMode.Shoot)
        {
            if (_shootEffect != null)
            {
                if (!_shootEffect.IsDone)
                    _shootEffect.Run();
                else
                    _shootEffect = null;
            }
        }
	}

    void CollectChoiceObject()
    {
        if (_mode == ControlMode.CharcterMove)
        {
        }
        else
        {
            
        }
    }



    public void Event(KeyEvent key)
    {
        if (!gameObject.activeSelf)
            return;
        _curStep(key);
    }

    void OnEnable()
    {
        _curTime = 0.0f;
        _curStep = CharacterChoice;
    }
}
