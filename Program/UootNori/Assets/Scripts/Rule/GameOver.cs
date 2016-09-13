using UnityEngine;
using System.Collections;
using FlowContainer;
using UootNori;

public class GameOver : Attribute
{
    GameObject _gameOver;
    float _curTime;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (IsDone)
            return;

        _curTime += Time.deltaTime;
        if(_curTime > 4.0f)
        {
            _isDone = true;
            _gameOver.SetActive(false);
            transform.parent.GetComponent<Attribute>().ReturnActive = "Title";
        }
	}

    void OnEnable()
    {
        if (_gameOver == null)
        {
            _gameOver = GameObject.Find("UI Root").transform.FindChild("Size").FindChild("GameOver").gameObject;
        }
        _gameOver.SetActive(true);
        _curTime = 0.0f;
        GameData.StartPointVisible(false);
        SoundPlayer.Instance.Play("sound0/sound/winend");

    }
}
