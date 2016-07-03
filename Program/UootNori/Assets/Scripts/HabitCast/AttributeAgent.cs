using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PatternSystem
{
    abstract public class AttributeAgent : MonoBehaviour {

        protected static System.Type[] s_attributeTypes = {
                                                               typeof(ArrangeAgent),
                                                               typeof(CallAgent),
                                                               typeof(TimerAgent),
                                                               typeof(MoveAgent),
                                                               typeof(RotationAgent),
                                                               typeof(OrbitAgent),
                                                               typeof(ScaleAgent),
                                                           };

        public static List<AttributeAgent> CollectAttribute(Transform target)
		{
            List<AttributeAgent> attributes = new List<AttributeAgent> ();
            int cnt = target.childCount;
			for (int i = 0; i < cnt; ++i)
			{
                Transform t = target.GetChild(i);
                for(int j = 0; j < s_attributeTypes.Length; ++j)
                {
                    AttributeAgent attribute = t.GetComponent(s_attributeTypes[j]) as AttributeAgent;
                    if (attribute != null)
                    {
                        attributes.Add(attribute);
                        break;
                    }
                }
			}
            return attributes;
		}


        public abstract Container GetContainer(GameObject target);
	}
}
