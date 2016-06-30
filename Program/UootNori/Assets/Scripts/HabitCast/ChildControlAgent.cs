using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace PatternSystem
{
    
    public class ChildControlAgent : AttributeAgent 
    {
        public override Container GetContainer(GameObject target)
        {
            if (transform.childCount == 0)
                return null;

            List<GameObject> children = new List<GameObject>();
            for (int i = 0; i < transform.childCount; ++i)
            {
                children.Add(transform.GetChild(i).gameObject);
                transform.GetChild(i).gameObject.SetActive(false);
            }
            
            return new ChildContainer(children);
        }
    }

}

