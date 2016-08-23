using UnityEngine;
using System.Collections;

namespace FlowContainer
{
    public class Arrange : Attribute {
        protected int _curActive = 0;

        // Use this for initialization
        protected virtual void Awake () {
            /*
            if (transform.childCount > 0)
            {
                transform.GetChild(_curActive).gameObject.SetActive(true);
            }
             * */
        }

        protected virtual void OnEnable()
        {
            if (transform.childCount > 0)
                transform.GetChild(_curActive).gameObject.SetActive(true);
        }

        // Update is called once per frame
        protected virtual void Update () 
        {
            ActiveCheck();
        }

        public virtual void ActiveCheck()
        {
            if (!IsDone)
            {
                if (_curActive >= 0)
                {   
                    int doneCount = 0;
                    Attribute [] atts = transform.GetChild(_curActive).GetComponents<Attribute>();
                    foreach (Attribute a in atts)
                    {
                        if (a.IsDone)
                            ++doneCount;
                    }

                    if (doneCount == atts.Length)
                    {
                        transform.GetChild(_curActive).gameObject.SetActive(false);
                        for (int i = 0; i < atts.Length; ++i)
                        {
                            atts[i].Reset();
                        }
                        if (ReturnActive.Length > 0)
                        {
                            for (int i = 0; i < transform.childCount; ++i)
                            {
                                if (transform.GetChild(i).name == ReturnActive)
                                {
                                    transform.GetChild(i).gameObject.SetActive(true);
                                    _curActive = i;
                                    _returnActive = "";
                                    return;
                                }
                            }

                            if(transform.parent != null)
                            {
                                Attribute parentAtt = transform.parent.GetComponent<Attribute>();
                                if (parentAtt != null)
                                {
                                    parentAtt.ReturnActive = ReturnActive;
                                    _isDone = true;
                                }
                                    
                            }
                            _returnActive = "";
                        }
                        else
                        {
                            ++_curActive;
                            if (transform.childCount == _curActive)
                            {   
                                _isDone = true;
                            }
                            else
                            {
                                transform.GetChild(_curActive).gameObject.SetActive(true);
                            }
                        }
                    }
                }
            }
        }

        public override void Reset()
        {
            base.Reset();
            for (int i = 0; i < transform.childCount; ++i)
            {
                Attribute [] atts = transform.GetChild(i).GetComponents<Attribute>();
                if (atts.Length > 0)
                {
                    _curActive = 0;
                    for (int j = 0; j < atts.Length; ++j)
                    {
                        atts[j].Reset();
                    }
                }
            }
        }
    }
}

