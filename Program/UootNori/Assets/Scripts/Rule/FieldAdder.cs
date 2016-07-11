using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldAdder : MonoBehaviour {

    static HashSet<GameObject> s_fields = new HashSet<GameObject>();
    public int _fieldNumber;
	// Use this for initialization
	void Start () {
        if (!s_fields.Contains(gameObject))
            s_fields.Add(gameObject);
	}

    static HashSet<GameObject> GetFields()
    {
        return s_fields;
    }
}
