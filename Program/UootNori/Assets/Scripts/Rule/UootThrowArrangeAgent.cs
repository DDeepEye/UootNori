using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PatternSystem;

namespace UootNori
{
    public class UootThrowArrangeAgent : ArrangeAgent
    {
        public override Container GetContainer(GameObject target)
        {
            List<Container> containers = new List<Container>();
            List<AttributeAgent> attributes = AttributeAgent.CollectAttribute(gameObject.transform);
            foreach (AttributeAgent att in attributes)
            {
                containers.Add(att.GetContainer(target));
            }
            return new UootThrowArrange(_type, containers, _repeat);
        }
    }
}
