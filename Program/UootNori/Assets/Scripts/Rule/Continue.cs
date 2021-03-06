﻿using UnityEngine;
using System.Collections;
using FlowContainer;
using UootNori;

public class Continue : Attribute {

    GameObject _continue;
    GameObject _yes;
    GameObject _no;
    GameObject _choice;
    GameObject _count;
    float _curTime;
    int _curCount = 20;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (IsDone)
            return;

        _curTime += Time.deltaTime;
        if(_curTime > 20.0f)
        {
            _isDone = true;
            _continue.SetActive(false);
            GameData.ReSetGame(false);
        }

        if (_curCount != ((int)(20 - _curTime)))
        {
            _curCount = (int)(20 - _curTime);
            _count.GetComponent<UISprite>().spriteName = _curCount.ToString();
        }
	}

    public override void Event(KeyEvent key)
    {
        if(key == KeyEvent.ENTER_EVENT)
        {
            bool isReGame = (_choice == _yes ? true : false);
            if(isReGame)
            {
                if (GameData._is4p && GameData._curCreditCount < 2)
                    return;

                if (GameData._curCreditCount == 0)
                    return;

                GameData.ConsumeCredit(GameData._is4p);                
            }
            _isDone = true;
            GameData.ReSetGame(isReGame);
            if (isReGame)
                transform.parent.GetComponent<Attribute>().ReturnActive = "GamePlay";
            _continue.SetActive(false);
            InputManager.Instance.InputAttribute = null;
        }
        else
        {
            _choice.SetActive(false);
            _choice = (_choice == _yes ? _no : _yes);
            _choice.SetActive(true);
        } 
    }

    void OnEnable()
    {
        if (GameData.s_isDemo)
        {
            _isDone = true;
            GameData.ReSetGame(false);
            transform.parent.GetComponent<Attribute>().ReturnActive = "Title";
            _continue.SetActive(false);
            return;
        }

        if (_continue == null)
        {
            _continue = GameObject.Find("UI Root").transform.FindChild("Size").FindChild("Continue").gameObject;
            _yes = _continue.transform.FindChild("Yes").gameObject;
            _no = _continue.transform.FindChild("No").gameObject;
            _count = _continue.transform.FindChild("Count").gameObject;
        }
        _continue.SetActive(true);
        _choice = _yes;
        _curCount = 20;
        _curTime = 0.0f;
        InputManager.Instance.InputAttribute = this;
    }
}
