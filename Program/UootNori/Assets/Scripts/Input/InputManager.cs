﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


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
                {KeyCode.O,"right"},
                {KeyCode.P,"enter"} 
            };
            playerControlKeys.Add(PlayerControl.Player4, keys);

            _playerControls.Clear();

            for (PlayerControl pc = PlayerControl.Player1; pc <= playerNum; ++pc)
            {
                InputControler ic = new InputControler(playerControlKeys[pc]);
                _playerControls.Add(pc, ic);
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
                if (_playerControls.ContainsKey(_curPlayer))
                {
                    string keyDown = _playerControls[_curPlayer].Update();
                    if (keyDown != null)
                    {
                        if (InputAttribute != null)
                            InputAttribute.Event(_keys[keyDown]);
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
            }
        }
    }
}

