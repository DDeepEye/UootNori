using UnityEngine;
using System.Collections;

namespace PatternSystem
{
    public class TimerAgent : AttributeAgent
    {
        public float _time = 0.0f;

        public override Container GetContainer(GameObject target)
		{
			return new Timer(target, _time);
		}
    }


}

