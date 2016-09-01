using UnityEngine;
using System.Collections;
using FlowContainer;
using UootNori;


public class Title : Attribute {

    float _curButtonImgChagneTime = 0.0f;

    GameObject _titleScene;

    GameObject _buttonObj;
    UISprite _buttonImage;
    string _buttonImageName = "button1";

    static public Title s_instance;
    static public Title Instance 
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = GameObject.Find("Flow").transform.FindChild("GameFlow").FindChild("Title").GetComponent<Title>();
            }
            return s_instance;
        }
    }

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
        
        _curButtonImgChagneTime += Time.deltaTime;
        if(_curButtonImgChagneTime > 0.35f)
        {
            _buttonImageName = (_buttonImageName == "button1" ? "button2" : "button1");
            _buttonImage.spriteName = _buttonImageName;
            _curButtonImgChagneTime = 0.0f;
        }
	}

    void OnEnable()
    {
        InputManager.Instance.InputAttribute = this;
        InputManager.Instance.SetPlayerNum(PlayerControl.Player4);

        InputManager.Instance._controlChoiceMode = true;
        if (_titleScene == null)
        {
            _titleScene = GameObject.Find("UI Root").transform.FindChild("Size").FindChild("Title").gameObject;
            _buttonObj = _titleScene.transform.FindChild("BG").FindChild("button").gameObject;
            _buttonImage = _buttonObj.GetComponent<UISprite>();
            _buttonImageName = _buttonImage.spriteName;
        }
        _titleScene.SetActive(true);
        if (GameData.GetCreditNum() > 0)
            _buttonObj.SetActive(true);
        else
            _buttonObj.SetActive(false);

        SoundPlayer.Instance.BGMPlay("sound0/bgm/bgm01");
        
    }
    public override void Reset()
    {
        base.Reset();
        _curButtonImgChagneTime = 0.0f;
    }

    public override void Event(KeyEvent key)
    {
        if (GameData.GetCreditNum() > 0)
        {
            _titleScene.SetActive(false);
            _isDone = true;
        }
    }

    public void OnCredit()
    {
        if (GameData.GetCreditNum() > 0)
        {
            _buttonObj.SetActive(true);
        }
    }
}
