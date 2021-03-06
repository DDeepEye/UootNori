﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FlowContainer;

namespace UootNori
{
    public enum PlayerControl
    {
        Player1,
        Player2,
        Player3,
        Player4,
        MAX,
    }

    public enum KeyEvent
    {
        LEFT_EVENT,
        RIGHT_EVENT,
        ENTER_EVENT,

        DO_EVENT,
        GE_EVENT,
        KUL_EVENT,
        YUT_EVENT,
        MO_EVENT,
        BACKDO_EVENT,
    }

    public class InputManager : MonoBehaviour 
    {
        static InputManager s_instance;
        static public InputManager Instance
        {
            get
            {
                if (s_instance == null)
                    s_instance = GameObject.Find("InputManager").GetComponent<InputManager>();
                return s_instance;
            }
        }


        Dictionary<PlayerControl, InputControler> _playerControls = new Dictionary<PlayerControl, InputControler>();

        PlayerControl _curPlayer = PlayerControl.Player1;
        public PlayerControl _maxControlNum = PlayerControl.MAX;
        public PlayerControl CurPlayer{get{ return _curPlayer;}set{ _curPlayer = value;}}

        FlowContainer.Attribute _inputAttribute;
        public FlowContainer.Attribute InputAttribute {get{ return _inputAttribute;}set{ _inputAttribute = value;}}
        FlowContainer.Attribute _backupInputAttribute;

        public bool _controlChoiceMode = false;
        public PlayerControl _resetPlayer = PlayerControl.Player1;

        Dictionary<PLAYER_KIND, List<InputControler>> _inputControls = new Dictionary<PLAYER_KIND, List<InputControler>>();

        public PlayerControl ResetPlayer 
        {
            get
            {
                return _resetPlayer;
            }
            set
            {
                _resetPlayer = value;
                if (_resetPlayer <= PlayerControl.Player2)
                {
                    NextTurnCheck.Instance.intactlyCamera();
                }
                else
                {
                    NextTurnCheck.Instance.reverseCamera();
                }
            }
        }

        Dictionary<string, KeyEvent> _keys = new Dictionary<string, KeyEvent>()
        {
            { "left", KeyEvent.LEFT_EVENT },
            { "right", KeyEvent.RIGHT_EVENT },
            { "enter", KeyEvent.ENTER_EVENT },
        };        

        public void Next()
        {
            ++_curPlayer;
            if (_curPlayer == _maxControlNum)
                _curPlayer = ResetPlayer;
        }

        public void SetPlayerNum(PlayerControl playerNum)
        {
            
            _inputControls.Clear();
            List<InputControler> keies1 = new List<InputControler>();
            List<InputControler> keies2 = new List<InputControler>();

            _inputControls.Add(PLAYER_KIND.PLAYER_1, keies1);
            _inputControls.Add(PLAYER_KIND.PLAYER_2, keies2);

            Dictionary<PlayerControl, Dictionary<KeyCode, string>> playerControlKeys = new Dictionary<PlayerControl, Dictionary<KeyCode, string>>();
            
            Dictionary<KeyCode, string> keys = new Dictionary<KeyCode, string>() 
            { 
                {KeyCode.Q,"left"},
                {KeyCode.W,"right"},
                {KeyCode.E,"enter"} 
            };
            playerControlKeys.Add(PlayerControl.Player1, keys);            

            keys = new Dictionary<KeyCode, string>() 
            { 
                {KeyCode.S,"left"},
                {KeyCode.D,"right"},
                {KeyCode.Z,"enter"} 
            };
            playerControlKeys.Add(PlayerControl.Player2, keys);            

            keys = new Dictionary<KeyCode, string>() 
            { 
                {KeyCode.K,"left"},
                {KeyCode.B,"right"},
                {KeyCode.L,"enter"} 
            };
            playerControlKeys.Add(PlayerControl.Player3, keys);            

            keys = new Dictionary<KeyCode, string>() 
            { 
                {KeyCode.I,"left"},
                {KeyCode.P,"right"},
                {KeyCode.O,"enter"} 
            };
            playerControlKeys.Add(PlayerControl.Player4, keys);            

            _playerControls.Clear();

            for (PlayerControl pc = PlayerControl.Player1; pc <= playerNum; ++pc)
            {
                InputControler ic = new InputControler(playerControlKeys[pc]);
                _playerControls.Add(pc, ic);
                List<InputControler> keies = ((int)pc % 2 == 0) ? keies1 : keies2;
                keies.Add(ic);
            }
            _maxControlNum = playerNum;
            ++_maxControlNum;

            

        }
        // Use this for initialization
        void Start () {

        }

        // Update is called once per frame
        void Update ()
        {
            if (_controlChoiceMode && InputAttribute != Calculate.Instance)
            {
                if (GameData.GetCreditNum() > 0)
                {
                    for (PlayerControl i = 0; i < PlayerControl.MAX; ++i)
                    {
                        string keyDown = _playerControls[i].Update();
                        if (keyDown != null)
                        {
                            CurPlayer = (PlayerControl)i;
                            if (i >= PlayerControl.Player3)
                            {
                                ResetPlayer = PlayerControl.Player3;
                                _maxControlNum = PlayerControl.MAX;
                            }
                            else
                            {
                                ResetPlayer = PlayerControl.Player1;
                                _maxControlNum = PlayerControl.Player3;
                            }
                            _controlChoiceMode = false;
                            if (InputAttribute != null)
                                InputAttribute.Event(_keys[keyDown]);
                        }
                    }
                }
            }
            else
            {
                if (InputAttribute == Calculate.Instance)
                {
                    foreach (KeyValuePair<PlayerControl, InputControler> c in _playerControls)
                    {
                        string keyDown = c.Value.Update();
                        if (keyDown != null)
                        {
                            if (InputAttribute != null)
                                InputAttribute.Event(_keys[keyDown]);
                        }
                    }
                    
                }
                else if (_playerControls.ContainsKey(_curPlayer))
                {
                    string keyDown = _playerControls[_curPlayer].Update();
                    if (GameData._is4p)
                    {
                        List<InputControler> keies = _inputControls[GameData.CurTurn];
                        for (int i = 0; i < keies.Count; ++i)
                        {
                            keyDown = keies[i].Update();
                            if (keyDown != null)
                            {
                                if (InputAttribute != null)
                                    InputAttribute.Event(_keys[keyDown]);
                                break;
                            }
                        }
                    }
                    else
                    {
                        keyDown = _playerControls[_curPlayer].Update();
                        if (keyDown != null)
                        {
                            if (InputAttribute != null)
                                InputAttribute.Event(_keys[keyDown]);
                        }
                    }
                }
            }


            if(Input.GetKeyUp(KeyCode.C))
            {
                if (InputAttribute != Calculate.Instance)
                {
                    GameObject.Find("UI Root").transform.FindChild("Size").FindChild("Calculate").gameObject.SetActive(true);
                    _backupInputAttribute = InputAttribute;
                    InputAttribute = Calculate.Instance;
                }
                else
                {
                    InputAttribute = _backupInputAttribute;
                    GameObject.Find("UI Root").transform.FindChild("Size").FindChild("Calculate").gameObject.SetActive(false);
                }
            }

            if(Input.GetKeyUp(KeyCode.X))
            {
                GameData.AddCredit();
                Title.Instance.OnCredit();

                if (GameData.s_isDemo)
                {
                    GameData.s_isDemo = false;
                    InputAttribute.IsDone = true;
                    InputAttribute.transform.parent.GetComponent<Attribute>().ReturnActive = "Title";
                    GameData.ReSetGame(false);
                }
            }

            KeyCode[] kcs = new KeyCode[]{ KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, };
            KeyEvent[] kes = new KeyEvent[]{ KeyEvent.DO_EVENT, KeyEvent.GE_EVENT,KeyEvent.KUL_EVENT,KeyEvent.YUT_EVENT,KeyEvent.MO_EVENT,KeyEvent.BACKDO_EVENT,};
            for (int i = 0; i < kcs.Length; ++i)
            {
                if (Input.GetKeyUp(kcs[i]))
                {
                    if (InputAttribute != null)
                        InputAttribute.Event(kes[i]);
                }
            }
        }
    }
}

