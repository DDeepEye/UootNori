using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UootNori;
using PatternSystem;

public class Initialize : MonoBehaviour {

    Arrange road;
	// Use this for initialization
	void Start () {
        GameData.Init();
        GameObject origin_pieces = Resources.Load("Uoot_N") as GameObject;
        GameObject pieces = GameObject.Instantiate(origin_pieces);
        pieces.transform.position = GameData.GetExitField().GetSelfField().transform.position;

        List<Vector3> points = GameData.GetWay(0);
        List<Container> containers = new List<Container>();
        containers.Add(new Timer(pieces, 1.0f));
        Vector3 offsetPoint = pieces.transform.position;
        foreach (Vector3 point in points)
        {
            Vector3 p = point - offsetPoint;
            offsetPoint = point;
            containers.Add(new Timer(pieces, 0.1f));
            containers.Add(new Move(pieces, p, 0.15f));
        }

        road = new Arrange(pieces, Arrange.ArrangeType.SERIES, containers, 0);
	}
	
	// Update is called once per frame
	void Update () {
        road.Run();
	}
}
