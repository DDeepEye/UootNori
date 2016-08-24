﻿using UnityEngine;
using System.Collections;
using UootNori;
using FlowContainer;

public class VsSelect : Attribute
{
    GameObject _vsSelect;
    GameObject _1_1_;
    GameObject _2_2_;
    GameObject _curChoice;

    // Use this for initialization
    void Start()
    {
        if (_vsSelect == null)
        {
            _vsSelect = GameObject.Find("UI Root").transform.FindChild("Size").FindChild("VsSelect").gameObject;
            _1_1_ = _vsSelect.transform.FindChild("2:2_P").FindChild("Select_P").gameObject;
            _2_2_ = _vsSelect.transform.FindChild("1:1_P").FindChild("Select_P").gameObject;

            _1_1_.SetActive(true);
            _2_2_.SetActive(false);
            _vsSelect.SetActive(true);
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (IsDone)
            return;
    }

    void OnEnable()
    {
        InputManager.Instance.InputAttribute = this;
        if (_vsSelect == null)
        {
            _vsSelect = GameObject.Find("UI Root").transform.FindChild("Size").FindChild("VsSelect").gameObject;
            _1_1_ = _vsSelect.transform.FindChild("2:2_P").FindChild("Select_P").gameObject;
            _2_2_ = _vsSelect.transform.FindChild("1:1_P").FindChild("Select_P").gameObject;
        }
        _1_1_.SetActive(true);
        _2_2_.SetActive(false);
        _vsSelect.SetActive(true);
        _curChoice = _1_1_;
    }

    public override void Event(KeyEvent key)
    {
        switch (key)
        {
            case KeyEvent.LEFT_EVENT:
            case KeyEvent.RIGHT_EVENT:
                _curChoice.SetActive(false);
                _curChoice = (_curChoice == _1_1_ ? _2_2_ : _1_1_);
                _curChoice.SetActive(true);
                break;
            case KeyEvent.ENTER_EVENT:
                _isDone = true;
                _vsSelect.SetActive(false);
                break;
        }
    }
}

