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

    List<int> _animalProbability = new List<int>();
    int [] _probabilityOffset = new int[(int)Animal.MAX];

	// Use this for initialization
	void Start () {
	
	}
	
    float _curTime;
	// Update is called once per frame
	void Update () {


        if (_isDone)
            return;

        if (_curTime < .2f)
        {
            _curTime += Time.deltaTime;
        }
        else
        {
            _curTime = 0.0f;
            if (UootThrowAniCheck())
            {
                if(_isOut)
                {
                    _isDone = true;
                    Attribute at = transform.parent.GetComponent<Attribute>();
                    at.ReturnActive = "NextTurn";
                    GameData.TurnRollBack();
                    return;
                }

                if (GameData.GetLastAnimal() == Animal.UOOT || GameData.GetLastAnimal() == Animal.MO)
                {
                    AnimalProbabiley();
                    ThrowToData();
                    UootThrowAni();
                }
                else
                {
                    _isDone = true;
                    Attribute at = transform.parent.GetComponent<Attribute>();
                    at.ReturnActive = "";
                }
            }
        }
	}


    void OnEnable()
    {
        AnimalProbabiley();
        ThrowToData();
        UootThrowAni();
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

    void ThrowToData()
    {
        _isOut = false;
        int rr = Random.Range(1, _animalProbability[_animalProbability.Count - 1]);
        for (int i = 0; i < _animalProbability.Count; ++i)
        {
            if (_animalProbability[i] > rr)
            {
                GameData._curAnimals.Add((Animal)i);
                ///Debug.Log(((Animal)i).ToString());
                break;
            }
        }

        int outResult = Random.Range(0, 10000);
        if (OUT > outResult)
        {
            Debug.Log("OUT !!!");
            _isOut = true;
            return;
        }
    }

    void UootThrowAni()
    {
    }

    bool UootThrowAniCheck()
    {
        return true;
    }


}
