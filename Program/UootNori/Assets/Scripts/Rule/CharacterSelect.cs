using UnityEngine;
using System.Collections;
using FlowContainer;
using UootNori;

public class CharacterSelect : Arrange
{

    GameObject _characterSelect;
    GameObject _character1;
    GameObject _character2;
    GameObject _curChoice;

    float _curSelectWaitTime;
    
	// Use this for initialization
	void Start () {
        if (_characterSelect == null)
        {
            _characterSelect = GameObject.Find("UI Root").transform.FindChild("Size").FindChild("CharacterSelect").gameObject;
            _character1 = _characterSelect.transform.FindChild("Character01_P").FindChild("Select_P").gameObject;
            _character2 = _characterSelect.transform.FindChild("Character02_P").FindChild("Select_P").gameObject;

            _character1.SetActive(true);
            _character2.SetActive(false);
            _characterSelect.SetActive(true);
        }
	}
    
	
	// Update is called once per frame
	void Update () {
        if (IsDone)
            return;

        _curSelectWaitTime += Time.deltaTime;
        if (_curSelectWaitTime > 5.0f)
        {
            GameData.Player2IsCharacter1(_curChoice == _character2 ? true : false);
            _isDone = true;
            _characterSelect.SetActive(false);
        }
	}

    void OnEnable()
    {
        InputManager.Instance.InputAttribute = this;
        if (_characterSelect == null)
        {
            _characterSelect = GameObject.Find("UI Root").transform.FindChild("Size").FindChild("CharacterSelect").gameObject;
            _character1 = _characterSelect.transform.FindChild("Character01_P").FindChild("Select_P").gameObject;
            _character2 = _characterSelect.transform.FindChild("Character02_P").FindChild("Select_P").gameObject;
        }
        _character1.SetActive(true);
        _character2.SetActive(false);
        _characterSelect.SetActive(true);
        _curChoice = _character1;
        _curSelectWaitTime = 0.0f;

        if (InputManager.Instance.CurPlayer <= PlayerControl.Player2)
            NextTurnCheck.Instance.intactlyCamera();
        else
            NextTurnCheck.Instance.reverseCamera();
    }

    public override void Event(KeyEvent key)
    {
        switch(key)
        {
            case KeyEvent.LEFT_EVENT:
            case KeyEvent.RIGHT_EVENT:
                _curChoice.SetActive(false);
                _curChoice = (_curChoice == _character1 ? _character2 : _character1);
                _curChoice.SetActive(true);
                break;
            case KeyEvent.ENTER_EVENT:
                bool result = ((int)InputManager.Instance.CurPlayer % 2) == 0 ? true : false;

                GameData.Player2IsCharacter1(_curChoice == _character2 ? result : !result);
                _isDone = true;
                _characterSelect.SetActive(false);
                break;
        }
    }
}
