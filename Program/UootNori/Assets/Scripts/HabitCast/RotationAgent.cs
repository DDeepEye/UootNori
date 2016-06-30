using UnityEngine;
using System.Collections;

namespace PatternSystem
{
    public class RotationAgent : PhysicalAgent
    {
        public const string DBType = "Rotation";
        public RotationAgent()
        {
            
        }

        public override Container GetContainer(GameObject target)
		{
			return new Rotation(target, _value, _time, _type);
		}
    }
}

