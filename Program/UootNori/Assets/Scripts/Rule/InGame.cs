using UnityEngine;
using System.Collections;
using UootNori;
using FlowContainer;


public class InGame : Arrange
{
    GameObject gamePlay;
	protected override void OnEnable()
    {
        if(gamePlay == null)
        { 
            gamePlay = GameObject.Find("UI Root").transform.FindChild("Size").FindChild("GamePlay").gameObject;
        }
            
        gamePlay.SetActive(true);

        ///InputManager.Instance.CurPlayer = PlayerControl.Player1;
        
        base.OnEnable();
    }

    void OnDisable()
    {
        if (gamePlay == null)
        {
            gamePlay = GameObject.Find("UI Root").transform.FindChild("Size").FindChild("GamePlay").gameObject;
        }
            
        gamePlay.SetActive(false);
    }
}
