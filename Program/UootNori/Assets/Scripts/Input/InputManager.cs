using UnityEngine;
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
        public PlayerControl CurPlayer{set{ _curPlayer = value;}}

        FlowContainer.Attribute _inputAttribute;
        public FlowContainer.Attribute InputAttribute {get{ return _inputAttribute;}set{ _inputAttribute = value;}}
        FlowContainer.Attribute _backupInputAttribute;


        Dictionary<string, KeyEvent> _keys = new Dictionary<string, KeyEvent>()
        {
            { "left", KeyEvent.LEFT_EVENT },
            { "right", KeyEvent.RIGHT_EVENT },
            { "enter", KeyEvent.ENTER_EVENT },
        };        

        public void Next()
        {
            ++_curPlayer;
            if (_curPlayer == PlayerControl.MAX)
                _curPlayer = PlayerControl.Player1;
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
                {KeyCode.A,"left"},
                {KeyCode.S,"right"},
                {KeyCode.D,"enter"} 
            };
            playerControlKeys.Add(PlayerControl.Player2, keys);

            keys = new Dictionary<KeyCode, string>() 
            { 
                {KeyCode.I,"left"},
                {KeyCode.O,"right"},
                {KeyCode.P,"enter"} 
            };
            playerControlKeys.Add(PlayerControl.Player3, keys);

            keys = new Dictionary<KeyCode, string>() 
            { 
                {KeyCode.J,"left"},
                {KeyCode.K,"right"},
                {KeyCode.L,"enter"} 
            };
            playerControlKeys.Add(PlayerControl.Player4, keys);

            _playerControls.Clear();

            for (PlayerControl pc = PlayerControl.Player1; pc <= playerNum; ++pc)
            {
                InputControler ic = new InputControler(playerControlKeys[pc]);
                _playerControls.Add(pc, ic);
            }
        }
        // Use this for initialization
        void Start () {

        }

        // Update is called once per frame
        void Update ()
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

            if(Input.GetKeyUp(KeyCode.Z))
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

            if(Input.GetKeyUp(KeyCode.B))
            {
                GameData.AddCredit();
            }
        }
    }
}

