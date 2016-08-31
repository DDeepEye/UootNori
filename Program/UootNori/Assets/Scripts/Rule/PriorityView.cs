using UnityEngine;
using System.Collections;
using FlowContainer;
using UootNori;

public class PriorityView : Attribute {

    float _curTime = 0.0f;

    GameObject _priorityScene;
    bool _isRegame = false;

    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void Update () {
        if(IsDone)
            return;

        _curTime += Time.deltaTime;
        if(_curTime > 3.0f)
        {
            _isDone = true;
            _priorityScene.SetActive(false);
        }
    }

    static public void Regame(bool isRegame)
    {
        PriorityView view = GameObject.Find("Flow").transform.FindChild("GameFlow").FindChild("InGame").FindChild("InGameFlow").FindChild("GamePlay").FindChild("PriorityView").GetComponent<PriorityView>();
        view._isRegame = isRegame;
    }

    void OnEnable()
    {
        if (_isRegame)
        {
            _isDone = true;
            return;
        }
        if (_priorityScene == null)
            _priorityScene = GameObject.Find("UI Root").transform.FindChild("Size").FindChild("OrderOfPriority").gameObject;
        _priorityScene.SetActive(true);
        InputManager.Instance.InputAttribute = this;
    }
    public override void Reset()
    {
        base.Reset();
        _curTime = 0.0f;
    }
}
