using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FlowContainer;
using UootNori;

public class Calculate : Attribute {

    static Calculate s_instance;
    public class MainChoiceContainer
    {
        public delegate void EventProc(KeyEvent key);
        public delegate void Proc();

        public GameObject _select;
        public MainChoiceContainer _left;
        public MainChoiceContainer _right;
        public EventProc _eventProc;
        public Proc _enable;
        public Proc _disable;
    }
    
    MainChoiceContainer _curChoice;

    UILabel _gamePriceValue;
    UILabel _characterrNumValue;


    static public Calculate Instance
    {
        get
        {
            if (s_instance == null)
            {
                MainChoiceContainer price = new MainChoiceContainer();
                MainChoiceContainer piecesNum = new MainChoiceContainer();
                MainChoiceContainer editOK = new MainChoiceContainer();

                s_instance = GameObject.Find("Flow").transform.FindChild("GameFlow").FindChild("Calculate").GetComponent<Calculate>();
                editOK._select = GameObject.Find("UI Root").transform.FindChild("Size").FindChild("Calculate").FindChild("Modify").FindChild("GameObject").FindChild("Select_P").gameObject;
                GameObject gamePrice = GameObject.Find("UI Root").transform.FindChild("Size").FindChild("Calculate").FindChild("GamePrice").FindChild("GameObject").FindChild("Select_P").gameObject;
                s_instance._gamePriceValue = GameObject.Find("UI Root").transform.FindChild("Size").FindChild("Calculate").FindChild("GamePrice").FindChild("GameObject").FindChild("Value_Label_P").GetComponent<UILabel>();
                GameObject character = GameObject.Find("UI Root").transform.FindChild("Size").FindChild("Calculate").FindChild("Character").FindChild("GameObject").FindChild("Select_P").gameObject;
                s_instance._characterrNumValue = GameObject.Find("UI Root").transform.FindChild("Size").FindChild("Calculate").FindChild("Character").FindChild("GameObject").FindChild("Value_Label_P").GetComponent<UILabel>();

                s_instance._curChoice = editOK;
                s_instance._curEventProc = s_instance.MainChoice;

                price._select = gamePrice;
                piecesNum._select = character;

                editOK._left = price;
                editOK._right = piecesNum;

                editOK._enable = s_instance.EidtEnable;
                editOK._eventProc = s_instance.EditEventProc;

                price._left = piecesNum;
                price._right = editOK;
                price._eventProc = s_instance.PriceChoice;
                price._enable = s_instance.PriceChoiceEnable;
                price._disable = s_instance.PriceChoiceDisable;

                piecesNum._left = editOK;
                piecesNum._right = price;
                piecesNum._eventProc = s_instance.PiecesNumChoice;
                piecesNum._enable = s_instance.PiecesChoiceEnable;
                piecesNum._disable = s_instance.PiecesChoiceDisable;

                editOK._select.SetActive(true);
            }

            return s_instance;
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}



    MainChoiceContainer.EventProc _curEventProc;

    public void MainChoice(KeyEvent key)
    {
        switch (key)
        {
            case KeyEvent.LEFT_EVENT:
                _curChoice._select.SetActive(false);
                _curChoice = _curChoice._left;
                _curChoice._select.SetActive(true);
                break;
            case KeyEvent.RIGHT_EVENT:
                _curChoice._select.SetActive(false);
                _curChoice = _curChoice._right;
                _curChoice._select.SetActive(true);
                break;
            case KeyEvent.ENTER_EVENT:
                _curChoice._enable();
                break;
        }
    }

    public void MainChoiceEnter()
    {
        _curEventProc = _curChoice._eventProc;
    }

    List<int> _priceList = new List<int>() { 1, 2, 4, 6 };
    int _curPriceIdx = 0;

    public void PriceChoice(KeyEvent key)
    {
        switch (key)
        {
            case KeyEvent.LEFT_EVENT:
                if (_curPriceIdx > 0)
                    --_curPriceIdx;
                _gamePriceValue.text = (_priceList[_curPriceIdx]*500).ToString();
                break;
            case KeyEvent.RIGHT_EVENT:
                if (_curPriceIdx < _priceList.Count - 1)
                    ++_curPriceIdx;
                _gamePriceValue.text = (_priceList[_curPriceIdx] * 500).ToString();
                break;
            case KeyEvent.ENTER_EVENT:
                _curChoice._disable();
                break;
        }
    }

    public void PriceChoiceEnable()
    {
        _curEventProc = PriceChoice;
    }

    public void PriceChoiceDisable()
    {
        _curEventProc = MainChoice;
    }

    List<int> _piecesNumList = new List<int>() { 1, 2, 3, 4, 5 };
    int _curPiecesNum = 0;
    public void PiecesNumChoice(KeyEvent key)
    {
        switch (key)
        {
            case KeyEvent.LEFT_EVENT:
                if (_curPiecesNum > 0)
                    --_curPiecesNum;
                _characterrNumValue.text = _piecesNumList[_curPiecesNum].ToString();
                break;
            case KeyEvent.RIGHT_EVENT:
                if (_curPiecesNum < _piecesNumList.Count - 1)
                    ++_curPiecesNum;
                _characterrNumValue.text = _piecesNumList[_curPiecesNum].ToString();
                break;
            case KeyEvent.ENTER_EVENT:
                _curChoice._disable();
                break;
        }
    }

    public void PiecesChoiceEnable()
    {
        _curEventProc = PiecesNumChoice;
    }

    public void PiecesChoiceDisable()
    {
        _curEventProc = MainChoice;
    }

    public void EditEventProc(KeyEvent key)
    {
        return;
    }

    public void EidtEnable()
    {
        GameData.PIECESMAX = _piecesNumList[_curPiecesNum];
        GameData._1creditToCount = _priceList[_curPriceIdx];
        _curEventProc = MainChoice;
        GameData.ReSetGame(false);
    }

    public override void Event(KeyEvent key)
    {
        _curEventProc(key);
    }
}
