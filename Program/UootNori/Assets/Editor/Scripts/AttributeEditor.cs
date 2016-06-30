using UnityEngine;
using UnityEditor;
using PatternSystem;
using System.Collections;
using System.Collections.Generic;

namespace PatternSystem
{    
    [CustomEditor(typeof(AttributeAgent))]
	public class AttributeEditor : Editor {

        protected AttributeAgent _attribute;
		void OnEnable () {
            _attribute = target as AttributeAgent;
		}
	}
}


