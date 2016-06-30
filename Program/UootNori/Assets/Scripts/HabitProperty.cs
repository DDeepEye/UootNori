using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PatternSystem
{
    public class Triger
    {
        string _key;
        public string Key { get { return _key; } }
        protected List<Container> _conditions = new List<Container>();
        public List<Container> Conditions { get { return _conditions; } }

		public Triger(string key, GameObject target, List<Container> conditions)
        {
            _key = key;
			_conditions = conditions;
        }

        public Triger(string key, GameObject target, Container c)
        {
            key = _key;
            _conditions.Add(c);
        }

        public void Run()
        {
            foreach(Container p in Conditions)
            {
                p.Run();
            }
        }
    }

    public abstract class Container
    {
        protected Container(){}
        public abstract void Run();

        protected bool _isDone = false;
        public bool IsDone { get { return _isDone; } }

        public virtual void Reset(bool isPure)
        {
            _isDone = false;
        }
    }

    public class ChildContainer : Container
    {
        List<GameObject> _children;

        public ChildContainer(List<GameObject> children)
        {
            _children = children;
        }
        public override void Run()
        {
            if (IsDone || _children == null)
                return;

            foreach (GameObject chiled in _children)
            {
                chiled.SetActive(true);
            }

            _isDone = true;
        }
    }

    public abstract class Property : Container
    {
		protected GameObject _target;
        protected Property() { }


		protected Property(GameObject target)
		{
			_target = target;
		}
    }



    public class Arrange : Property
    {
        public enum ArrangeType
        {
            SERIES,
            PARALLEL,
        }

        private ArrangeType _type = ArrangeType.SERIES;
        private List<Container> _containers = new List<Container>();
        private int _curProerty = 0;
        private int _repeatCount;
        private int _curCount = 0;

        public Arrange(GameObject target, ArrangeType type, List<Container> containers, int repeatCount):base(target)
		{
			_type = type;
            _containers = containers;
			_repeatCount = repeatCount;
		}

        public override void Run()
        {
            if (!IsDone)
            {   
                switch (_type)
                {
                    case ArrangeType.SERIES:
                        SeiesRun();
                        break;
                    case ArrangeType.PARALLEL:
                        ParallelRun();
                        break;
                }
                ResetCheck();
            }
        }

        public override void Reset(bool isPure)
        {
            _curProerty = 0;
            _isDone = false;
            _curCount = 0;
            for (int i = 0; i < _containers.Count; ++i)
                _containers[i].Reset(isPure);
        }

        private void SeiesRun()
        {
            _containers[_curProerty].Run();

            if (_containers[_curProerty].IsDone)
                ++_curProerty;
        }

        private void ParallelRun()
        {
            int doneCnt = 0;

            for (int i = 0; i < _containers.Count; ++i)
            {
                Container inter = _containers[i];
                inter.Run();

                if (inter.IsDone)
                    ++doneCnt;
            }
        }
        private void ResetCheck()
        {
            if (_containers.Count == _curProerty)
            {
                if (_repeatCount == 0)
                {
                    for (int i = 0; i < _containers.Count; ++i)
                    {
                        Container p = _containers[i];
                        p.Reset(false);
                        _curProerty = 0;
                    }
                }
                else
                {
                    if (_repeatCount == _curCount + 1)
                    {
                        for (int i = 0; i < _containers.Count; ++i)
                        {
                            Container p = _containers[i];
                            p.Reset(true);
                            _curProerty = 0;
                        }
						_isDone = true;
                    }
                    else
                    {
                        for (int i = 0; i < _containers.Count; ++i)
                        {
                            Container p = _containers[i];
                            p.Reset(false);
                            _curProerty = 0;
                        }
                        ++_curCount;
                    }
                }
            }
        }
    }

    public class Caller : Property
    {
        public delegate void CallFunc();
        public CallFunc _callFunc;

		public Caller (GameObject target) : base (target)
		{
		}

        public override void Run()
        {
            if (_isDone)
                return;

            if (_callFunc != null)
                _callFunc();

            _isDone = true;
        }
    }

    public class Timer : Property
    {
        float _time;
        public float Time { get { return _time; } }
        float _curTime;

		public Timer(GameObject target, float time) : base(target)
		{
			_time = time;
		}
        
        public override void Run()
        {
            if (_isDone)
                return;

            _curTime += UnityEngine.Time.deltaTime;

            if(_curTime >= _time)
            {
                _isDone = true;
            }
        }

        public override void Reset(bool isPure)
        {
            Reset(isPure);
            if (_curTime > _time && isPure != false)
                _curTime = _curTime - _time;
            else
                _curTime = 0.0f;
        }
    }

    public abstract class Physical : Property
    {
        public enum Type
        {
            RELATIVE,
            ABSOLUTE,
        }
        protected Vector3 _originLocalPoint;
        public Vector3 OriginLocalPosition { get { return _originLocalPoint; } }

        protected Vector3 _translatePoint;
        public Vector3 TranslatePoint { get { return _translatePoint; } }
        protected float _time;
        public float Time { get { return _time; } }
        protected float _curTime = 0.0f;

        public Type _type = Type.RELATIVE;

        protected bool _isBegin = true;

		protected Physical(GameObject target, Vector3 translatePoint, float time, Type type) : base(target)
		{	
			_originLocalPoint = target.transform.localPosition;
			_translatePoint = translatePoint;
			_time = time;
            _type = type;
		}

        public override void Reset(bool isPure)
        {
            base.Reset(isPure);
            _isBegin = true;
        }
    }

    public class Move : Physical
    {
        public Move(GameObject target, Vector3 translatePoint, float time, Type type)
            : base(target, translatePoint, time, type)
		{
			
		}
        public override void Run()
        {
			if (_isDone)
                return;
			
            _curTime += UnityEngine.Time.deltaTime;

            float tickTime = _curTime > _time ? _curTime - _time : UnityEngine.Time.deltaTime;

            if (_time != 0.0f)
                tickTime *= 1 / _time;

            tickTime = (tickTime > 1.0f) ? 1.0f : tickTime;

            _target.transform.position += _translatePoint * tickTime;

            if (_curTime >= _time)
            {
                _isDone = true;
            }
        }

        public override void Reset(bool isPure)
        {
            base.Reset(isPure);
            if (_curTime > _time && isPure)
                _curTime = _curTime - _time;
            else
                _curTime = 0.0f;
        }
    }

    public class Rotation : Physical
    {
        public Rotation(GameObject target, Vector3 translatePoint, float time, Type type)
            : base(target, translatePoint, time, type)
		{
		}
        public override void Run()
        {
            if (_isDone)
                return;

            _curTime += UnityEngine.Time.deltaTime;

            float tickTime = _curTime > _time ? _curTime - _time : UnityEngine.Time.deltaTime;
            
            if (_time != 0.0f)
                tickTime *= 1 / _time;

            tickTime = (tickTime > 1.0f) ? 1.0f : tickTime;

            Vector3 rotate = new Vector3(tickTime * _translatePoint.x
                                    , tickTime * _translatePoint.y
                                    , tickTime * _translatePoint.z);

            _target.transform.Rotate(rotate, Space.World);

            if (_curTime >= _time)
            {
                _isDone = true;
            }
        }

        public override void Reset(bool isPure)
        {
            base.Reset(isPure);
            if (_curTime > _time)
                _curTime = _curTime - _time;
            else
                _curTime = 0.0f;
        }
    }

    public class Orbit : Physical
    {
        public Orbit(GameObject target, Vector3 translatePoint, float time, Type type)
            : base(target, translatePoint, time, type)
		{
		}
        public override void Run()
        {
            if (_isDone)
                return;

            _curTime += UnityEngine.Time.deltaTime;

            float tickTime = _curTime > _time ? _curTime - _time : UnityEngine.Time.deltaTime;

            if (_time != 0.0f)
                tickTime *= 1 / _time;

            tickTime = (tickTime > 1.0f) ? 1.0f : tickTime;

            Quaternion localRotation = _target.transform.localRotation;
            _target.transform.RotateAround(_originLocalPoint, Vector3.right, _translatePoint.x * tickTime);
            _target.transform.RotateAround(_originLocalPoint, Vector3.up, _translatePoint.y * tickTime);
            _target.transform.RotateAround(_originLocalPoint, Vector3.forward, _translatePoint.z * tickTime);
            _target.transform.localRotation = localRotation;

            if (_curTime >= _time)
            {
                _isDone = true;
            }
        }

        public override void Reset(bool isPure)
        {
            base.Reset(isPure);
            if (_curTime > _time && isPure != false)
                _curTime = _curTime - _time;
            else
                _curTime = 0.0f;
        }

    }

    public class Scale : Physical
    {
        Vector3 _scaleOff;
        public Scale(GameObject target, Vector3 translatePoint, float time, Type type)
            : base(target, translatePoint, time, type)
		{   
		}
        public override void Run()
        {
            if (_isDone)
                return;

            if (_time == 0.0f)
                return;

            if (_isBegin)
            {
                _isBegin = false;
                _originLocalPoint = _target.transform.localScale;
                switch(_type)
                {
                    case Physical.Type.RELATIVE:
                        {
                            _scaleOff.x = _translatePoint.x * _originLocalPoint.x;
                            _scaleOff.y = _translatePoint.y * _originLocalPoint.y;
                            _scaleOff.z = _translatePoint.z * _originLocalPoint.z;

                            _scaleOff.x = _scaleOff.x - _originLocalPoint.x;
                            _scaleOff.y = _scaleOff.y - _originLocalPoint.y;
                            _scaleOff.z = _scaleOff.z - _originLocalPoint.z;
                        }

                        break;

                    case Physical.Type.ABSOLUTE:
                        {
                            _scaleOff.x = _translatePoint.x - _originLocalPoint.x;
                            _scaleOff.y = _translatePoint.y - _originLocalPoint.y;
                            _scaleOff.z = _translatePoint.z - _originLocalPoint.z;
                        }
                        break;
                }
            }

            _curTime += UnityEngine.Time.deltaTime;
            float off = _curTime / _time;
            off = (off > 1.0f) ? 1.0f : off;

            Vector3 localSale = new Vector3();

            
            localSale.x = _originLocalPoint.x + (_scaleOff.x * off);
            localSale.y = _originLocalPoint.y + (_scaleOff.y * off);
            localSale.z = _originLocalPoint.z + (_scaleOff.z * off);
            
            _target.transform.localScale = localSale;

            if (_curTime >= _time)
            {
                _isDone = true;
            }
        }

        public override void Reset(bool isPure)
        {
            base.Reset(isPure);
            if (_curTime > _time)
                _curTime = _curTime - _time;
            else
                _curTime = 0.0f;
        }
    }
}

