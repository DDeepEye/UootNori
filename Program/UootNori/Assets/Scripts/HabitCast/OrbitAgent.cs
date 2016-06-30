using UnityEngine;
using System.Collections;

namespace PatternSystem
{
    public class OrbitAgent : PhysicalAgent
    {
        public const string DBType = "Orbit";
        public OrbitAgent()
        {
            
        }

		public override Container GetContainer(GameObject target)
		{
			return new Orbit(target, _value, _time, _type);
		}
    }
}
