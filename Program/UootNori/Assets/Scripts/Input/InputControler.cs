using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputControler
{
    Dictionary<KeyCode, string> _checkKeys = new Dictionary<KeyCode, string>();
    public InputControler(Dictionary<KeyCode, string> checkKeys)
    {
        _checkKeys = checkKeys;
    }
	
	public string Update () 
    {
        foreach (KeyValuePair<KeyCode, string> key in _checkKeys)
        {
            if (Input.GetKey(key.Key))
                return key.Value;
        }
        return "";
	}
}
