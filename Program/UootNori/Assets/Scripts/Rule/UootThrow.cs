using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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

    List<int> _animalProbabilty = new List<int>();

	// Use this for initialization
	void Start () {
	
	}
	
    float _curTime;
	// Update is called once per frame
	void Update () {

        if (_isDone)
            return;

        if (_curTime < 3.0f)
        {
            _curTime += Time.deltaTime;
        }
        else
        {
            _curTime = 0.0f;
            _isDone = true;
            transform.parent.GetComponent<Attribute>().ReturnActive = "";
        }

        /*
        if (UootThrowAniCheck())
        {
            _isDone = true;
            Attribute at = transform.parent.GetComponent<Attribute>();
            at.ReturnActive = "";
        }
        */
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
        int prob = DO;
        _animalProbabilty.Add(prob);
        _animalProbabilty.Add(prob+=GE);
        _animalProbabilty.Add(prob+=GUL);
        _animalProbabilty.Add(prob+=UOOT);
        _animalProbabilty.Add(prob+=MO);
        _animalProbabilty.Add(prob+=BACK_DO);
    }

    void ThrowToData()
    {
        int rr = Random.Range(1, _animalProbabilty[_animalProbabilty.Count - 1]);
        for (int i = 0; i < _animalProbabilty.Count; ++i)
        {
            if (_animalProbabilty[i] > rr)
            {
                GameData._curAnimals.Add((Animal)i);
            }
        }

        int outResult = Random.Range(0, 10000);
        if (OUT > outResult)
        {
            _isOut = true;
            GameData.TurnRollBack();
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
