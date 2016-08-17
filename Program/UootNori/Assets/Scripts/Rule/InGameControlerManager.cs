using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PatternSystem;
using UootNori;
using FlowContainer;
using System;

public class InGameControlerManager : FlowContainer.Attribute
{

    
    public struct SelecterContainer
    {
        public SelecterContainer(GameObject select, PiecesMoveContainer mover = null)
        {
            _select = select;
            _mover = mover;
        }
        public GameObject _select;
        public PiecesMoveContainer _mover;
    }

    static InGameControlerManager s_instance;
    static public InGameControlerManager Instance{get{ return s_instance; }}

    delegate void Step(KeyEvent key);
    Step _curStep = null;

    float _curTime;
    const float LimitTime = 10.0f;
    ControlMode _mode = ControlMode.CharcterMove;
    PatternSystem.Arrange _shootEffect = null;

    List<PiecesMoveContainer> _movers;
    List<SelecterContainer> _selecterMovers = new List<SelecterContainer>();
    int _choiceIndex;

    GameObject _shootEffectObjOrigin;
    GameObject _shootEffectClone;

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
        if (IsDone)
            return;
        _selecterMovers[_choiceIndex]._select.SetActive(false);
        switch (key)
        {
            case KeyEvent.LEFT_EVENT:
                {
                    if (_choiceIndex == 0)
                    {
                        _choiceIndex = _selecterMovers.Count - 1;
                    }
                    else
                    {
                        --_choiceIndex;
                    }
                    _selecterMovers[_choiceIndex]._select.SetActive(true);
                }
                break;
            case KeyEvent.RIGHT_EVENT:
                {
                    if (_choiceIndex == _selecterMovers.Count - 1)
                    {
                        _choiceIndex = 0;
                    }
                    else
                    {
                        ++_choiceIndex;
                    }
                    _selecterMovers[_choiceIndex]._select.SetActive(true);
                }
                break;
            case KeyEvent.ENTER_EVENT:
                {
                    if (_mode == ControlMode.CharcterMove)
                    {
                        if(GameData.CurAnimalCount() > 1)
                        {
                            _curStep = AnimalChoice;
                            GameData.OpenAnimalChoice();
                        }
                        else
                        {
                            Animal choice = GameData.GetLastAnimal();
                            _isDone = true;
                            GameData.RemoveAnimal(choice);
                            ManMove.SetMover(_selecterMovers[_choiceIndex]._mover, choice);
                            GameData.CloseAnimalChoice();
                        }
                    }
                    else
                    {
                        ShootEffectContainerCreate();
                    }
                }
                break;
        }
    }

    public void AnimalChoice(KeyEvent key)
    {
        if (IsDone)
            return;

        switch (key)
        {
            case KeyEvent.LEFT_EVENT:
                {
                    GameData.LeftAnimalChoice();                    
                }
                break;
            case KeyEvent.RIGHT_EVENT:
                {
                    GameData.RightAnimalChoice();                    
                }
                break;
            case KeyEvent.ENTER_EVENT:
                {
                    Animal choice = GameData.EnterAnimalChoice();
                    _isDone = true;
                    GameData.RemoveAnimal(choice);
                    ManMove.SetMover(_selecterMovers[_choiceIndex]._mover, choice);
                    _selecterMovers[_choiceIndex]._select.SetActive(false);
                    GameData.CloseAnimalChoice();
                }
                break;
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (IsDone)
            return;
        
        if (_mode == ControlMode.Shoot)
        {
            if (_shootEffect != null)
            {
                if (!_shootEffect.IsDone)
                    _shootEffect.Run();
                else
                {
                    ShootDestroy();
                    _shootEffect = null;
                    _isDone = true;
                    transform.parent.GetComponent<FlowContainer.Attribute>().ReturnActive = "UootThrow";
                } 
            }
        }
	}

    void CollectChoiceObject()
    {
        _choiceIndex = 0;
        _curStep = CharacterChoice;

        _selecterMovers.Clear();

        if (_mode == ControlMode.CharcterMove)
        {
            if (GameData.GetCurTurnOutPiecess() > 0)
                _selecterMovers.Add(new SelecterContainer(GameData.s_startPoint[(int)GameData.CurTurn].transform.FindChild("Select_P").gameObject));

            _movers = GameData.GetPiecesMover(GameData.CurTurn);
            foreach(PiecesMoveContainer m in _movers)
            {
                _selecterMovers.Add(new SelecterContainer(m.Pieces.transform.FindChild("Select_P").gameObject, m));
            }

            if (_selecterMovers.Count > 1)
            {
                _choiceIndex = 0;
                _selecterMovers[0]._select.SetActive(true);
            }
            else
            {
                if(GameData.CurAnimalCount() > 1)
                {
                    _curStep = AnimalChoice;
                    GameData.OpenAnimalChoice();
                }
                else
                {
                    Animal animal = GameData.GetLastAnimal();
                    GameData.RemoveAnimal(animal);
                    if (GameData.GetCurTurnOutPiecess() > 0)
                        ManMove.SetMover(null, animal);
                    else
                        ManMove.SetMover(_selecterMovers[0]._mover, animal);
                    _isDone = true;
                }
            }
        }
        else
        {
            if(_shootEffectObjOrigin == null)
            {
                _shootEffectObjOrigin = Resources.Load("Shoot_Root_P") as GameObject;
            }
            PLAYER_KIND targetKind = GameData.CurTurn == PLAYER_KIND.PLAYER_1 ? PLAYER_KIND.PLAYER_2 : PLAYER_KIND.PLAYER_1;
            _movers = GameData.GetPiecesMover(targetKind);
            foreach (PiecesMoveContainer m in _movers)
            {
                GameObject zweck = m.Pieces.transform.FindChild("billboard_P").FindChild("Zweck_P").gameObject;
                _selecterMovers.Add(new SelecterContainer(zweck, m));
            }
            if (_selecterMovers.Count == 0)
            {
                _isDone = true;
            }
            else
            {
                if(_selecterMovers.Count == 1)
                {
                    _choiceIndex = 0;
                    ShootEffectContainerCreate();
                }
            }
        }
    }

    public void Shoot()
    {
        _shootEffectClone = GameObject.Instantiate(_shootEffectObjOrigin) as GameObject;
        _shootEffectClone.transform.FindChild("Shoot_P").GetComponent<TweenTransform>().to = _selecterMovers[_choiceIndex]._mover.Pieces.transform;
        _shootEffectClone.SetActive(true);
    }

    public void ShootDestroy()
    {
        GameObject.Destroy(_shootEffectClone);
        _shootEffectClone = null;
    }

    public class ShootEffect : PatternSystem.Container
    {
        public override void Run()
        {
            InGameControlerManager.Instance.Shoot();                                          
        }
    }

    void ShootEffectContainerCreate()
    {
        List<Container> actions = new List<Container>();
        actions.Add(new ShootEffect());
        actions.Add(new Timer(null, 2.0f));
    }

    public void Event(KeyEvent key)
    {
        if (!gameObject.active)
            return;
        _curStep(key);
    }

    void OnEnable()
    {
        _curTime = 0.0f;        
    }
}
