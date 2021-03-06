﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PatternSystem;
using FlowContainer;
using UootNori;

public class UootThrow : Attribute {

    /*
    도: (0.6)*(0.4^3)*(4C1)=0.1536(약 15%)
    개: (0.6^2)*(0.4^2)*(4C2)=0.3456(약 35%)
    걸: (0.6^3)*(0.4)*(4C1)=0.3456(약 35%)
    윷: (0.6^4)=0.1296(약 13%)
    모: (0.4^4)=0.0256(약 3%)
    낙: (0.6)*(0.4^3)*(4C1)=0.0512(약 5.12%) 
    빽도: (0.6)*(0.4^3)*(4C1)=0.0512(약 5.12%) 
    */


    public enum UootState
    {
        DO,
        UOOT,
        KUL,
        BACKDO,
        GE,
        MO,
        OUT,
        MAX,
    }


    public const int DO = 1900;
    public const int GE = 3500;
    public const int GUL = 3200;
    public const int UOOT = 700;
    public const int MO = 500;
    public const int BACK_DO = 200;

    public const int OUT = 100;
    public int _outOffset;

    private bool _isOut = false;
    private bool _isPriorityMode = true;
   

    List<int> _animalProbability = new List<int>();
    int [] _probabilityOffset = new int[(int)Animal.MAX];
    public static Animator s_uootAni;
    GameObject  _uootAniObj;
    GameObject  _gauge;
    UISlider _uiGauge;

    GameObject  _choiceObj;
    UISprite _choiceUI;
    Animal _curRandomChoice = Animal.DO;


    int _randTimer = 200;
    float _curTimer;
    UootState _curState = UootState.DO;

    float       _gaugeOffSet = 0.0f;
    bool _gaugeIsRight = true;
    int _curAniIndex;

    const int UOOT_NUM = 4;
    GameObject [] _uoots = new GameObject[UOOT_NUM];

    string[] _voiceKeyword = new string[4];
    
    delegate void UootAnimaion();
    UootAnimaion[] _uootAnimaion = new UootAnimaion[(int)UootNori.Animal.MAX];

    PatternSystem.Arrange _aniArrange;

    static UootThrow s_inst = null;

    const float THROW_STANBY_TIME = 3.0f;
    float _curThrowStanbyTime = 0.0f;

    delegate void Step();
    Step _curStep = null;

    static public UootThrow GetInstance()
    {
        if (s_inst == null)
        {
            s_inst = GameObject.Find("Flow").transform.FindChild("GameFlow").FindChild("InGame").FindChild("InGameFlow").FindChild("GamePlay").FindChild("UootThrow").GetComponent<UootThrow>();
            s_inst.ShuppleVoice();

            string[] gaugePath = { "Size", "GamePlay", "S_Uoot_P", "Texture", "Progress Bar" };

            GameObject finder = GameObject.Find("UI Root");

            foreach (string p in gaugePath)
            {
                finder = finder.transform.FindChild(p).gameObject;
            }
            s_inst._gauge = finder;
            s_inst._uiGauge = s_inst._gauge.GetComponent<UISlider>();

            string[] choicePath = { "Size", "GamePlay", "S_Uoot_P", "Texture", "ChoiceBar" };
            finder = GameObject.Find("UI Root");

            foreach (string p in choicePath)
            {
                finder = finder.transform.FindChild(p).gameObject;
            }
            s_inst._choiceObj = finder;

            string[] choiceUIPath = { "Size", "GamePlay", "S_Uoot_P", "Texture", "ChoiceBar", "01_P", "Uoot_Sprite_P" };
            finder = GameObject.Find("UI Root");

            foreach (string p in choiceUIPath)
            {
                finder = finder.transform.FindChild(p).gameObject;
            }
            s_inst._choiceUI = finder.GetComponent<UISprite>();
        }
        return s_inst;
    }

    public void ResetGame(bool isregame)
    {
        if (!isregame)
        {
            _isPriorityMode = true;
            _animalQueue.Clear();
            s_inst.ShuppleVoice();            
        }

        _outOffset = 0;
        for (int i = 0; i < _probabilityOffset.Length; ++i)
        {
            _probabilityOffset[i] = 0;
        }
    }

    public void ShuppleVoice()
    {
        int cnt = 0;
        List<string> keywords = new List<string>(){ "Ch","Ki","No","Su" };

        while (keywords.Count > 0)
        {
            int i = Random.Range(0, keywords.Count);
            _voiceKeyword[cnt] = keywords[i];
            keywords.Remove(_voiceKeyword[cnt]);
            ++cnt;
        }
    }

    public string CurVoicePath()
    {
        return "sound0/voice/" + _voiceKeyword[(int)InputManager.Instance.CurPlayer] + "/Voice" + _voiceKeyword[(int)InputManager.Instance.CurPlayer];
    }

    void UootAniInit()
    {
        if(_uootAniObj == null)
        {
            _uootAniObj = GameObject.Find("Uoot_ani");

            s_uootAni = _uootAniObj.GetComponent<Animator>();
            Object normal = Resources.Load("Uoot/Uoot_N");
            Object back = Resources.Load("Uoot/Uoot_B");

            _uoots[0] = GameObject.Instantiate(normal, new Vector3(0.0f, 0.0f, 0.0f),new Quaternion()) as GameObject;
            _uoots[1] = GameObject.Instantiate(normal, new Vector3(0.0f, 0.0f, 0.0f), new Quaternion()) as GameObject;
            _uoots[2] = GameObject.Instantiate(normal, new Vector3(0.0f, 0.0f, 0.0f), new Quaternion()) as GameObject;
            _uoots[3] = GameObject.Instantiate(back, new Vector3(0.0f, 0.0f, 0.0f), new Quaternion()) as GameObject;

            _uoots[0].transform.SetParent(_uootAniObj.transform.FindChild("Uoot_01"), false);
            _uoots[1].transform.SetParent(_uootAniObj.transform.FindChild("Uoot_02"), false);
            _uoots[2].transform.SetParent(_uootAniObj.transform.FindChild("Uoot_03"), false);
            _uoots[3].transform.SetParent(_uootAniObj.transform.FindChild("Uoot_04"), false);

            _uootAnimaion[(int)UootNori.Animal.DO] = Do;
            _uootAnimaion[(int)UootNori.Animal.GE] = Ge;
            _uootAnimaion[(int)UootNori.Animal.KUL] = Kul;
            _uootAnimaion[(int)UootNori.Animal.UOOT] = Uoot;
            _uootAnimaion[(int)UootNori.Animal.MO] = Mo;
            _uootAnimaion[(int)UootNori.Animal.BACK_DO] = BackDo;            
        }        
        UootThrowAni();
    }

    void Do()
    {
        Vector3 eulerAngles;
        int doUoot = Random.Range(0, 3);
        for(int i = 0; i < UOOT_NUM; ++i)
        {
            eulerAngles = _uoots[i].transform.localEulerAngles;
            eulerAngles.y = 0.0f;
            _uoots[i].transform.localEulerAngles = eulerAngles;            
        }

        eulerAngles = _uoots[doUoot].transform.localEulerAngles;
        eulerAngles.y = 180.0f;
        _uoots[doUoot].transform.localEulerAngles = eulerAngles;
    }

    void Ge()
    {
        Vector3 eulerAngles;
        int doUoot = Random.Range(0, 2);
        int geUoot = Random.Range(2, 4);
        for (int i = 0; i < UOOT_NUM; ++i)
        {
            eulerAngles = _uoots[i].transform.localEulerAngles;
            eulerAngles.y = 180.0f;
            _uoots[i].transform.localEulerAngles = eulerAngles;
        }

        eulerAngles = _uoots[doUoot].transform.localEulerAngles;
        eulerAngles.y = 0.0f;
        _uoots[doUoot].transform.localEulerAngles = eulerAngles;

        eulerAngles = _uoots[geUoot].transform.localEulerAngles;
        eulerAngles.y = 0.0f;
        _uoots[geUoot].transform.localEulerAngles = eulerAngles;
    }

    void Kul()
    {
        Vector3 eulerAngles;
        int kulUoot = Random.Range(0, 3);
        for (int i = 0; i < UOOT_NUM; ++i)
        {
            eulerAngles = _uoots[i].transform.localEulerAngles;
            eulerAngles.y = 180.0f;
            _uoots[i].transform.localEulerAngles = eulerAngles;
        }

        eulerAngles = _uoots[kulUoot].transform.localEulerAngles;
        eulerAngles.y = 0.0f;
        _uoots[kulUoot].transform.localEulerAngles = eulerAngles;
    }

    void Uoot()
    {
        Vector3 eulerAngles;
        for (int i = 0; i < UOOT_NUM; ++i)
        {
            eulerAngles = _uoots[i].transform.localEulerAngles;
            eulerAngles.y = 180.0f;
            _uoots[i].transform.localEulerAngles = eulerAngles;
        }
    }

    void Mo()
    {
        Vector3 eulerAngles;
        for (int i = 0; i < UOOT_NUM; ++i)
        {
            eulerAngles = _uoots[i].transform.localEulerAngles;
            eulerAngles.y = 0.0f;
            _uoots[i].transform.localEulerAngles = eulerAngles;
        }
    }

    void BackDo()
    {
        Vector3 eulerAngles;
        for (int i = 0; i < UOOT_NUM; ++i)
        {
            eulerAngles = _uoots[i].transform.localEulerAngles;
            eulerAngles.y = 0.0f;
            _uoots[i].transform.localEulerAngles = eulerAngles;
        }

        eulerAngles = _uoots[3].transform.localEulerAngles;
        eulerAngles.y = 180.0f;
        _uoots[3].transform.localEulerAngles = eulerAngles;
    }

    // Use this for initialization    
    void Start () {
	
	}
	// Update is called once per frame
	void Update () {
        if (_isDone)
            return;
        _curStep();
	}

    bool _isPlaye1Priority;

    Queue<Animal> _animalQueue = new Queue<Animal>();

    void PrioritySettring()
    {
        _isPlaye1Priority = Random.Range(0, 2) == 0 ? true : false;
        int animal = Random.Range((int)Animal.GE, (int)Animal.MO);
        if (_isPlaye1Priority)
        {
            _animalQueue.Enqueue((Animal)animal - 1);
            _animalQueue.Enqueue((Animal)animal);
        }
        else
        {
            _animalQueue.Enqueue((Animal)animal);
            _animalQueue.Enqueue((Animal)animal - 1);
        }
    }
    void ThrowStanbyCheck()
    {
        if (GameData.s_isDemo)
        {
            if ((_curThrowStanbyTime < THROW_STANBY_TIME && !_isPriorityMode))
            {
                _curThrowStanbyTime += Time.deltaTime;
            }
            else
            {
                Event(KeyEvent.ENTER_EVENT);
            }
        }

        if (_isPriorityMode)
        {
            NextTurnCheck.Instance.ArrowVisible(false);
            _curStep = ThrowCheck;
            if (!_isPriorityMode)
            {
                AnimalProbabiley();
                ThrowToData();
            }
            UootAniInit();
        }
        else
        {
            GaugeUpdate();
        }
    }
    
    void GaugeUpdate()
    {
        _curTimer += Time.deltaTime;
        if((float)_randTimer*0.001f < _curTimer)
        {
            _curTimer = 0.0f;
            _randTimer = Random.Range(100, 251);
            ++_curState;
            if(_curState >= UootState.MAX)
            {
                _curState = UootState.DO;
                _choiceUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
        }

        if(_curState != UootState.OUT)
        {
            string[] UootsSpriteName = { "ETC_S_Uoot_1", "ETC_S_Uoot_4", "ETC_S_Uoot_3", "ETC_S_Uoot_0", "ETC_S_Uoot_2", "ETC_S_Uoot_5", };
            Animal [] uoot = {Animal.DO, Animal.UOOT, Animal.KUL, Animal.BACK_DO, Animal.GE, Animal.MO};
            _choiceUI.spriteName = UootsSpriteName[(int)_curState];
            _curRandomChoice = uoot[(int)_curState];
            _isOut = false;
        }
        else
        {
            _choiceUI.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
            _isOut = true;
        }
        

        /*
        _uiGauge.value += (_gaugeIsRight == true ? _gaugeOffSet : -_gaugeOffSet);
        if (_uiGauge.value >= 1.0f)
        {
            _uiGauge.value = 1.0f;
            _gaugeIsRight = !_gaugeIsRight;
            _gaugeOffSet = 0.0f;
            ++_curRandomChoice;
            if (_curRandomChoice == Animal.MAX)
                _curRandomChoice = Animal.DO;

            string[] UootsSpriteName = { "ETC_S_Uoot_1", "ETC_S_Uoot_2", "ETC_S_Uoot_3", "ETC_S_Uoot_4", "ETC_S_Uoot_5", "ETC_S_Uoot_0", };
            _choiceUI.spriteName = UootsSpriteName[(int)_curRandomChoice];

        }
        else if (_uiGauge.value <= 0.0f)
        {
            _uiGauge.value = 0.0f;
            _gaugeIsRight = !_gaugeIsRight;
            _gaugeOffSet = 0.0f;
            ++_curRandomChoice;
            if (_curRandomChoice == Animal.MAX)
                _curRandomChoice = Animal.DO;
            string[] UootsSpriteName = { "ETC_S_Uoot_1", "ETC_S_Uoot_2", "ETC_S_Uoot_3", "ETC_S_Uoot_4", "ETC_S_Uoot_5", "ETC_S_Uoot_0", };
            _choiceUI.spriteName = UootsSpriteName[(int)_curRandomChoice];
        }

        _gaugeOffSet += 0.0015f;
         */
    }
     

    void ThrowCheck()
    {
        if (UootThrowAniCheck())
        {
            _gauge.SetActive(false);
            _choiceObj.SetActive(false);
            _uiGauge.value = 0.0f;
            _gaugeOffSet = 0.0f;
            _gaugeIsRight = true;
            if (_isPriorityMode)
            {                
                if (_animalQueue.Count > 0)
                {
                    _isDone = true;
                    Attribute at = transform.parent.GetComponent<Attribute>();
                    at.ReturnActive = "NextTurn";
                }
                else
                {
                    if (_isPlaye1Priority)
                    {   
                        _isDone = true;
                        Attribute at = transform.parent.GetComponent<Attribute>();
                        at.ReturnActive = "NextTurn";
                        InputManager.Instance.CurPlayer = InputManager.Instance.ResetPlayer;
                        GameData.s_IsNotControlChange = true;
                    }
                    else
                    {
                        _randTimer = Random.Range(100, 251);
                        _choiceUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                        _curTimer = 0.0f;
                        _isOut = false;
                        _choiceUI.spriteName = "ETC_S_Uoot_1";
                        _curRandomChoice = Animal.DO;
                        _curState = UootState.DO;

                        _curStep = ThrowStanbyCheck;
                        InputManager.Instance.CurPlayer = InputManager.Instance.ResetPlayer + 1;
                        NextTurnCheck.Instance.ArrowVisible(true);
                    }
                    if(GameData.CurTurn == PLAYER_KIND.PLAYER_2)
                        SoundPlayer.Instance.Play("sound0/voice/"+_voiceKeyword[(int)InputManager.Instance.CurPlayer]+"/Voice"+_voiceKeyword[(int)InputManager.Instance.CurPlayer]+"_Turn0");
                    
                    _isPriorityMode = false;
                    _gauge.SetActive(true);
                    _choiceObj.SetActive(true);
                }
                return;
            }
            if (_isOut)
            {
                _isDone = true;
                Attribute at = transform.parent.GetComponent<Attribute>();
                at.ReturnActive = "NextTurn";
                GameData.TurnRollBack();
                GameData.RefreshAnimalView(false);

                return;
            }

            if (GameData.GetLastAnimal() == Animal.UOOT || GameData.GetLastAnimal() == Animal.MO)
            {   
                /* 
                AnimalProbabiley();
                ThrowToData();
                UootAniInit();
                 * */

                _randTimer = Random.Range(100, 251);
                _choiceUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                _curTimer = 0.0f;
                _isOut = false;
                _choiceUI.spriteName = "ETC_S_Uoot_1";
                _curRandomChoice = Animal.DO;
                _curState = UootState.DO;

                _curStep = ThrowStanbyCheck;
                _gauge.SetActive(true);
                _choiceObj.SetActive(true);
                NextTurnCheck.Instance.ArrowVisible(true);
            }
            else
            {
                _isDone = true;
                Attribute at = transform.parent.GetComponent<Attribute>();
                at.ReturnActive = "";
                InGameControlerManager.Instance.ReadyToCharacterMode();
            }
        }
   
    }


    void OnEnable()
    {
        if (_isPriorityMode)
        {
            if (_animalQueue.Count == 0)
                PrioritySettring();
        }
        else
        {
            SoundPlayer.Instance.Play("sound0/voice/"+_voiceKeyword[(int)InputManager.Instance.CurPlayer]+"/Voice"+_voiceKeyword[(int)InputManager.Instance.CurPlayer]+"_Turn0");
            _gauge.SetActive(true);
            _choiceObj.SetActive(true);
            _uiGauge.value = 0.0f;
        }

        _randTimer = Random.Range(100, 251);
        _choiceUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        _curTimer = 0.0f;
        _isOut = false;
        _choiceUI.spriteName = "ETC_S_Uoot_1";
        _curRandomChoice = Animal.DO;
        _curState = UootState.DO;

        _curStep = ThrowStanbyCheck;

        InputManager.Instance.InputAttribute = this;
        NextTurnCheck.Instance.ArrowVisible(true);
        
    }

    void OnDisable()
    {
        
    }

    void AnimalProbabiley()
    {
        _probabilityOffset[3] += 25;
        _probabilityOffset[4] += 25;
        _probabilityOffset[5] += 25;
        _animalProbability.Clear();
        int prob = DO;
        _animalProbability.Add(prob+_probabilityOffset[0]);
        _animalProbability.Add(prob+=GE+_probabilityOffset[1]);
        _animalProbability.Add(prob+=GUL+_probabilityOffset[2]);
        _animalProbability.Add(prob+=UOOT+_probabilityOffset[3]);
        _animalProbability.Add(prob+=MO+_probabilityOffset[4]);
        _animalProbability.Add(prob+=BACK_DO+_probabilityOffset[5]);
    }


    List<Animal> _tempanimalQueue = new List<Animal>() { Animal.BACK_DO, Animal.DO, Animal.BACK_DO, Animal.DO, Animal.BACK_DO, Animal.DO, Animal.BACK_DO, Animal.DO, Animal.BACK_DO, Animal.DO, Animal.BACK_DO, Animal.DO, Animal.BACK_DO, Animal.DO };
    ///int cnt = 0;
    void ThrowToData()
    {
        
        GameData.AddAnimal(_curRandomChoice);
        /*
        if (GameData.CurTurn == PLAYER_KIND.PLAYER_1)
        {
            ++cnt;
            if (cnt > 1)
                _isOut = true;
            
            GameData.AddAnimal(Animal.BACK_DO);
            return;
        }*/

        /*
        if (GameData.CurTurn == PLAYER_KIND.PLAYER_1)
        {
            if (_tempanimalQueue.Count > 0)
            {
                GameData.AddAnimal(_tempanimalQueue[0]);
                _tempanimalQueue.RemoveAt(0);

                return;
            }
        }
        */

        /*
        if(_uiGauge.value > 0.4 && _uiGauge.value < 0.6)
        {
            if (_uiGauge.value > 0.45 && _uiGauge.value < 0.55)
            {
                GameData.AddAnimal(_curRandomChoice);
            }   
            else
            {
                int rr = Random.Range(0, 2);
                if(rr == 0)
                {
                    GameData.AddAnimal(_curRandomChoice);
                }
                else
                {
                    rr = Random.Range(1, _animalProbability[_animalProbability.Count - 1]);
                    for (int i = 0; i < _animalProbability.Count; ++i)
                    {
                        if (_animalProbability[i] > rr)
                        {
                            GameData.AddAnimal((Animal)i);
                            ///Debug.Log(((Animal)i).ToString());
                            break;
                        }
                    }
                }
            }   
        }
        else
        {
            int rr = Random.Range(1, _animalProbability[_animalProbability.Count - 1]);
            for (int i = 0; i < _animalProbability.Count; ++i)
            {
                if (_animalProbability[i] > rr)
                {
                    GameData.AddAnimal((Animal)i);
                    ///Debug.Log(((Animal)i).ToString());
                    break;
                }
            }
        }
        

        

        if (!_isPriorityMode)
        {
            _outOffset += 5;
            int outResult = Random.Range(0, 10000);
            if (OUT + _outOffset > outResult)
            {
                Debug.Log("OUT !!!");
                _isOut = true;
                return;
            }
        }
         * */
    }


    string [] uootSounds = new string[(int)UootNori.Animal.MAX] {"Do0","Gae0","Gul0","Yut0","Mo0","Back0"};
    void UootThrowAni()
    {   
        int aninum = Random.Range(1, 7);

        if (_isOut)
        {
            aninum = Random.Range(11, 17);
        }

        string voiceDefaultPath = "sound0/voice/";
        string YutResult = "_YutResult_";


        UootThrow.s_uootAni.SetInteger("state", 0);
        List<Container> uootThrowFlow = new List<Container>();
        uootThrowFlow.Add(new UootUpSideTurn());
        uootThrowFlow.Add(new PatternSystem.Timer(null, 0.3f));
        uootThrowFlow.Add(new UootThrowPlayer(aninum));
        uootThrowFlow.Add(new PatternSystem.Timer(null, 2.9f));
        if(!_isOut) uootThrowFlow.Add(new UootThrowResultRefresh());
        uootThrowFlow.Add(new PatternSystem.Timer(null, 1.0f));
        string sound;
        if (!_isPriorityMode)
        {
            if (
                _isOut)sound = voiceDefaultPath + 
                    _voiceKeyword[(int)InputManager.Instance.CurPlayer]+"/"+
                    "Voice"+
                    _voiceKeyword[(int)InputManager.Instance.CurPlayer] + 
                    YutResult + 
                    "Nak0";
            else
                sound = voiceDefaultPath + 
                    _voiceKeyword[(int)InputManager.Instance.CurPlayer]+"/"+
                    "Voice"+
                    _voiceKeyword[(int)InputManager.Instance.CurPlayer] + 
                    YutResult +
                    uootSounds[(int)GameData.GetLastAnimal()];
            uootThrowFlow.Add(new SoundPlay(sound));
        }


        uootThrowFlow.Add(new UootCollect());
        uootThrowFlow.Add(new PatternSystem.Timer(null, 3.0f));
        _aniArrange = new PatternSystem.Arrange(null, PatternSystem.Arrange.ArrangeType.SERIES, uootThrowFlow, 1);
    }    
    

    bool UootThrowAniCheck()
    {
        if(_aniArrange.IsDone)
            return true;
        _aniArrange.Run();
        return false;
    }

    public void UootUpsideTurn()
    {
        s_uootAni.gameObject.SetActive(false);

        Animal animal;
        if (_isPriorityMode)
        {
            animal = _animalQueue.Dequeue();
        }
        else
        {
            animal = GameData.GetLastAnimal();
        }

        _uootAnimaion[(int)animal]();
    }

    public override void Event(KeyEvent key)
    {
        if (!gameObject.active)
            return;

        switch (key)
        {
            case KeyEvent.ENTER_EVENT:
                if (_curStep != ThrowCheck)
                {
                    _curThrowStanbyTime = 0.0f;
                    _curStep = ThrowCheck;
                    if (!_isPriorityMode)
                    {
                        AnimalProbabiley();
                        ThrowToData();
                    }

                    UootAniInit();
                    NextTurnCheck.Instance.ArrowVisible(false);
                }
                break;
                /*
            case KeyEvent.DO_EVENT:
                if (_curStep != ThrowCheck)
                {
                    _curThrowStanbyTime = 0.0f;
                    _curStep = ThrowCheck;
                    if (!_isPriorityMode)
                    {
                        GameData.AddAnimal(Animal.DO);
                    }

                    UootAniInit();
                    NextTurnCheck.Instance.ArrowVisible(false);
                }
                break;
            case KeyEvent.GE_EVENT:
                if (_curStep != ThrowCheck)
                {
                    _curThrowStanbyTime = 0.0f;
                    _curStep = ThrowCheck;
                    if (!_isPriorityMode)
                    {
                        GameData.AddAnimal(Animal.GE);
                    }

                    UootAniInit();
                    NextTurnCheck.Instance.ArrowVisible(false);
                }
                break;
            case KeyEvent.KUL_EVENT:
                if (_curStep != ThrowCheck)
                {
                    _curThrowStanbyTime = 0.0f;
                    _curStep = ThrowCheck;
                    if (!_isPriorityMode)
                    {
                        GameData.AddAnimal(Animal.KUL);
                    }

                    UootAniInit();
                    NextTurnCheck.Instance.ArrowVisible(false);
                }
                break;
            case KeyEvent.YUT_EVENT:
                if (_curStep != ThrowCheck)
                {
                    _curThrowStanbyTime = 0.0f;
                    _curStep = ThrowCheck;
                    if (!_isPriorityMode)
                    {
                        GameData.AddAnimal(Animal.UOOT);
                    }

                    UootAniInit();
                    NextTurnCheck.Instance.ArrowVisible(false);
                }
                break;
            case KeyEvent.MO_EVENT:
                if (_curStep != ThrowCheck)
                {
                    _curThrowStanbyTime = 0.0f;
                    _curStep = ThrowCheck;
                    if (!_isPriorityMode)
                    {
                        GameData.AddAnimal(Animal.MO);
                    }

                    UootAniInit();
                    NextTurnCheck.Instance.ArrowVisible(false);
                }
                break;
            case KeyEvent.BACKDO_EVENT:
                if (_curStep != ThrowCheck)
                {
                    _curThrowStanbyTime = 0.0f;
                    _curStep = ThrowCheck;
                    if (!_isPriorityMode)
                    {
                        GameData.AddAnimal(Animal.BACK_DO);
                    }

                    UootAniInit();
                    NextTurnCheck.Instance.ArrowVisible(false);
                }
                break;         
                 * */
        }
    }
}
