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
                List<PiecesMoveContainer> movers = GameData.GetPiecesMover(GameData.CurTurn);
                if (movers.Count > 0)
                {
                    _mover = GameData.MoveContainer(movers[0], GameData.GetLastAnimal());
                }
                else
                {
                    if(GameData.GetLastAnimal() != Animal.BACK_DO)
                        _mover = GameData.NewInField(GameData.GetLastAnimal());
                }
                GameData.RemoveAnimal(GameData.CurAnimalCount()-1);
            }
            return;
        }

        if (_mover.IsDone)
        {
            if (GameData.CurAnimalCount() > 0)
            {
                List<PiecesMoveContainer> movers = GameData.GetPiecesMover(GameData.CurTurn);
                if (movers.Count > 0)
                {
                    _mover = GameData.MoveContainer(movers[0], GameData.GetLastAnimal());
                }
                else
                {
                    if(GameData.GetLastAnimal() != Animal.BACK_DO)
                        _mover = GameData.NewInField(GameData.GetLastAnimal());
                }
                GameData.RemoveAnimal(GameData.CurAnimalCount()-1);
                return;
            }
            else
            {
                _isDone = true;
                _mover = null;
                transform.parent.GetComponent<Attribute>().ReturnActive = "UootThrow";
                return;
            }
        }

        _mover.Run();
	}

    void OnEnable()
    {
        /*
        GameObject origin_pieces = Resources.Load("Uoot_N") as GameObject;
        GameObject pieces = GameObject.Instantiate(origin_pieces);
        pieces.transform.position = GameData.GetExitField().GetSelfField().transform.position;

        List<Vector3> points = GameData.GetWay(2);
        List<Container> containers = new List<Container>();
        containers.Add(new Timer(pieces, 1.0f));
        Vector3 offsetPoint = pieces.transform.position;
        foreach (Vector3 point in points)
        {
            Vector3 p = point - offsetPoint;
            offsetPoint = point;
            containers.Add(new Timer(pieces, 0.1f));
            containers.Add(new Move(pieces, p, 0.15f));
        }

        road = new Arrange(pieces, Arrange.ArrangeType.SERIES, containers, 0);
        */
        List<PiecesMoveContainer> movers = GameData.GetPiecesMover(GameData.CurTurn);
        if (movers.Count > 0)
        {
            _mover = GameData.MoveContainer(movers[0], GameData.GetLastAnimal());
        }
        else
        {
            if(GameData.GetLastAnimal() != Animal.BACK_DO)
                _mover = GameData.NewInField(GameData.GetLastAnimal());
        }
        GameData.RemoveAnimal(GameData.CurAnimalCount()-1);
    }
}
