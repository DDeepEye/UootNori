using UnityEngine;
using System.Collections;
using FlowContainer;
using UootNori;

public class Result : Attribute {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnEnable()
    {
        PLAYER_KIND winner = PLAYER_KIND.MAX;
        for (int i = 0; i < GameData.s_players.Length; ++i)
        {
            if (GameData.PIECESMAX == GameData.s_players[i].GetGoalInNum())
            {
                winner = (PLAYER_KIND)i;
                break;
            }
        }
        
        if(winner != PLAYER_KIND.MAX)
            Debug.Log("Game Over!! Winner!! " + winner.ToString());
    }
}
