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
        static public InputManager Instance{get{return s_instance;}}

        Dictionary<PlayerControl, InputControler> _playerControls = new Dictionary<PlayerControl, InputControler>();

        PlayerControl _curPlayer = PlayerControl.Player1;
        public PlayerControl CurPlayer{set{ _curPlayer = value;}}

        Dictionary<string, KeyEvent> _keys = new Dictionary<string, KeyEvent>()
        {
            { "left", KeyEvent.LEFT_EVENT },
            { "right", KeyEvent.RIGHT_EVENT },
            { "enter", KeyEvent.ENTER_EVENT },
        };

        InputManager()
        {
            s_instance = this;
        }

        public void SetPlayerNum(PlayerControl playerNum)
        {
            Dictionary<PlayerControl, Dictionary<KeyCode, string>> playerControlKeys = new Dictionary<PlayerControl, Dictionary<KeyCode, string>>();
            
            Dictionary<KeyCode, string> keys = new Dictionary<KeyCode, string>() 
            { 
                {KeyCode.LeftArrow,"left"},
                {KeyCode.RightArrow,"right"},
                {KeyCode.Return,"enter"} 
            };
            playerControlKeys.Add(PlayerControl.Player1, keys);

            keys = new Dictionary<KeyCode, string>() 
            { 
                {KeyCode.Q,"left"},
                {KeyCode.W,"right"},
                {KeyCode.E,"enter"} 
            };
            playerControlKeys.Add(PlayerControl.Player2, keys);

            keys = new Dictionary<KeyCode, string>() 
            { 
                {KeyCode.R,"left"},
                {KeyCode.T,"right"},
                {KeyCode.Y,"enter"} 
            };
            playerControlKeys.Add(PlayerControl.Player3, keys);

            keys = new Dictionary<KeyCode, string>() 
            { 
                {KeyCode.U,"left"},
                {KeyCode.I,"right"},
                {KeyCode.O,"enter"} 
            };
            playerControlKeys.Add(PlayerControl.Player4, keys);

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
                    InGameControlerManager.Instance.Event(_keys[keyDown]);
                }
            }
        }
    }
}

