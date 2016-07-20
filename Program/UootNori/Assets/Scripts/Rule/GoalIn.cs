using UnityEngine;
using System.Collections;
using PatternSystem;
using UootNori;

public class GoalIn : Property {
    PiecesMoveContainer _destroyMover;
    public GoalIn(GameObject target, PiecesMoveContainer destroyMover) : base(target)
    {
        _destroyMover = destroyMover;
    }

    public override void Run()
    {
        GameObject.Destroy(_target);
        GameData.DestoryMover(_destroyMover, GameData.MoverDestroyKind.GOALIN);
    }
}
