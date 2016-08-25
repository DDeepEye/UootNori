using UnityEngine;
using System.Collections;
using FlowContainer;
using UootNori;

public class Calculate : Attribute {

    static Calculate s_instance;

    delegate void EventProc(KeyEvent key);
    EventProc _curEventProc;

    public class MainChoiceContainer
    {
        public GameObject _select;
        public MainChoiceContainer _left;
        public MainChoiceContainer _right;
    }

    MainChoiceContainer _curMainChoice = new MainChoiceContainer();

    static public Calculate Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = GameObject.Find("Flow").transform.FindChild("GameFlow").FindChild("Calculate").GetComponent<Calculate>();
                s_instance._curEventProc = s_instance.MainChoice;
                s_instance._curMainChoice._select = GameObject.Find("UI Root").transform.FindChild("Size").FindChild("Calculate").FindChild("Modify").FindChild("GameObject").FindChild("Select_P").gameObject;
                GameObject gamePrice = GameObject.Find("UI Root").transform.FindChild("Size").FindChild("Calculate").FindChild("GamePrice").FindChild("GameObject").FindChild("Select_P").gameObject;
                GameObject character = GameObject.Find("UI Root").transform.FindChild("Size").FindChild("Calculate").FindChild("Character").FindChild("GameObject").FindChild("Select_P").gameObject;

                MainChoiceContainer price = new MainChoiceContainer();
                MainChoiceContainer piecesNum = new MainChoiceContainer();

                price._select = gamePrice;
                piecesNum._select = character;

                s_instance._curMainChoice._left = price;
                s_instance._curMainChoice._right = piecesNum;

                price._left = piecesNum;
                price._right = s_instance._curMainChoice;

                piecesNum._left = s_instance._curMainChoice;
                piecesNum._right = price;

                s_instance._curMainChoice._select.SetActive(true);
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

    public void MainChoice(KeyEvent key)
    {
        switch (key)
        {
            case KeyEvent.LEFT_EVENT:
                _curMainChoice._select.SetActive(false);
                _curMainChoice = _curMainChoice._left;
                _curMainChoice._select.SetActive(true);
                break;
            case KeyEvent.RIGHT_EVENT:
                _curMainChoice._select.SetActive(false);
                _curMainChoice = _curMainChoice._right;
                _curMainChoice._select.SetActive(true);
                break;
            case KeyEvent.ENTER_EVENT:
                break;
        }
    }

    public void PriceChoice(KeyEvent key)
    {
    }

    public void PiecesNumChoice(KeyEvent key)
    {
    }

    public override void Event(KeyEvent key)
    {
        _curEventProc(key);
    }
}
