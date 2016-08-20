using System;
using UnityEngine;
using System.Collections.Generic;
using PatternSystem;

public class JumpingMove : Move
{
    public JumpingMove(GameObject target, Vector3 translatePoint, float time)
            : base(target, translatePoint, time)
	{

    }

    public override void Run()
    {
        if (IsDone)
            return;

        base.Run();
        Vector3 center = _target.transform.position + (_translatePoint * 0.5f);
        center -= new Vector3(0, 1, 0);
        Vector3 riseRelCenter = _target.transform.position - center;
        Vector3 setRelCenter = _target.transform.position + _translatePoint - center;

        float tickTime = _curTime > _time ? _curTime - _time : UnityEngine.Time.deltaTime;

        if (_time > 0)
            tickTime *= 1 / _time;
        
        Vector3 r = Vector3.Slerp(riseRelCenter, setRelCenter, tickTime);
        center.y += r.y;
        _target.transform.position = new Vector3(_target.transform.position.x, center.y, _target.transform.position.z);

        if (_curTime >= _time)
        {
            _target.transform.position = _resultValue;
            _isDone = true;
        }
    }
}
