using UnityEngine;
using System.Collections;


namespace PatternSystem
{
    public class ScaleAgent : PhysicalAgent
    {
        public const string DBType = "Scale";
        public ScaleAgent()
        {
            
        }

		public override Container GetContainer(GameObject target)
		{
			return new Scale(target, _value, _time, _type);
		}
    }
}
