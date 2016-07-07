using UnityEngine;
using System.Collections;
using FlowContainer;

public class FlowStart : MonoBehaviour {

	// Use this for initialization
	void Start () {
        if (transform.childCount > 0)
            transform.GetChild(0).gameObject.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
