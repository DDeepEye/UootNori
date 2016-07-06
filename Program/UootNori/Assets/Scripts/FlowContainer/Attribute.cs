using UnityEngine;
using System.Collections;

namespace FlowContainer
{
    public abstract class Attribute : MonoBehaviour {

        protected string _returnActive="";
        public string ReturnActive {get{return _returnActive; } set{ _returnActive = value;}}

        protected bool _isDone = false;
        public bool IsDone {get{ return _isDone; }}

        public virtual void Reset()
        {
            _isDone = false;
        }
    }
}

