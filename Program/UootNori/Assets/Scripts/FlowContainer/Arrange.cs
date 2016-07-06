using UnityEngine;
using System.Collections;

namespace FlowContainer
{
    public class Arrange : Attribute {
        int _curActive = -1;

        // Use this for initialization
        void Start () {
            for (int i = 0; i < transform.childCount; ++i)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        // Update is called once per frame
        void Update () 
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
                        ++_curActive;
                        if (transform.childCount == _curActive)
                        {
                            if (ReturnActive.Length > 0)
                            {
                                for (int i = 0; i < transform.childCount; ++i)
                                {
                                    if (transform.GetChild(i).name == ReturnActive)
                                    {
                                        transform.GetChild(i).gameObject.SetActive(true);
                                        _curActive = i;
                                        break;
                                    }
                                }
                                _returnActive = "";
                                return;
                            }
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

        void OnEnable()
        {
            if (transform.childCount > 0)
            {
                Attribute [] atts = transform.GetChild(0).GetComponents<Attribute>();
                if (atts.Length > 0)
                {
                    _curActive = 0;
                    transform.GetChild(0).gameObject.SetActive(true);;
                }
            }
        }

        void OnDisable()
        {
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

