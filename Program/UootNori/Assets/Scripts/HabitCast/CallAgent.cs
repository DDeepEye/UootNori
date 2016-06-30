using UnityEngine;
using System.Collections;

namespace PatternSystem
{
    public class CallAgent : AttributeAgent
    {
        
        public string _sendMessage = "";
        public CallAgent()
        {
            
        }
        public override Container GetContainer(GameObject target)
		{
			Caller p = new Caller (target);
			return p;
		}
    }
}
