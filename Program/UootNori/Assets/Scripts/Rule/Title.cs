using UnityEngine;
using System.Collections;
using FlowContainer;


public class Title : Attribute {

    float _curTime = 0.0f;

    GameObject _titleScene;

	// Use this for initialization
	void Start () {

        if(_titleScene == null)
            _titleScene = GameObject.Find("UI Root").transform.FindChild("Size").FindChild("Title").gameObject;

        _titleScene.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {
        if(IsDone)
            return;
        
        _curTime += Time.deltaTime;
        if(_curTime > 4.0f)
        {
            _isDone = true;
            _titleScene.SetActive(false);
        }
	}

    void OnEnable()
    {
    }
    public override void Reset()
    {
        base.Reset();
        _curTime = 0.0f;
    }
}
