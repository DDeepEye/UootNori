using UnityEngine;
using System.Collections;
using FlowContainer;
using UootNori;

public class Result : Attribute {

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
        }
	}

    void OnEnable()
    {
        _curTime = 0.0f;
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

        PLAYER_KIND loser = winner == PLAYER_KIND.PLAYER_1 ? PLAYER_KIND.PLAYER_2 : PLAYER_KIND.PLAYER_1;
        foreach(PiecesMoveContainer pmc in GameData.GetPiecesMover(loser))
        {
            pmc.Pieces.GetComponent<Animator>().SetInteger("state", 12);
        }
        GameData.VictoryAni(winner);

        Transform gp = GameObject.Find("UI Root").transform.FindChild("Size").FindChild("GamePlay");
        if (winner == PLAYER_KIND.PLAYER_1)
        {
            gp.FindChild("Play01").FindChild("Win_Label_Count_P").GetComponent<UILabel>().text = ((int.Parse(gp.FindChild("Play01").FindChild("Win_Label_Count_P").GetComponent<UILabel>().text)) + 1).ToString();
            gp.FindChild("Play02").FindChild("Lose_Label_Count_P").GetComponent<UILabel>().text = ((int.Parse(gp.FindChild("Play02").FindChild("Lose_Label_Count_P").GetComponent<UILabel>().text)) + 1).ToString();
            
        }
        else
        {
            gp.FindChild("Play02").FindChild("Win_Label_Count_P").GetComponent<UILabel>().text = ((int.Parse(gp.FindChild("Play02").FindChild("Win_Label_Count_P").GetComponent<UILabel>().text)) + 1).ToString();
            gp.FindChild("Play01").FindChild("Lose_Label_Count_P").GetComponent<UILabel>().text = ((int.Parse(gp.FindChild("Play01").FindChild("Lose_Label_Count_P").GetComponent<UILabel>().text)) + 1).ToString();
        }



    }
}
