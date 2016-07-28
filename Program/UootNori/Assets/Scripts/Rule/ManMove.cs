using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PatternSystem;
using UootNori;
using FlowContainer;

public class ManMove : Attribute {
    float _curTime = 0.0f;
    PatternSystem.Arrange _mover;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (_isDone)
            return;
        
        if (_mover == null)
        {
            if (GameData.CurAnimalCount() > 0)
            {
                if(GameData.GetCurTurnOutPiecess() > 0)
                {
                    if (GameData.GetLastAnimal() != Animal.BACK_DO)
                    {
                        _mover = GameData.NewInField(GameData.GetLastAnimal());
                    }   
                    else
                    {
                        List<PiecesMoveContainer> movers = GameData.GetPiecesMover(GameData.CurTurn);
                        if (movers.Count > 0)
                        {
                            _mover = GameData.MoveContainer(movers[0], GameData.GetLastAnimal());
                        }
                        else
                        {
                            _mover = GameData.NewInField(GameData.GetLastAnimal());
                        }
                    }
                }
                else
                {
                    List<PiecesMoveContainer> movers = GameData.GetPiecesMover(GameData.CurTurn);
                    if (movers.Count > 0)
                    {
                        _mover = GameData.MoveContainer(movers[0], GameData.GetLastAnimal());
                    }
                    else
                    {
                        _mover = GameData.NewInField(GameData.GetLastAnimal());
                    }
                }
                
                GameData.RemoveAnimal(GameData.CurAnimalCount()-1);
            }
            return;
        }

        if (_mover.IsDone)
        {   
            if (GameData.IsOneMoreUootThrow)
            {
                _isDone = true;
                GameData.OneMoreUootThrowCheck();
                transform.parent.GetComponent<Attribute>().ReturnActive = "UootThrow";                
                return;
            }
            
            if (GameData.CurAnimalCount() > 0)
            {
                if(GameData.GetCurTurnOutPiecess() > 0)
                {
                    if (GameData.GetLastAnimal() != Animal.BACK_DO)
                    {
                        _mover = GameData.NewInField(GameData.GetLastAnimal());
                    }
                    else
                    {
                        List<PiecesMoveContainer> movers = GameData.GetPiecesMover(GameData.CurTurn);
                        if (movers.Count > 0)
                        {
                            _mover = GameData.MoveContainer(movers[0], GameData.GetLastAnimal());
                        }
                        else
                        {
                            _mover = GameData.NewInField(GameData.GetLastAnimal());
                        }
                    }
                }
                else
                {
                    List<PiecesMoveContainer> movers = GameData.GetPiecesMover(GameData.CurTurn);
                    if (movers.Count > 0)
                    {
                        _mover = GameData.MoveContainer(movers[0], GameData.GetLastAnimal());
                    }
                    else
                    {
                        _mover = GameData.NewInField(GameData.GetLastAnimal());
                    }
                }
                GameData.RemoveAnimal(GameData.CurAnimalCount()-1);
                return;
            }
            else
            {
                _isDone = true;
                _mover = null;
                GameData.TurnRollBack();
                return;
            }
        }
        _mover.Run();
	}

    void OnEnable()
    {
        List<PiecesMoveContainer> movers = GameData.GetPiecesMover(GameData.CurTurn);
        /*
        if (movers.Count == 0 && GameData.GetLastAnimal() == Animal.BACK_DO && GameData.CurAnimalCount() == 1)
        {
            _isDone = true;
            transform.parent.GetComponent<Attribute>().ReturnActive = "NextTurn";
            GameData.TurnRollBack();
            return;
        }
        */

        if (GameData.GetCurTurnOutPiecess() > 0)
        {
            if (GameData.GetLastAnimal() != Animal.BACK_DO)
            {
                _mover = GameData.NewInField(GameData.GetLastAnimal());
            }
            else
            {
                movers = GameData.GetPiecesMover(GameData.CurTurn);
                if (movers.Count > 0)
                {
                    _mover = GameData.MoveContainer(movers[0], GameData.GetLastAnimal());
                }
                else
                {
                    _mover = GameData.NewInField(GameData.GetLastAnimal());
                }
            }
        }
        else
        {
            movers = GameData.GetPiecesMover(GameData.CurTurn);
            if (movers.Count > 0)
            {
                _mover = GameData.MoveContainer(movers[0], GameData.GetLastAnimal());
            }
            else
            {
                if (GameData.GetLastAnimal() != Animal.BACK_DO)
                    _mover = GameData.NewInField(GameData.GetLastAnimal());
            }
        }

        
        GameData.RemoveAnimal(GameData.CurAnimalCount()-1);
    }
}
