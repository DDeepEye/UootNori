using UnityEngine;
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

    public const int DO = 1536;
    public const int GE = 3456;
    public const int GUL = 3456;
    public const int UOOT = 1296;
    public const int MO = 256;
    public const int BACK_DO = 512;

    public const int OUT = 512;

    private bool _isOut = false;
    private bool _isPriorityMode = true;
   

    List<int> _animalProbability = new List<int>();
    int [] _probabilityOffset = new int[(int)Animal.MAX];
    public static Animator s_uootAni;
    GameObject _uootAniObj;
    int _curAniIndex;

    const int UOOT_NUM = 4;
    GameObject [] _uoots = new GameObject[UOOT_NUM];
    
    delegate void UootAnimaion();
    UootAnimaion[] _uootAnimaion = new UootAnimaion[(int)UootNori.Animal.MAX];

    PatternSystem.Arrange _aniArrange;

    static UootThrow s_inst = null;

    const float THROW_STANBY_TIME = 10.0f;
    float _curThrowStanbyTime = 0.0f;

    delegate void Step();
    Step _curStep = null;

    static public UootThrow GetInstance()
    {
        if (s_inst == null)
        {
            s_inst = GameObject.Find("Flow").transform.FindChild("GameFlow").FindChild("InGame").FindChild("InGameFlow").FindChild("GamePlay").FindChild("UootThrow").GetComponent<UootThrow>();
        }
        return s_inst;
    }

    public void ResetGame()
    {
        _isPriorityMode = true;
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
        if (_curThrowStanbyTime < THROW_STANBY_TIME && !_isPriorityMode)
        {
            _curThrowStanbyTime += Time.deltaTime;
        }
        else
        {
            _curThrowStanbyTime = 0.0f;
            _curStep = ThrowCheck;
            if (!_isPriorityMode)
            {
                AnimalProbabiley();
                ThrowToData();
            }
            UootAniInit();
        }
    }

    void ThrowCheck()
    {
        if (UootThrowAniCheck())
        {   
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
                        InputManager.Instance.CurPlayer = InputManager.Instance._resetPlayer;
                        GameData.s_IsNotControlChange = true;
                    }
                    else
                    {
                        _curStep = ThrowStanbyCheck;
                        InputManager.Instance.CurPlayer = InputManager.Instance._resetPlayer + 1;
                    }
                    if(GameData.CurTurn == PLAYER_KIND.PLAYER_1)
                        SoundPlayer.Instance.Play("sound0/voice/no/VoiceNo_Turn0");
                    else
                        SoundPlayer.Instance.Play("sound0/voice/ki/VoiceKi_Turn0");
                    _isPriorityMode = false;
                    SoundPlayer.Instance.BGMPlay("sound0/bgm/bgm05");
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
                AnimalProbabiley();
                ThrowToData();
                UootAniInit();
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
            if(GameData.CurTurn == PLAYER_KIND.PLAYER_1)
                SoundPlayer.Instance.Play("sound0/voice/no/VoiceNo_Turn0");
            else
                SoundPlayer.Instance.Play("sound0/voice/ki/VoiceKi_Turn0");
        }

        _curStep = ThrowStanbyCheck;

        InputManager.Instance.InputAttribute = this;
        NextTurnCheck.Instance.ArrowVisible(true);
    }

    void OnDisable()
    {
    }

    void AnimalProbabiley()
    {
        _animalProbability.Clear();
        int prob = DO;
        _animalProbability.Add(prob+_probabilityOffset[0]);
        _animalProbability.Add(prob+=GE+_probabilityOffset[1]);
        _animalProbability.Add(prob+=GUL+_probabilityOffset[2]);
        _animalProbability.Add(prob+=UOOT+_probabilityOffset[3]);
        _animalProbability.Add(prob+=MO+_probabilityOffset[4]);
        _animalProbability.Add(prob+=BACK_DO+_probabilityOffset[5]);
    }


    List<Animal> _tempanimalQueue = new List<Animal>() { Animal.BACK_DO, Animal.DO, Animal.DO, Animal.DO };
    ///int cnt = 0;
    void ThrowToData()
    {

        _isOut = false;        
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
        if (_tempanimalQueue.Count > 0)
        {
            GameData.AddAnimal(_tempanimalQueue[0]);
            _tempanimalQueue.RemoveAt(0);

            return;
        }*/


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

        if (!_isPriorityMode)
        {
            int outResult = Random.Range(0, 10000);
            if (OUT > outResult)
            {
                Debug.Log("OUT !!!");
                _isOut = true;
                return;
            }
        }
    }

    void UootThrowAni()
    {   
        int aninum = Random.Range(1, 7);

        if (_isOut)
        {
            aninum = Random.Range(11, 17);
        }

        UootThrow.s_uootAni.SetInteger("state", 0);
        List<Container> uootThrowFlow = new List<Container>();
        uootThrowFlow.Add(new UootUpSideTurn());
        uootThrowFlow.Add(new PatternSystem.Timer(null, 0.3f));
        uootThrowFlow.Add(new UootThrowPlayer(aninum));
        uootThrowFlow.Add(new PatternSystem.Timer(null, 2.9f));
        if(!_isOut) uootThrowFlow.Add(new UootThrowResultRefresh());
        uootThrowFlow.Add(new PatternSystem.Timer(null, 1.0f));
        if(GameData.GetLastAnimal() == Animal.MO) uootThrowFlow.Add(new SoundPlay("sound0/effect/Item_MoOrNak_Use"));
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

        if (key == KeyEvent.ENTER_EVENT)
        {
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
        }
    }
}
