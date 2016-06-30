using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PatternSystem
{
    public class TrigerAgent : MonoBehaviour {

  

        public const string DBType = "Triger";

        private int _id = -1;
        public int ID{get{ return _id;} set{ _id = value;}}

        public TrigerAgent()
        {
            
        }


        public List<AttributeAgent> CollectAttribute()
        {
            return AttributeAgent.CollectAttribute(transform);
		}

		public Triger GetTriger(GameObject target)
		{
			List<AttributeAgent> attributes = CollectAttribute();

            List<Container> conditions = new List<Container> ();
			foreach (AttributeAgent att in attributes)
			{
				conditions.Add(att.GetContainer (target));
			}

			return new Triger (name, target, conditions);
		}
	}
}
