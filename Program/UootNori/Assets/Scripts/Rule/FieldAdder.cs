using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldAdder : MonoBehaviour {

    static Dictionary<int, GameObject> s_fields = new Dictionary<int, GameObject>();
    public int _fieldNumber;
	// Use this for initialization
    void Awake()
    {
        if (!s_fields.ContainsKey(_fieldNumber))
            s_fields.Add(_fieldNumber, gameObject);
    }
	void Start () {
        
	}

    public static GameObject GetFields(int key)
    {
        if(s_fields.ContainsKey(key))
            return s_fields[key];
        return null;
    }
}
