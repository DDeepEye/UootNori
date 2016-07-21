using UnityEngine;
using System.Collections;
using FlowContainer;
using UootNori;

public class GameOverCheck : Attribute {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (IsDone)
            return;
        _isDone = true;
        for(int i = 0; i < GameData.s_players.Length; ++i)
        {
            if (GameData.PIECESMAX == GameData.s_players[i].GetGoalInNum())
            {
                Attribute at = transform.parent.GetComponent<Attribute>();
                at.ReturnActive = "Result";
            }
        }
        
	}
}
