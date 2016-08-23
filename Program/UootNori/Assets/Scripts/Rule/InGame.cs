using UnityEngine;
using System.Collections;
using UootNori;
using FlowContainer;


public class InGame : Arrange
{
    GameObject gamePlay;
    GameObject creditCount;
	protected override void OnEnable()
    {
        if(gamePlay == null)
        { 
            gamePlay = GameObject.Find("UI Root").transform.FindChild("Size").FindChild("GamePlay").gameObject;
            creditCount = GameObject.Find("UI Root").transform.FindChild("Size").FindChild("Credit_Group_P").gameObject;
        }
            
        gamePlay.SetActive(true);
        creditCount.SetActive(true);
        
        base.OnEnable();
    }

    void OnDisable()
    {
        if (gamePlay == null)
        {
            gamePlay = GameObject.Find("UI Root").transform.FindChild("Size").FindChild("GamePlay").gameObject;
            creditCount = GameObject.Find("UI Root").transform.FindChild("Size").FindChild("Credit_Group_P").gameObject;
        }
            
        gamePlay.SetActive(false);
        creditCount.SetActive(false);
    }
}
