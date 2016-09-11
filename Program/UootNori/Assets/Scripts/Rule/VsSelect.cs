using UnityEngine;
using System.Collections;
using UootNori;
using FlowContainer;

public class VsSelect : Attribute
{
    GameObject _vsSelect;
    GameObject _1_1_;
    GameObject _2_2_;
    GameObject _curChoice;

    float _curSelectWaitTime;

    // Use this for initialization
    void Start()
    {
        if (_vsSelect == null)
        {
            _vsSelect = GameObject.Find("UI Root").transform.FindChild("Size").FindChild("VsSelect").gameObject;
            _1_1_ = _vsSelect.transform.FindChild("1:1_P").FindChild("Select_P").gameObject;
            _2_2_ = _vsSelect.transform.FindChild("2:2_P").FindChild("Select_P").gameObject;

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

        _curSelectWaitTime += Time.deltaTime;
        float limit = GameData.s_isDemo ? 2.0f : 5.0f;

        if (_curSelectWaitTime > limit)
        {
            _isDone = true;                
            _vsSelect.SetActive(false);
            if(!GameData.s_isDemo)
                GameData.ConsumeCredit(_curChoice == _2_2_);
        }
    }
    void OnDisable()
    {
        _vsSelect.SetActive(false);
    }

    void OnEnable()
    {
        InputManager.Instance.InputAttribute = this;
        if (_vsSelect == null)
        {
            _vsSelect = GameObject.Find("UI Root").transform.FindChild("Size").FindChild("VsSelect").gameObject;
            _1_1_ = _vsSelect.transform.FindChild("1:1_P").FindChild("Select_P").gameObject;
            _2_2_ = _vsSelect.transform.FindChild("2:2_P").FindChild("Select_P").gameObject;
        }
        _1_1_.SetActive(true);
        _2_2_.SetActive(false);
        _vsSelect.SetActive(true);
        _curChoice = _1_1_;
        _curSelectWaitTime = 0.0f;

        if (InputManager.Instance.CurPlayer <= PlayerControl.Player2)
            NextTurnCheck.Instance.intactlyCamera();
        else
            NextTurnCheck.Instance.reverseCamera();

    }

    public override void Event(KeyEvent key)
    {
        switch (key)
        {
            case KeyEvent.LEFT_EVENT:
            case KeyEvent.RIGHT_EVENT:
                if(GameData._curCreditCount > 1)
                {
                    _curChoice.SetActive(false);
                    _curChoice = (_curChoice == _1_1_ ? _2_2_ : _1_1_);
                    _curChoice.SetActive(true);
                }
                break;
            case KeyEvent.ENTER_EVENT:
                _isDone = true;                
                _vsSelect.SetActive(false);
                GameData.ConsumeCredit(_curChoice == _2_2_);

                InputManager.Instance.ResetPlayer = (_curChoice == _2_2_) ? PlayerControl.Player1 : InputManager.Instance.ResetPlayer;
                InputManager.Instance._maxControlNum = (_curChoice == _2_2_) ? PlayerControl.MAX : InputManager.Instance._maxControlNum;
                SoundPlayer.Instance.Play("sound0/effect/Button0");

                break;
        }
    }
}


