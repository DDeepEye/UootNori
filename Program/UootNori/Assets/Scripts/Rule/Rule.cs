using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PatternSystem;
using System;

namespace UootNori
{
    public enum Animal
    {
        NONE = -1,
        DO = 0,
        GE,
        KUL,
        UOOT,
        MO,
        BACK_DO,
        MAX,
    }

    public enum ControlMode
    {
        CharcterMove,
        Shoot,
    }

    public abstract class FieldAttribute
    {
        public abstract void Run(PiecesMoveContainer mover);
    }

    public class Kill : FieldAttribute
    {   
        public override void Run(PiecesMoveContainer mover)
        {
            mover.CurRoad._field.Mover = null;
            GameData.AdjustMover(mover, GameData.MoverAdjustKind.KILL_ONESELF);
            mover.Containers.AddContainer(new CharacterTrap(mover.Pieces));
            mover.Containers.AddContainer(new CharacterFadeOut(mover.Pieces, 1.2f));

            List<GameObject> deleteTarget = new List<GameObject>();
            deleteTarget.Add(mover.Pieces);

            mover.Containers.AddContainer(new CharacterDelete(deleteTarget));
            GameData.FieldInNumToMoverPiecesIsSame(mover.PlayerKind);
        }
    }

    public class AllKill : FieldAttribute
    {
        public override void Run(PiecesMoveContainer mover)
        {
            List<Container> delEffs = new List<Container>();
            PatternSystem.Arrange delEffArrange = new PatternSystem.Arrange(null, Arrange.ArrangeType.PARALLEL, delEffs);
            List<GameObject> deleteTarget = new List<GameObject>();
            for (PLAYER_KIND i = PLAYER_KIND.PLAYER_1; i < PLAYER_KIND.MAX; ++i)
            {   
                deleteTarget.Add(mover.Pieces);

                List<PiecesMoveContainer> movers = GameData.GetPiecesMover(i);
                foreach (PiecesMoveContainer m in movers)
                {   
                    m.CurRoad._field.Mover = null;
                    GameData.s_players[(int)i].Out(m.GetPiecesNum());

                    if (m == mover)
                        continue;

                    deleteTarget.Add(m.Pieces);
                    m.Pieces.GetComponent<Animator>().SetInteger("state", 5);
                    delEffs.Add(new CharacterFadeOut(m.Pieces));
                }
                movers.Clear();
                mover.Containers.AddContainer(new CharacterTrap(mover.Pieces));                
                delEffs.Add(new CharacterFadeOut(mover.Pieces, 1.2f));
                mover.Containers.AddContainer(delEffArrange);

                GameData.FieldInNumToMoverPiecesIsSame(i);
            }

            mover.Containers.AddContainer(new CharacterDelete(deleteTarget));
        }
    }

    public class Send1toSend2 : FieldAttribute
    {
        Road _sendRoad;
        public Send1toSend2(Road sendRoad)
        {
            _sendRoad = sendRoad;
        }
        public override void Run(PiecesMoveContainer mover)
        {   
            Vector3 offsetPoint = mover.Pieces.transform.position;
            Vector3 p = _sendRoad._field.GetSelfField().transform.position - offsetPoint;
            mover.Containers.AddContainer(new Timer(null, 0.1f));

            if (_sendRoad._field.Mover != null)
            {
                mover.Containers.AddContainer(new CharacterFadeOut(_sendRoad._field.Mover.Pieces));
                if (_sendRoad._field.Mover.PlayerKind == mover.PlayerKind)
                {
                    mover.Containers.AddContainer(new CharacterRide(mover.Pieces));
                }
                else
                {
                    mover.Containers.AddContainer(new CharacterAttack(mover.Pieces, _sendRoad._field.Mover.Pieces));
                }
            }
            
            mover.Containers.AddContainer(new JumpingMove(mover.Pieces, p, 0.15f));
            mover.Containers.AddContainer(new Timer(null, 0.1f));
            mover.Containers.AddContainer(new FieldSet(_sendRoad._field, mover));            
            mover.CurRoad._field.Mover = null;
            mover.CurRoad = _sendRoad;
            
            Debug.Log("field run Send1toSend2 !!");
        }
    }

    public class AddPieces : FieldAttribute
    {
        public override void Run(PiecesMoveContainer mover)
        {
            if (GameData.s_players[(int)mover.PlayerKind].GetOutFieldNum() > 0)
            {
                GameData.s_players[(int)mover.PlayerKind].FieldIn(1);
                mover.Add(1);
                GameData.OutPiecessViewRefresh();
                mover.Containers.AddContainer(new CharacterAdd(mover.Pieces));
                mover.Containers.AddContainer(new Timer(null, 1.2f));
                mover.Containers.AddContainer(new CharacterIdle(mover.Pieces));
            }

            GameData.FieldInNumToMoverPiecesIsSame(mover.PlayerKind);
        }
    }

    public class RemovePieces : FieldAttribute
    {
        public override void Run(PiecesMoveContainer mover)
        {
            if (mover.GetPiecesNum() > 1)
            {
                mover.Add(-1);
                GameData.s_players[(int)mover.PlayerKind].Out(1);
                GameData.OutPiecessViewRefresh();
                mover.Containers.AddContainer(new CharacterRemove(mover.Pieces));
                mover.Containers.AddContainer(new Timer(null, 1.2f));
                mover.Containers.AddContainer(new CharacterIdle(mover.Pieces));
            }

            GameData.FieldInNumToMoverPiecesIsSame(mover.PlayerKind);
        }
    }

    public class OneMoreThrow : FieldAttribute
    {
        public override void Run(PiecesMoveContainer mover)
        {
            GameData.OneMoreUootThrow();
        }
    }

    public class Shoot: FieldAttribute
    {
        public override void Run(PiecesMoveContainer mover)
        {
            InGameControlerManager.Instance.ReadyToShootMode();
            GameData.ShootMode();
        }
    }

    public class ChangeWay : FieldAttribute
    {
        private int _wayKind;
        public ChangeWay(int wayKind)
        {
            _wayKind = wayKind;
        }
        public override void Run(PiecesMoveContainer mover)
        {
            if (mover == null)
                return;
            mover.CurRoad = GameData.GetWayChangetoRoad(_wayKind, mover.CurRoad);
        }
    }
    public class Exit : FieldAttribute
    {
        public override void Run(PiecesMoveContainer mover)
        {

        }
    }

    public class ExitSchedule : FieldAttribute
    {
        public override void Run(PiecesMoveContainer mover)
        {
            mover._goalinSchedule = true;
        }
    }

    public class GoalIn : Property
    {
        PiecesMoveContainer _destroyMover;
        public GoalIn(PiecesMoveContainer destroyMover)
            : base(null)
        {
            _destroyMover = destroyMover;
        }

        public override void Run()
        {
            GameObject.Destroy(_destroyMover.Pieces);
            GameData.AdjustMover(_destroyMover, GameData.MoverAdjustKind.GOALIN);
            if (GameData.s_players[(int)GameData.CurTurn].GetGoalInNum() >= GameData.PIECESMAX)
                GameData.TurnRollBack();
            
            _isDone = true;
        }
    }

    public class FieldSet : Property
    {
        FieldData _field;
        PiecesMoveContainer _mover;
        public FieldSet(FieldData field, PiecesMoveContainer mover)
            : base(null)
        {
            _field = field;
            _mover = mover;
        }

        public override void Run()
        {
            if (_isDone)
                return;

            _isDone = true;
            
            if (_field.Mover != null)
            {
                if(_field.Mover.PlayerKind == _mover.PlayerKind)
                {
                    _mover.Add(_field.Mover.GetPiecesNum());
                    GameData.AdjustMover(_field.Mover, GameData.MoverAdjustKind.ADD);
                    GameData.FieldInNumToMoverPiecesIsSame(_field.Mover.PlayerKind);
                }
                else
                {
                    GameData.AdjustMover(_field.Mover, GameData.MoverAdjustKind.DESTROY);
                    if (!(_mover._animal == Animal.MO || _mover._animal == Animal.UOOT))
                        GameData.OneMoreUootThrow();
                    GameData.FieldInNumToMoverPiecesIsSame(_field.Mover.PlayerKind);
                }
                GameObject.Destroy(_field.Mover.Pieces);
            }
            else
            {
                PiecesMoveContainer findMover = GameData.InFieldMoverCheck(_mover.CurRoad, 0);
                if(findMover != null)
                {
                    if (findMover.CurRoad._field.Mover.PlayerKind == _mover.PlayerKind)
                    {
                        _mover.Add(findMover.CurRoad._field.Mover.GetPiecesNum());
                        GameData.AdjustMover(findMover.CurRoad._field.Mover, GameData.MoverAdjustKind.ADD);
                        GameData.FieldInNumToMoverPiecesIsSame(_mover.PlayerKind);
                    }
                    else
                    {
                        GameData.AdjustMover(findMover.CurRoad._field.Mover, GameData.MoverAdjustKind.DESTROY);
                        if (!(_mover._animal == Animal.MO || _mover._animal == Animal.UOOT))
                            GameData.OneMoreUootThrow();
                        GameData.FieldInNumToMoverPiecesIsSame(findMover.CurRoad._field.Mover.PlayerKind);
                    }
                    GameObject.Destroy(findMover.CurRoad._field.Mover.Pieces);
                    findMover.CurRoad._field.Mover = null;
                }
            }
            _field.Mover = _mover;
            _field.FieldRun();
            Debug.Log(GameData.CurTurn.ToString() + " FieldIn -> " + GameData.s_players[(int)GameData.CurTurn].GetInFieldNum().ToString() + " FieldOut -> " + GameData.s_players[(int)GameData.CurTurn].GetOutFieldNum().ToString());
        }
    }


    public class UootThrowPlayer : Container
    {
        int _aniNum;
        public UootThrowPlayer(int aniNum)
        {
            _aniNum = aniNum;
        }
        public override void Run()
        {
            if (IsDone)
                return;
            _isDone = true;
            UootThrow.s_uootAni.gameObject.SetActive(true);
            UootThrow.s_uootAni.SetInteger("state", _aniNum);
            GameData.s_animaleEffect.SetActive(false);
            SoundPlayer.Instance.Play("sound0/effect/Yut_Throw");
        }
    }

    public class UootUpSideTurn : Container
    {
        public override void Run()
        {
            if (IsDone)
                return;
            _isDone = true;            
            UootThrow.GetInstance().UootUpsideTurn();
        }
    }

    public class UootThrowResultRefresh : Container
    {
        public override void Run()
        {
            if (IsDone)
                return;

            _isDone = true;

            GameData.RefreshAnimalView();
        }
    }

    public class UootCollect : Container
    {
        public override void Run()
        {
            if (IsDone)
                return;

            _isDone = true;

            UootThrow.s_uootAni.SetInteger("state", 0);
        }
    }


    public abstract class CharacterAni : Container
    {
        protected GameObject _aniObj;
        protected Animator _ani;
        protected int ANI_NUMBER = 1;
        protected CharacterAni(GameObject aniObject)
        {
            _aniObj = aniObject;
            _ani = aniObject.GetComponent<Animator>();
        }
        public override void Run()
        {
            if (IsDone)
                return;

            _isDone = true;
            Debug.Log("ani state, " + ANI_NUMBER.ToString());
            _ani.SetInteger("state", ANI_NUMBER);
            _ani.speed = 2.0f;
        }
    }

    public class CharacterMove : CharacterAni
    {
        public CharacterMove(GameObject aniObject)
            :base(aniObject)
        {
            ANI_NUMBER = 2;
        }
    }

    public class CharacterIdle : CharacterAni
    {
        public CharacterIdle(GameObject aniObject)
            :base(aniObject)
        {
            ANI_NUMBER = 1;
        }
    }

    public class CharacterTrap : CharacterAni
    {
        public CharacterTrap(GameObject aniObject)
            :base(aniObject)
        {
            ANI_NUMBER = 6;
        }
    }

    public class CharacterRide : CharacterAni
    {
        public CharacterRide(GameObject aniObject)
            :base(aniObject)
        {
            ANI_NUMBER = 4;
        }
    }

    public class CharacterStepped : CharacterAni
    {
        public CharacterStepped(GameObject aniObject)
            :base(aniObject)
        {
            ANI_NUMBER = 13;
        }
    }

    public class CharacterAdd : CharacterAni
    {
        public CharacterAdd(GameObject aniObject)
            :base(aniObject)
        {
            ANI_NUMBER = 15;
        }
    }

    public class CharacterRemove : CharacterAni
    {
        public CharacterRemove(GameObject aniObject)
            : base(aniObject)
        {
            ANI_NUMBER = 14;
        }
    }

    public class CharacterAttack : CharacterAni
    {
        Animator _target;
        public CharacterAttack(GameObject aniObject, GameObject target)
            :base(aniObject)
        {
            ANI_NUMBER = 3;
            _target = target.GetComponent<Animator>();
        }

        public override void Run()
        {
            base.Run();
            _target.SetInteger("state", 5);
            SoundPlayer.Instance.Play("sound0/effect/Bag_Catch");
            /*
            SkinnedMeshRenderer smr = _target.transform.FindChild("CH01").GetComponent<SkinnedMeshRenderer>();
            foreach (Material m in smr.materials)
            {
                Color c = m.color;
                c.a = 0.15f;
                m.color = c;
            }
            */
        }
    }

    public class CharacterDelete : Container
    {
        List<GameObject> _deleteTargets;
        public CharacterDelete(List<GameObject> deleteTargets)
        {
            _deleteTargets = deleteTargets;
        }

        public override void Run()
        {
            if (IsDone)
                return;
            _isDone = true;
            foreach (GameObject o in _deleteTargets)
            {
                GameObject.Destroy(o);
            }
        }
    }

    public class CharacterFadeOut : Container
    {
        GameObject _target;
        float _timer = 0.25f;
        float _curTime = 0.0f;
        public CharacterFadeOut(GameObject target, float timer = 0.25f)
        {
            _target = target;
            _timer = timer;
        }

        public override void Run()
        {
            if (IsDone)
                return;

            string[] CHILDREN = {"CH01", "CH02_B", "CH_H", "CH_HO"};
        
            _curTime += Time.deltaTime;
            _curTime = _curTime > _timer ? _timer : _curTime;

            float alpha = _curTime / _timer;
            alpha = 1.0f - alpha;

            for (int i = 0; i < CHILDREN.Length; ++i)
            {
                Renderer [] renderers = _target.transform.FindChild(CHILDREN[i]).GetComponents<Renderer>();
                foreach (Renderer r in renderers)
                {
                    foreach (Material m in r.materials)
                    {
                        Color c = m.color;
                        c.a = alpha;
                        m.color = c;
                    }
                }
            }
            if (_curTime == _timer)
                _isDone = true;
        }
    }

    public class SoundPlay : Container
    {
        string _path;
        public SoundPlay(string path)
        {
            _path = path;
        }

        public override void Run()
        {
            if (IsDone)
                return;
            _isDone = true;
            SoundPlayer.Instance.Play(_path);
        }
    }

    public enum PIECES_STATE
    {
        OUT_FIELD,
        IN_FIELD,
        GOALIN,
    }


    public class PiecesData
    {
        public PIECES_STATE _state = PIECES_STATE.OUT_FIELD;
    }

    public class PiecesMoveContainer
    {        
        static GameObject [] s_originPiecess = new GameObject[(int)PLAYER_KIND.MAX];
        private Road _curRoad = null;
        public Road CurRoad { get { return _curRoad; } set { _curRoad = value; } }
        public int _wayKind = 0;
        public bool _isGoalin = false;
        public bool _goalinSchedule = false;
        int _piecesNum;
        GameObject _pieces;
        public GameObject Pieces { get { return _pieces; } }
        PLAYER_KIND _playerKind;
        public PLAYER_KIND PlayerKind { get { return _playerKind; } }

        public Arrange _arrange;
        public Arrange Containers { get { return _arrange; } }

        public Animal _animal = Animal.MAX;

        static string[] s_moverName = {"Character/CH_01", "Character/CH_02"};

        static public void CharacterSetting(bool player1isCharacter)
        {
            if(player1isCharacter)
            {
                s_moverName[0] = "Character/CH_01";
                s_moverName[1] = "Character/CH_02";
                if (s_originPiecess[0] != null)
                {
                    if (s_originPiecess[0].name == "CH_02")
                    {
                        GameObject b = s_originPiecess[0];
                        s_originPiecess[0] = s_originPiecess[1];
                        s_originPiecess[1] = b;
                    }
                }
            }
            else
            {
                s_moverName[1] = "Character/CH_01";
                s_moverName[0] = "Character/CH_02";
                if (s_originPiecess[0] != null)
                {
                    if (s_originPiecess[0].name == "CH_01")
                    {
                        GameObject b = s_originPiecess[0];
                        s_originPiecess[0] = s_originPiecess[1];
                        s_originPiecess[1] = b;
                    }
                }
            }
        }
        
        public PiecesMoveContainer(PLAYER_KIND kind)
        {
            _playerKind = kind;
            if(s_originPiecess[(int)GameData.CurTurn] == null)
                s_originPiecess[(int)GameData.CurTurn] = Resources.Load(s_moverName[(int)GameData.CurTurn]) as GameObject;
            _pieces = GameObject.Instantiate(s_originPiecess[(int)GameData.CurTurn]);

            _curRoad = GameData.GetStartRoad();
            ///_pieces.transform.position = _curRoad._field.GetSelfField().transform.position;
            _pieces.transform.position = GameData.s_startPoint[0].transform.position;
            _pieces.GetComponent<Animator>().SetInteger("state", 1);
            _piecesNum = 1;
            GameData.s_players[(int)kind].FieldIn(1);

            int goalin = GameData.s_players[(int)kind].GetGoalInNum();
            int movepieces = 0;
            foreach(PiecesMoveContainer m in GameData.GetPiecesMover(PlayerKind))
                movepieces += m.GetPiecesNum();
            if(goalin + movepieces > GameData.PIECESMAX)
                Debug.Log("pieces number error " + (GameData.PIECESMAX - (goalin + movepieces)).ToString());
        }
        public void Add(int piecesNum)
        {
            _piecesNum += piecesNum;
            GameObject number = Pieces.transform.FindChild("billboard_P").FindChild("Population_P").gameObject;
            if (_piecesNum > 1)
            {   
                number.SetActive(true);
                number.transform.FindChild("Population_Label_P").GetComponent<TextMesh>().text = _piecesNum.ToString();
                GameObject camera = GameObject.Find("Field_Camera");
                number.transform.LookAt(camera.transform);
            }
            else
            {
                number.SetActive(false);
            }
        }

        public int GetPiecesNum()
        {
            return _piecesNum;
        }

        public PatternSystem.Arrange StartPointToStay()
        {
            List<Container> containers = new List<Container>();
            _curRoad = GameData.GetStartRoad();

            Vector3 offsetPoint = _pieces.transform.position;
            Vector3 p = _curRoad._field.GetSelfField().transform.position - offsetPoint;
            float roty = Mathf.Atan2(p.x, p.z) * Mathf.Rad2Deg;
            containers.Add(new Rotation(_pieces, new Vector3(0.0f, roty, 0.0f), 0.0f, Physical.Type.ABSOLUTE));
            if (_curRoad._field.Mover != null)
            {
                if (PlayerKind == _curRoad._field.Mover.PlayerKind)
                {
                    containers.Add(new CharacterRide(_pieces));
                }
                else
                {
                    containers.Add(new CharacterAttack(_pieces, _curRoad._field.Mover.Pieces));
                }
                containers.Add(new CharacterFadeOut(_curRoad._field.Mover.Pieces));
            }
            else
            {
                containers.Add(new CharacterMove(_pieces));
            }
            containers.Add(new JumpingMove(_pieces, p, 0.2f));
            containers.Add(new Rotation(_pieces, new Vector3(_pieces.transform.localEulerAngles.x, 180.0f, _pieces.transform.localEulerAngles.z), 0.0f, Physical.Type.ABSOLUTE));
            containers.Add(new CharacterIdle(_pieces));
            containers.Add(new FieldSet(_curRoad._field, this));
            _arrange = new Arrange(_pieces, Arrange.ArrangeType.SERIES, containers, 1);
            _animal = Animal.BACK_DO;
            return _arrange;
        }

        public PatternSystem.Arrange Move(Animal animal)
        {
            List<Container> containers = new List<Container>();            
            int forwardNum = GameData.GetForwardNum(animal);
            ///PiecesMoveContainer mover = GameData.InFieldMoverCheck(CurRoad, forwardNum);

            if (_animal != Animal.MAX)
            {
                if (_curRoad != null)
                    _curRoad._field.Mover = null;
            }
            else
            {
                containers.Add(new Timer(_pieces, 0.3f));
            }

            _animal = animal;

            if (_goalinSchedule)
            {
                if(animal == Animal.BACK_DO)
                {
                    _curRoad = GameData.GetAllKillRoad();
                    Vector3 offsetPoint = _pieces.transform.position;
                    Vector3 p = _curRoad._field.GetSelfField().transform.position - offsetPoint;
                    float roty = Mathf.Atan2(p.x, p.z) * Mathf.Rad2Deg;
                    containers.Add(new Rotation(_pieces, new Vector3(0.0f, roty, 0.0f), 0.0f, Physical.Type.ABSOLUTE));
                    containers.Add(new CharacterMove(_pieces));
                    containers.Add(new Timer(_pieces, 0.1f));
                    containers.Add(new JumpingMove(_pieces, p, 0.4f));
                    containers.Add(new CharacterIdle(_pieces));
                    containers.Add(new FieldSet(_curRoad._field, this));
                }
                else
                {
                    _isGoalin = true;
                    containers.Add(new Timer(null, 0.1f));
                    containers.Add(new GoalIn(this));
                }
            }
            else
            {
                Vector3 lastStepPoint = new Vector3(0,0,0);
                Vector3 offsetPoint = _pieces.transform.position;
                if (forwardNum > 0)
                {
                    for (int i = 0; i < forwardNum; ++i)
                    {
                        if (GameData.NextRoad(_curRoad) == null)
                        {
                            _isGoalin = true;
                            containers.Add(new Timer(null, 0.1f));
                            containers.Add(new GoalIn(this));
                            break;
                        }
                        else
                        {
                            _curRoad = GameData.NextRoad(_curRoad);
                            Vector3 p = _curRoad._field.GetSelfField().transform.position - offsetPoint;
                            float roty = Mathf.Atan2(p.x, p.z) * Mathf.Rad2Deg;
                            offsetPoint = _curRoad._field.GetSelfField().transform.position;
                            containers.Add(new Rotation(_pieces, new Vector3(0.0f, roty, 0.0f), 0.0f, Physical.Type.ABSOLUTE));
                            bool isstepped = false;
                            p -= lastStepPoint;
                            lastStepPoint.x = 0.0f;
                            lastStepPoint.y = 0.0f;
                            lastStepPoint.z = 0.0f;

                            if (forwardNum == i + 1)
                            {
                                if (_curRoad._field.Mover != null)
                                {
                                    
                                    if (PlayerKind == _curRoad._field.Mover.PlayerKind)
                                    {
                                        containers.Add(new CharacterRide(_pieces));
                                    }
                                    else
                                    {
                                        containers.Add(new CharacterAttack(_pieces, _curRoad._field.Mover.Pieces));
                                    }
                                    containers.Add(new CharacterFadeOut(_curRoad._field.Mover.Pieces));
                                }
                                else
                                {
                                    containers.Add(new CharacterMove(_pieces));
                                }
                            }
                            else
                            {                                
                                if (_curRoad._field.Mover != null)
                                {
                                    isstepped = true;
                                    lastStepPoint = _curRoad._field.Mover.Pieces.transform.FindChild("Position_Check_P").transform.localPosition;
                                    p += lastStepPoint;
                                    ///밟아서 가는 애 처
                                }
                                containers.Add(new CharacterMove(_pieces));
                            }
                            containers.Add(new JumpingMove(_pieces, p, 0.4f));
                            containers.Add(new CharacterIdle(_pieces));
                            if (isstepped)
                            {
                                containers.Add(new CharacterStepped(_curRoad._field.Mover.Pieces));
                                containers.Add(new CharacterIdle(_curRoad._field.Mover.Pieces));
                            }
                        }
                    }                    

                    if (!_isGoalin)
                    {
                        containers.Add(new Rotation(_pieces, new Vector3(_pieces.transform.localEulerAngles.x, 180.0f, _pieces.transform.localEulerAngles.z), 0.0f, Physical.Type.ABSOLUTE));
                        containers.Add(new Timer(null, 0.1f));
                        containers.Add(new FieldSet(_curRoad._field, this));
                    }
                }
                else
                {
                    if (GameData.PrevRoad(_curRoad) == null)
                    {
                        _isGoalin = true;
                        containers.Add(new Timer(_pieces, 0.1f));
                        containers.Add(new GoalIn(this));
                    }
                    else
                    {
                        _curRoad = GameData.PrevRoad(_curRoad);
                        Vector3 p = _curRoad._field.GetSelfField().transform.position - offsetPoint;
                        float roty = Mathf.Atan2(p.x, p.z) * Mathf.Rad2Deg;
                        offsetPoint = _curRoad._field.GetSelfField().transform.position;
                        if (_curRoad._field.Mover != null)
                        {

                            if (PlayerKind == _curRoad._field.Mover.PlayerKind)
                            {
                                containers.Add(new CharacterRide(_pieces));
                            }
                            else
                            {
                                containers.Add(new CharacterAttack(_pieces, _curRoad._field.Mover.Pieces));
                            }
                            containers.Add(new CharacterFadeOut(_curRoad._field.Mover.Pieces));
                            ///containers.Add(new Timer(null, 0.25f));

                        }
                        else
                        {
                            containers.Add(new CharacterMove(_pieces));
                        }
                        containers.Add(new Timer(null, 0.1f));
                        containers.Add(new Rotation(_pieces, new Vector3(0.0f, roty, 0.0f), 0.0f, Physical.Type.ABSOLUTE));
                        containers.Add(new JumpingMove(_pieces, p, 0.4f));
                        containers.Add(new CharacterIdle(_pieces));
                    }

                    if (!_isGoalin)
                    {
                        containers.Add(new Rotation(_pieces, new Vector3(_pieces.transform.localEulerAngles.x, 180.0f, _pieces.transform.localEulerAngles.z), 0.0f, Physical.Type.ABSOLUTE));
                        containers.Add(new Timer(_pieces, 0.1f));
                        containers.Add(new FieldSet(_curRoad._field, this));
                    }
                }
            }
            _arrange = new Arrange(_pieces, Arrange.ArrangeType.SERIES, containers, 1);
            return _arrange;
        }
    }

    public enum PLAYER_KIND
    {
        PLAYER_1,
        PLAYER_2,
        MAX,
    }

    public class PlayerData
    {
        UILabel _goalInNum;
        public PlayerData(PLAYER_KIND kind,  int piecesMax = 5)
        {
            _pieces = new PiecesData[piecesMax];
            for(int i = 0; i < piecesMax; ++i)
            {
                _pieces[i] = new PiecesData();
            }

            Transform gp = GameObject.Find("UI Root").transform.FindChild("Size").FindChild("GamePlay");
            if (kind == PLAYER_KIND.PLAYER_1)
            {
                _goalInNum = gp.FindChild("Play01").FindChild("Label (8)").GetComponent<UILabel>();
                gp.FindChild("Play01").FindChild("Substitute_Label_Count_P").GetComponent<UILabel>().text = piecesMax.ToString();
            }
            else
            {
                _goalInNum = gp.FindChild("Play02").FindChild("Label (8)").GetComponent<UILabel>();
                gp.FindChild("Play02").FindChild("Substitute_Label_Count_P").GetComponent<UILabel>().text = piecesMax.ToString();
            }
            _goalInNum.text = "0";
        }

        public PiecesData[] _pieces;
        public int GetInFieldNum()
        {
            int cnt = 0;
            for (int i = 0; i < _pieces.Length; ++i)
            {
                if (_pieces[i]._state == PIECES_STATE.IN_FIELD)
                    ++cnt;
            }
            return cnt;
        }

        public int GetOutFieldNum()
        {
            int cnt = 0;
            for (int i = 0; i < _pieces.Length; ++i)
            {
                if (_pieces[i]._state == PIECES_STATE.OUT_FIELD)
                    ++cnt;
            }
            return cnt;
        }

        public int GetGoalInNum()
        {
            int cnt = 0;
            for (int i = 0; i < _pieces.Length; ++i)
            {
                if (_pieces[i]._state == PIECES_STATE.GOALIN)
                    ++cnt;
            }
            return cnt;
        }

        public void FieldIn(int num)
        {
            if(GetOutFieldNum() > 0)
            {
                int cnt = 0;
                for (int i = 0; i < _pieces.Length; ++i)
                {
                    if (_pieces[i]._state == PIECES_STATE.OUT_FIELD)
                    {
                        _pieces[i]._state = PIECES_STATE.IN_FIELD;
                        ++cnt;
                    }
                    if (cnt == num)
                    {
                        if (GetInFieldNum() + GetOutFieldNum() + GetGoalInNum() > GameData.PIECESMAX)
                            Debug.Log("what the fuck !!!");
                        return;
                    }
                }
            }
        }

        public void Out(int num)
        {
            if (GetInFieldNum() > 0)
            {
                int cnt = 0;
                for (int i = 0; i < _pieces.Length; ++i)
                {
                    if (_pieces[i]._state == PIECES_STATE.IN_FIELD)
                    {
                        _pieces[i]._state = PIECES_STATE.OUT_FIELD;
                        ++cnt;
                    }
                    if(cnt == num)
                    {
                        if (GetInFieldNum() + GetOutFieldNum() + GetGoalInNum() > GameData.PIECESMAX)
                            Debug.Log("what the fuck !!!");
                        return;
                    }   
                }
            }
        }

        public void Reset()
        {
            for (int i = 0; i < _pieces.Length; ++i)
            {
                _pieces[i]._state = PIECES_STATE.OUT_FIELD;
            }
        }

        public void GoalIn(int num)
        {
            if (GetInFieldNum() > 0)
            {
                int cnt = 0;
                for (int i = 0; i < _pieces.Length; ++i)
                {
                    if (_pieces[i]._state == PIECES_STATE.IN_FIELD)
                    {
                        _pieces[i]._state = PIECES_STATE.GOALIN;
                        ++cnt;
                    }
                    if (cnt == num)
                    {
                        _goalInNum.text = GetGoalInNum().ToString();
                        if (GetInFieldNum() + GetOutFieldNum() + GetGoalInNum() > GameData.PIECESMAX)
                            Debug.Log("what the fuck !!!");
                        return;
                    }
                }
            }
        }
    }

    public class FieldData
    {
        GameObject _selfField;
        List<FieldAttribute> _attributes = new List<FieldAttribute>();
        private PiecesMoveContainer _mover;
        public PiecesMoveContainer Mover { get { return _mover; } set { _mover = value; } }
        

        public void SetField(GameObject field)
        {
            _selfField = field;
        }

        public GameObject GetSelfField()
        {
            return _selfField;
        }

        public void AddAttribute(FieldAttribute attribute)
        {
            _attributes.Add(attribute);
        }

        public void FieldRun()
        {
            foreach (FieldAttribute att in _attributes)
            {
                att.Run(Mover);
            }
        }
    }

    public class Road
    {
        public FieldData _field;
        public Road _next;
        public Road _prev;
        public int _wayKind = 0;
    }




    /*

    f14 -   f13 -   f12 -   f11 -   f10 -   f9
                        
    f15     f28                     f25     f8
                  f27        f26  
    f16                f22                  f7
                    
    f17           f23         f21           f6

    f18     f24                     f20     f5

    f19 -   f0   -  f1  -   f2  -   f3 -    f4


     */



    public class GameData
    {
        public static int PIECESMAX = 5;
        public static int _1creditToCount = 1;

        public static int _cur1creditToCount = 0;
        public static int _curCreditCount = 0;

        public const int FIELD_MAXNUM = 30;
        public const int ROAD_MAXNUM = 8;
        public const int WAYKIND = 4;
        public const int ANIMAL_STATE_NUM = 4;

        public static List<Animal> _curAnimals = new List<Animal>();
        public static PlayerData[] s_players = new PlayerData[(int)PLAYER_KIND.MAX];
        private static PLAYER_KIND _curTurn = PLAYER_KIND.PLAYER_1;
        public static PLAYER_KIND CurTurn {get {return _curTurn;}}
        public static FieldData[] _fields = new FieldData[FIELD_MAXNUM];

        public static Road[] s_roads = new Road[WAYKIND];
        ///public static PlayerControl s_plyerControlNum = PlayerControl.Player1;

        public static Road s_allKillRoad;

        public static List<FieldData>[] _roads = new List<FieldData>[ROAD_MAXNUM];
        public static List<List<FieldData>>[] _ways = new List<List<FieldData>>[WAYKIND];
        public static GameObject[] s_animalStateList = new GameObject[ANIMAL_STATE_NUM];
        public static GameObject s_animaleEffect;
        public static bool s_backdoLock = false;        

        public static GameObject [] s_startPoint = new GameObject[(int)PLAYER_KIND.MAX];

        static Dictionary<Animal, int> s_animalToForwardNum = new Dictionary<Animal, int>();

        static Dictionary<PLAYER_KIND, List<PiecesMoveContainer>> s_moveContainers = new Dictionary<PLAYER_KIND, List<PiecesMoveContainer>>();

        static bool s_oneMoreUootThrow = false;
        public static bool IsOneMoreUootThrow { get { return s_oneMoreUootThrow; } }
        public static void OneMoreUootThrowCheck() { s_oneMoreUootThrow = false; }
        public static void OneMoreUootThrow() { s_oneMoreUootThrow = true; }

        static bool s_shoot = false;

        static bool s_IsPlayer1IsCharcter1 = true;
        static public bool IsPlayer1IsCharacter1 {get{ return s_IsPlayer1IsCharcter1;}}
        public static bool IsShoot {get{ return s_shoot;}}
        public static void ShootCheck() {s_shoot = false;}
        public static void ShootMode(){s_shoot = true;}


        public static void Init()
        {
            for(int i = 0; i < _fields.Length; ++i)
            {
                _fields[i] = new FieldData();
            }           

            WayInit();

            _fields[0].AddAttribute(new ExitSchedule());
            _fields[4].AddAttribute(new ChangeWay(0));
            _fields[5].AddAttribute(new ChangeWay(2));

            _fields[6].AddAttribute(new Kill());
            _fields[7].AddAttribute(new AddPieces());
            _fields[9].AddAttribute(new RemovePieces());

            _fields[9].AddAttribute(new ChangeWay(0));
            _fields[10].AddAttribute(new ChangeWay(3));

            _fields[11].AddAttribute(new AddPieces());
            _fields[12].AddAttribute(new OneMoreThrow());
            _fields[13].AddAttribute(new RemovePieces());

            Road r = GameData.GetStartRoad();
            for (int i = 0; i < 11; ++i )
                r = GameData.NextRoad(r);

            _fields[14].AddAttribute(new Send1toSend2(r));

            r = GameData.GetStartRoad();
            r = GameData.GetWayChangetoRoad(2, r);
            for (int i = 0; i < 9; ++i)
                r = GameData.NextRoad(r);

            _fields[17].AddAttribute(new Send1toSend2(r));
            _fields[19].AddAttribute(new AllKill());
            _fields[20].AddAttribute(new Exit());

            _fields[22].AddAttribute(new Kill());            
            _fields[23].AddAttribute(new ChangeWay(1));
            _fields[25].AddAttribute(new Kill());

            _fields[26].AddAttribute(new Shoot());

            r = GameData.GetStartRoad();
            for (int i = 0; i < 8; ++i)
                r = GameData.NextRoad(r);            
            _fields[27].AddAttribute(new Send1toSend2(r));            

            s_animalToForwardNum.Add(Animal.DO, 1);
            s_animalToForwardNum.Add(Animal.GE, 2);
            s_animalToForwardNum.Add(Animal.KUL, 3);
            s_animalToForwardNum.Add(Animal.UOOT, 4);
            s_animalToForwardNum.Add(Animal.MO, 5);
            s_animalToForwardNum.Add(Animal.BACK_DO, -1);
            for (int i = 0; i < s_players.Length; ++i)
            {
                s_players[i] = new PlayerData((PLAYER_KIND)i, PIECESMAX);
                s_moveContainers.Add((PLAYER_KIND)i, new List<PiecesMoveContainer>());
            }
            InitAnimalStateView();
            LoadReadyFieldCharacter();
            ///InputManager.Instance.SetPlayerNum(s_plyerControlNum);
            NextTurnCheck.Instance.GameTurnMarking(PLAYER_KIND.PLAYER_1);
            Calculate.Instance.EidtEnable();
        }

        private static void WayInit()
        {
            _fields[0].SetField(FieldAdder.GetFields(19));
            for (int i = 0; i < _fields.Length-1; ++i )
            {
                _fields[i+1].SetField(FieldAdder.GetFields(i));
            }

            _roads[0] = new List<FieldData>();
            for (int i = 0; i < 6; ++i)
            {   
                _roads[0].Add(_fields[i]);
            }

            _roads[1] = new List<FieldData>();
            for (int i = 6; i < 11; ++i)
            {
                _roads[1].Add(_fields[i]);
            }

            _roads[2] = new List<FieldData>();
            for (int i = 11; i < 16; ++i)
            {
                _roads[2].Add(_fields[i]);
            }

            _roads[3] = new List<FieldData>();
            for (int i = 16; i < 21; ++i)
            {
                _roads[3].Add(_fields[i]);
            }

            _roads[4] = new List<FieldData>();
            for (int i = 21; i < 24; ++i)
            {
                _roads[4].Add(_fields[i]);
            }

            _roads[5] = new List<FieldData>();
            for (int i = 24; i < 26; ++i)
            {
                _roads[5].Add(_fields[i]);
            }
            _roads[5].Add(_fields[20]);

            _roads[6] = new List<FieldData>();
            for (int i = 28; i < 30; ++i)
            {
                _roads[6].Add(_fields[i]);
            }
            _roads[6].Add(_fields[15]);

            _roads[7] = new List<FieldData>();
            for (int i = 26; i < 28; ++i)
            {
                _roads[7].Add(_fields[i]);
            }
            _roads[7].Add(_fields[23]);



            _ways[0] = new List<List<FieldData>>();
            _ways[0].Add(_roads[0]);
            _ways[0].Add(_roads[1]);
            _ways[0].Add(_roads[2]);
            _ways[0].Add(_roads[3]);
            List<Road> roads = new List<Road>();
            foreach (List<FieldData> fields in _ways[0])
            {
                foreach (FieldData field in fields)
                {
                    Road road = new Road();
                    road._field = field;
                    road._wayKind = 0;
                    roads.Add(road);
                }
            }
            Road prev = null;
            for (int i = 0; i < roads.Count-1; ++i)
            {
                roads[i]._prev = prev;
                roads[i]._next = roads[i+1];
                prev = roads[i];
            }
            roads[roads.Count-1]._prev = prev;
            s_roads[0] = roads[0];
            s_allKillRoad = roads[roads.Count - 2];
            roads.Clear();

            _ways[1] = new List<List<FieldData>>();
            _ways[1].Add(_roads[0]);
            _ways[1].Add(_roads[4]);
            _ways[1].Add(_roads[5]);
            foreach (List<FieldData> fields in _ways[1])
            {
                foreach (FieldData field in fields)
                {
                    Road road = new Road();
                    road._field = field;
                    road._wayKind = 1;
                    roads.Add(road);
                }
            }
            prev = null;
            for (int i = 0; i < roads.Count-1; ++i)
            {
                roads[i]._prev = prev;
                roads[i]._next = roads[i+1];
                prev = roads[i];
            }
            roads[roads.Count-1]._prev = prev;
            s_roads[1] = roads[0];
            roads.Clear();

            _ways[2] = new List<List<FieldData>>();
            _ways[2].Add(_roads[0]);
            _ways[2].Add(_roads[4]);
            _ways[2].Add(_roads[6]);
            _ways[2].Add(_roads[3]);
            foreach (List<FieldData> fields in _ways[2])
            {
                foreach (FieldData field in fields)
                {
                    Road road = new Road();
                    road._field = field;
                    road._wayKind = 2;
                    roads.Add(road);
                }
            }
            prev = null;
            for (int i = 0; i < roads.Count-1; ++i)
            {
                roads[i]._prev = prev;
                roads[i]._next = roads[i+1];
                prev = roads[i];
            }
            roads[roads.Count-1]._prev = prev;
            s_roads[2] = roads[0];
            roads.Clear();

            _ways[3] = new List<List<FieldData>>();
            _ways[3].Add(_roads[0]);
            _ways[3].Add(_roads[1]);
            _ways[3].Add(_roads[7]);
            _ways[3].Add(_roads[5]);
            foreach (List<FieldData> fields in _ways[3])
            {
                foreach (FieldData field in fields)
                {
                    Road road = new Road();
                    road._field = field;
                    road._wayKind = 3;
                    roads.Add(road);
                }
            }
            prev = null;
            for (int i = 0; i < roads.Count-1; ++i)
            {
                roads[i]._prev = prev;
                roads[i]._next = roads[i+1];
                prev = roads[i];
            }
            roads[roads.Count-1]._prev = prev;
            s_roads[3] = roads[0];
            roads.Clear();
        }

        public static void InitAnimalStateView()
        {
            GameObject finder = GameObject.Find("UI Root");
            GameObject effectFinder = finder;
            string[] animalStateObjectPath = {"Size","GamePlay","S_Uoot_P" };
            string[] animalEffectObjectPath = {"Size","GamePlay","B_Uoot_P" };
            foreach (string p in animalStateObjectPath)
            {
                finder = finder.transform.FindChild(p).gameObject;
            }

            foreach (string p in animalEffectObjectPath)
            {
                effectFinder = effectFinder.transform.FindChild(p).gameObject;
            }
            s_animaleEffect = effectFinder;
            s_animaleEffect.SetActive(false);

            for (int i = 0; i < ANIMAL_STATE_NUM; ++i)
            {
                s_animalStateList[i] = finder.transform.FindChild("0" + (i + 1).ToString() + "_P").gameObject;
            }
            foreach (GameObject go in s_animalStateList)
            {
                go.SetActive(false);
            }
        }

        static GameObject[] s_readyField = new GameObject[5];
        static int s_indexReadyField = 4;

        public static void LoadReadyFieldCharacter()
        {
            GameObject rf = GameObject.Find("ReadyField").transform.FindChild("blank_N").gameObject;
            s_startPoint[0] = rf.transform.FindChild("CH_01").gameObject;
            s_startPoint[1] = rf.transform.FindChild("CH_02").gameObject;
            s_startPoint[0].SetActive(true);
            s_startPoint[0].SetActive(false);

            TextMesh tm = s_startPoint[0].transform.FindChild("billboard_P").FindChild("Population_P").FindChild("Population_Label_P").GetComponent<TextMesh>();
            tm.text = s_players[0].GetOutFieldNum().ToString();

            for(int i = 0; i < s_readyField.Length; ++i)
            {
                s_readyField[i] = rf.transform.FindChild("0" + (i+1).ToString()).gameObject;
            }
        }

        public static void ChacracterSetting()
        {
            GameObject rf = GameObject.Find("ReadyField").transform.FindChild("blank_N").gameObject;
            Transform gp = GameObject.Find("UI Root").transform.FindChild("Size").FindChild("GamePlay");
            if(s_IsPlayer1IsCharcter1)
            {
                s_startPoint[0] = rf.transform.FindChild("CH_01").gameObject;
                s_startPoint[1] = rf.transform.FindChild("CH_02").gameObject;
                ///gp.FindChild("Play01").FindChild("Character_Sprite_P").GetComponent<UISprite>().spriteName = "ETC_CH01";
                ///gp.FindChild("Play02").FindChild("Character_Sprite_P").GetComponent<UISprite>().spriteName = "ETC_CH02";                
            }
            else
            {
                s_startPoint[1] = rf.transform.FindChild("CH_01").gameObject;
                s_startPoint[0] = rf.transform.FindChild("CH_02").gameObject;

                ///gp.FindChild("Play01").FindChild("Character_Sprite_P").GetComponent<UISprite>().spriteName = "ETC_CH02";
                ///gp.FindChild("Play02").FindChild("Character_Sprite_P").GetComponent<UISprite>().spriteName = "ETC_CH01";
            }
            s_startPoint[0].SetActive(true);
            s_startPoint[1].SetActive(false);

            TextMesh tm = s_startPoint[0].transform.FindChild("billboard_P").FindChild("Population_P").FindChild("Population_Label_P").GetComponent<TextMesh>();
            tm.text = s_players[0].GetOutFieldNum().ToString();
        }

        public static void StartPointVisible(bool visible)
        {
            s_startPoint[0].SetActive(visible);
            s_startPoint[1].SetActive(visible);
        }

        public static void OpenAnimalChoice()
        {
            foreach (GameObject go in s_animalStateList)
            {
                if(go.active == true)
                {
                    go.transform.FindChild("Select_P").gameObject.SetActive(true);
                    return;
                }
            }
        }

        public static void CloseAnimalChoice()
        {
            foreach (GameObject go in s_animalStateList)
            {   
                go.transform.FindChild("Select_P").gameObject.SetActive(false);
            }
            BackDoUnLock();
        }

        public static void BackDoLock()
        {
            GameObject go;
            for (int i = 0; i < s_animalStateList.Length; ++i)
            {
                go = s_animalStateList[i];
                if (go.active)
                {
                    if (s_animalStateList[i].GetComponent<AnimalContainer>()._animal == Animal.BACK_DO)
                        s_animalStateList[i].transform.FindChild("Uoot_Sprite_P").GetComponent<UISprite>().alpha = 0.4f;
                }
            }
            s_backdoLock = true;
        }

        public static void BackDoUnLock()
        {
            GameObject go;
            for (int i = 0; i < s_animalStateList.Length; ++i)
            {
                go = s_animalStateList[i];
                s_animalStateList[i].transform.FindChild("Uoot_Sprite_P").GetComponent<UISprite>().alpha = 1.0f;

            }
            s_backdoLock = false;
        }

        public static void LeftAnimalChoice()
        {   
            GameObject go;
            List<GameObject> activeList = new List<GameObject>();
            for (int i = 0; i < s_animalStateList.Length; ++i)
            {
                go = s_animalStateList[i];
                if (go.active)
                    activeList.Add(go);
            }


            for (int i = 0; i < activeList.Count; ++i)
            {
                go = activeList[i].transform.FindChild("Select_P").gameObject;
                if (go.active)
                {
                    go.SetActive(false);
                    if (i == 0)
                    {
                        if (s_backdoLock)
                        {
                            if (activeList[activeList.Count - 1].GetComponent<AnimalContainer>()._animal == Animal.BACK_DO)
                            {
                                go.SetActive(true);
                                return;
                            }
                        }
                        activeList[activeList.Count - 1].transform.FindChild("Select_P").gameObject.SetActive(true);
                    }
                    else
                    {
                        if (s_backdoLock)
                        {
                            if (activeList[i - 1].GetComponent<AnimalContainer>()._animal == Animal.BACK_DO)
                            {
                                go.SetActive(true);
                                return;
                            }
                        }
                        activeList[i - 1].transform.FindChild("Select_P").gameObject.SetActive(true);
                    }
                    return;
                }
            }
        }

        public static void RightAnimalChoice()
        {
            GameObject go;
            List<GameObject> activeList = new List<GameObject>();
            for (int i = 0; i < s_animalStateList.Length; ++i)
            {
                go = s_animalStateList[i];
                if (go.active)
                    activeList.Add(go);
            }


            for (int i = 0; i < activeList.Count; ++i)
            {
                go = activeList[i].transform.FindChild("Select_P").gameObject;
                if (go.active)
                {
                    go.SetActive(false);
                    if (i == activeList.Count - 1)
                    {
                        if (s_backdoLock)
                        {
                            if (activeList[0].GetComponent<AnimalContainer>()._animal == Animal.BACK_DO)
                            {
                                go.SetActive(true);
                                return;
                            }
                        }
                        activeList[0].transform.FindChild("Select_P").gameObject.SetActive(true);
                    }
                    else
                    {
                        if (s_backdoLock)
                        {
                            if (activeList[i + 1].GetComponent<AnimalContainer>()._animal == Animal.BACK_DO)
                            {
                                go.SetActive(true);
                                return;
                            }
                        }
                        activeList[i + 1].transform.FindChild("Select_P").gameObject.SetActive(true);
                    }
                    return;
                }
            }
        }

        public static Animal EnterAnimalChoice()
        {   
            GameObject go;
            for (int i = 0; i < s_animalStateList.Length; ++i)
            {
                go = s_animalStateList[i].transform.FindChild("Select_P").gameObject;
                if (go.active)
                {
                    go.SetActive(false);
                    return s_animalStateList[i].GetComponent<AnimalContainer>()._animal;
                }
            }
            return Animal.NONE;
        }


        public static void TurnSave()
        {
        }

        public static FieldData GetExitField()
        {
            return _fields[20];
        }

        public static Road GetStartRoad()
        {
            return s_roads[0];
        }

        public static Road GetAllKillRoad()
        {
            return s_allKillRoad;
        }

        public static Road GetWayChangetoRoad(int changeWay, Road road)
        {
            if (changeWay == 1 && road._wayKind == 3)
                return road;

            Road r = s_roads[changeWay];
            while(r != null)
            {
                if (road._field == r._field)
                    return r;
                r = NextRoad(r);
            }
            return null;
        }

        public static Road NextRoad(Road cur)
        {
            return cur._next;
        }

        public static Road PrevRoad(Road cur)
        {
            return cur._prev;
        }

        public static PiecesMoveContainer InFieldMoverCheck(Road curRoad, int forward)
        {
            Road r = curRoad;
            if(forward >= 0)
            {
                for(int i = 0; i < forward; ++i)
                {                    
                    r = NextRoad(r);
                    if (r == null)
                        return null;                    
                }

                if (NextRoad(r) == null)
                {
                    if (GetStartRoad()._field.Mover != null)
                        return GetStartRoad()._field.Mover;
                }
            }
            else
            {
                forward *= -1;
                for(int i = 0; i < forward; ++i)
                {
                    r = PrevRoad(r);
                    if (r == null)
                        return null;
                }

                if (PrevRoad(r) == null)
                {
                    if (GetExitField().Mover != null)
                        return GetExitField().Mover;
                }
            }
            return r._field.Mover;
        }

        public static List<Vector3> GetWay(int wayKind)
        {
            List<Vector3> way = new List<Vector3>();
            foreach (List<FieldData> fields in _ways[wayKind])
            {
                foreach (FieldData field in fields)
                {
                    way.Add(field.GetSelfField().transform.position);
                }
            }
            return way;
        }

        public static int GetWayToIndex(int wayKind, FieldData findField)
        {
            int index = -1;
            if (findField == null)
                return index;
           

            foreach (List<FieldData> fields in _ways[wayKind])
            {  
                index = fields.IndexOf(findField);
                if(index >= 0)
                    return index;
            }
            return index;
        }

        public static FieldData GetWayToField(int wayKind, FieldData curField, int forwardNum)
        {
            List<FieldData> wayFields = new List<FieldData>();
            foreach (List<FieldData> fields in _ways[wayKind])
            {
                foreach (FieldData field in fields)
                {
                    wayFields.Add(field);
                }
            }

            if (curField == null)
            {
                if (forwardNum < 0)
                    return null;
                if(wayFields.Count > forwardNum)
                    return wayFields[forwardNum];
                return wayFields[wayFields.Count - 1];
            }

            int index = wayFields.IndexOf(curField);
            if (index > 0)
            {
                if (wayFields.Count > index + forwardNum)
                    return wayFields[index + forwardNum];
                return wayFields[wayFields.Count - 1];
            }

            return null;
        }

        public static void TurnRollBack()
        {
            GameData._curAnimals.Clear();
        }

        public static int CurAnimalCount()
        {
            return _curAnimals.Count;
        }

        public static Dictionary<Animal, int> GetAnimalState()
        {
            Dictionary<Animal, int> animalState = new Dictionary<Animal, int>();
            for (Animal a = Animal.DO; a < Animal.MAX; ++a)
            {
                animalState.Add(a, 0);
            }

            foreach (Animal a in _curAnimals)
            {
                animalState[a] = animalState[a] + 1;
            }
            return animalState;
        }

        public static void AddAnimal(Animal animal)
        {
            _curAnimals.Add(animal);
        }

        public static Animal GetAnimal(int index)
        {
            return _curAnimals[index];
        }

        public static void RemoveAnimal(int index)
        {
            _curAnimals.RemoveAt(index);
            RefreshAnimalView(false);
        }

        public static void RemoveAnimal(Animal animal)
        {
            _curAnimals.Remove(animal);
            RefreshAnimalView(false);
        }

        public static Animal GetLastAnimal()
        {
            if(_curAnimals.Count > 0)
                return _curAnimals[_curAnimals.Count - 1];
            return Animal.NONE;
        }

        public static void RefreshAnimalView(bool isNewThrow = true)
        {
            string animalObj = "Uoot_Sprite_P";
            string animalNumObj = "Uoot_Label_Count_P";
            foreach (GameObject go in s_animalStateList)
            {
                go.SetActive(false);
            }

            Dictionary<Animal, string> animalImage = new Dictionary<Animal, string>();
            for (Animal a = Animal.DO; a < Animal.BACK_DO; ++a)
            {
                animalImage.Add(a, "ETC_S_Uoot_" + (((int)a) + 1).ToString());
            }
            animalImage.Add(Animal.BACK_DO, "ETC_S_Uoot_0");

            Dictionary<Animal, int> animalState = GetAnimalState();
            int index = 0;

            for (Animal a = Animal.DO; a < Animal.MAX; ++a)
            {
                if (animalState[a] > 0)
                {
                    ++index;
                    GameObject s = s_animalStateList[index];
                    s.SetActive(true);
                    s.GetComponent<AnimalContainer>()._animal = a;
                    s.transform.FindChild(animalObj).GetComponent<UISprite>().spriteName = animalImage[a];
                    if (animalState[a] > 1)
                    {
                        s.transform.FindChild(animalNumObj).GetComponent<UILabel>().text = animalState[a].ToString();
                        s.transform.FindChild(animalNumObj).gameObject.SetActive(true);
                    }
                    else 
                    {
                        s.transform.FindChild(animalNumObj).GetComponent<UILabel>().text = animalState[a].ToString();
                        s.transform.FindChild(animalNumObj).gameObject.SetActive(false);
                    }
                }
            }

            animalImage.Clear();
            for (Animal a = Animal.DO; a < Animal.BACK_DO; ++a)
            {
                animalImage.Add(a, "ETC_B_Uoot_" + (((int)a) + 1).ToString());
            }
            animalImage.Add(Animal.BACK_DO, "ETC_B_Uoot_0");

            if(isNewThrow)
            {
                if (animalImage.ContainsKey(GetLastAnimal()))
                {
                    s_animaleEffect.SetActive(true);
                    s_animaleEffect.transform.FindChild("B_Uoot_Sprite_P").GetComponent<UISprite>().spriteName = animalImage[GetLastAnimal()];
                }
            }
            else
            {
                s_animaleEffect.SetActive(false);
            }
            
        }
        public static int GetForwardNum(Animal animal)
        {
            if(!s_animalToForwardNum.ContainsKey(animal))
                Debug.Log("Forward Animal : " + ((int)animal).ToString());
                
            return s_animalToForwardNum[animal];
        }

        static public bool s_IsNotControlChange = false;

        public static void NextTurn()
        {
            s_startPoint[(int)_curTurn].SetActive(false);
            _curTurn = ( _curTurn == PLAYER_KIND.PLAYER_1 ? PLAYER_KIND.PLAYER_2 : PLAYER_KIND.PLAYER_1 );
            OutPiecessViewRefresh();
            if (!s_IsNotControlChange)
            {
                InputManager.Instance.Next();
            }
            s_IsNotControlChange = false;

            s_readyField[s_indexReadyField].SetActive(false);
            s_indexReadyField = UnityEngine.Random.RandomRange(0, 5);
            s_readyField[s_indexReadyField].SetActive(true);
            if (CurTurn == PLAYER_KIND.PLAYER_1)
            {
                NextTurnCheck.Instance.Left();
            }
            else
            {
                NextTurnCheck.Instance.Right();
            }
        }

        public static void OutPiecessViewRefresh()
        {
            if(GetCurTurnOutPiecess() > 0)
                s_startPoint[(int)_curTurn].SetActive(true);
            TextMesh tm = s_startPoint[(int)_curTurn].transform.FindChild("billboard_P").FindChild("Population_P").FindChild("Population_Label_P").GetComponent<TextMesh>();
            tm.text = s_players[(int)_curTurn].GetOutFieldNum().ToString();
        }

        public static PatternSystem.Arrange NewInField(Animal animal)
        {
            PiecesMoveContainer mover = new PiecesMoveContainer(_curTurn);
            PatternSystem.Arrange arr = null;

            GameData.s_startPoint[0].SetActive(false);
            GameData.s_startPoint[1].SetActive(false);

            if (animal != Animal.BACK_DO)
                arr = mover.Move(animal);
            else
                arr = mover.StartPointToStay();

            if(arr != null)
                s_moveContainers[_curTurn].Add(mover);

            TextMesh tm = s_startPoint[(int)_curTurn].transform.FindChild("billboard_P").FindChild("Population_P").FindChild("Population_Label_P").GetComponent<TextMesh>();
            tm.text = s_players[(int)_curTurn].GetOutFieldNum().ToString();

            GameData.FieldInNumToMoverPiecesIsSame(mover.PlayerKind);
            return arr;

        }

        public static List<PiecesMoveContainer> GetPiecesMover(PLAYER_KIND kind)
        {
            return s_moveContainers[kind];
        }

        public static int GetCurTurnOutPiecess()
        {
            return s_players[(int)CurTurn].GetOutFieldNum();
        }

        public static PatternSystem.Arrange MoveContainer(PiecesMoveContainer mover, Animal animal)
        {
            PatternSystem.Arrange arr = mover.Move(animal);
            return arr;
        }

        public enum MoverAdjustKind
        {
            KILL_ONESELF,
            DESTROY,
            GOALIN,
            ADD,
        }

        public static void AdjustMover(PiecesMoveContainer mover, MoverAdjustKind destroyKind)
        {
            switch(destroyKind)
            {
                case MoverAdjustKind.GOALIN:
                    s_players[(int)mover.PlayerKind].GoalIn(mover.GetPiecesNum());
                    NextTurnCheck.Instance.GoalIn(mover.PlayerKind, s_players[(int)mover.PlayerKind].GetGoalInNum());
                    ///SoundPlayer.Instance.Play("sound0/effect/UI_Btn_Out");
                    break;
                case MoverAdjustKind.KILL_ONESELF:
                    s_players[(int)mover.PlayerKind].Out(mover.GetPiecesNum());
                    break;
                case MoverAdjustKind.DESTROY:                    
                    s_players[(int)mover.PlayerKind].Out(mover.GetPiecesNum());
                    break;
            }

            if (s_moveContainers[mover.PlayerKind].Contains(mover))
                s_moveContainers[mover.PlayerKind].Remove(mover);
        }

        public static bool FieldInNumToMoverPiecesIsSame(PLAYER_KIND kind)
        {
            int infieldNum = GameData.s_players[(int)kind].GetInFieldNum();
            int moverPiecesNum = 0;
            List<PiecesMoveContainer> movers = GameData.GetPiecesMover(kind);
            foreach (PiecesMoveContainer m in movers)
                moverPiecesNum += m.GetPiecesNum();
            if (moverPiecesNum != infieldNum)
            {
                Debug.Log(kind.ToString() + " Error !! infieldNum and MoverPiecesNum not same !!");
                return false;
            }
            return true;
        }

        public static void Player2IsCharacter1(bool isChacter1)
        {
            s_IsPlayer1IsCharcter1 = !isChacter1;
            ChacracterSetting();
            PiecesMoveContainer.CharacterSetting(s_IsPlayer1IsCharcter1);
        }

        public static void VictoryAni(PLAYER_KIND victory)
        {
            s_startPoint[(int)victory].SetActive(true);
            s_startPoint[(int)victory].GetComponent<Animator>().SetInteger("state", 11);
        }


        public static void ReSetGame(bool isRegame)
        {
            foreach (KeyValuePair<PLAYER_KIND, List<PiecesMoveContainer>> movers in s_moveContainers)
            {
                foreach (PiecesMoveContainer m in movers.Value)
                {
                    m.CurRoad._field.Mover = null;
                    GameObject.Destroy(m.Pieces);
                }
                movers.Value.Clear();
            }
            
            _curAnimals.Clear();

            for (int i = 0; i < s_startPoint.Length; ++i )
            {
                s_startPoint[i].SetActive(false);
                s_startPoint[i].GetComponent<Animator>().SetInteger("state", 1);

                TextMesh tm = GameData.s_startPoint[i].transform.FindChild("billboard_P").FindChild("Population_P").FindChild("Population_Label_P").GetComponent<TextMesh>();
                tm.text = GameData.PIECESMAX.ToString();
            }

            for (int i = 0; i < s_players.Length; ++i )
            {
                s_players[i].Reset();
            }

            if (!isRegame)
            {
                _curTurn = PLAYER_KIND.PLAYER_1;
                s_startPoint[0].SetActive(true);
                UootThrow.GetInstance().ResetGame();

                Transform gp = GameObject.Find("UI Root").transform.FindChild("Size").FindChild("GamePlay");

                gp.FindChild("Play01").FindChild("Win_Label_Count_P").GetComponent<UILabel>().text = "0";
                gp.FindChild("Play01").FindChild("Lose_Label_Count_P").GetComponent<UILabel>().text = "0";

                gp.FindChild("Play02").FindChild("Win_Label_Count_P").GetComponent<UILabel>().text = "0";
                gp.FindChild("Play02").FindChild("Lose_Label_Count_P").GetComponent<UILabel>().text = "0";
            }
            else
            {
                s_startPoint[(int)_curTurn].SetActive(true);
            }

            PriorityView.Regame(isRegame);
            if (UootThrow.s_uootAni != null)
                UootThrow.s_uootAni.SetInteger("state", 0);

            for (int i = 0; i < s_players.Length; ++i)
            {
                s_players[i] = new PlayerData((PLAYER_KIND)i, PIECESMAX);
            }

            NextTurnCheck.Instance.GameOverReset();
        }

        public static void AddCredit()
        {
            SoundPlayer.Instance.Play("sound0/effect/Coin");
            ++_cur1creditToCount;
            if(_1creditToCount == _cur1creditToCount)
            {
                _cur1creditToCount = 0;
                ++_curCreditCount;
                GameObject credit = GameObject.Find("UI Root").transform.FindChild("Size").FindChild("Credit_Group_P").gameObject;
                credit.transform.FindChild("Credit_P").GetComponent<UILabel>().text = "CREDIT " + _curCreditCount.ToString();
            }

            Calculate.Instance.AddCash();
        }

        public static bool _is4p = false;
        public static void ConsumeCredit(bool is4p = false)
        {
            if(is4p)
                _curCreditCount -= 2;
            else
                --_curCreditCount;

            _is4p = is4p;
            /*
            if(_is4p)
            {
                s_plyerControlNum = PlayerControl.Player4;
            }
            else
            {
                s_plyerControlNum = PlayerControl.Player2;
            }
            InputManager.Instance.SetPlayerNum(s_plyerControlNum);
            */

            GameObject credit = GameObject.Find("UI Root").transform.FindChild("Size").FindChild("Credit_Group_P").gameObject;
            credit.transform.FindChild("Credit_P").GetComponent<UILabel>().text = "CREDIT "+_curCreditCount.ToString();
        }

        public static int GetCreditNum()
        {
            return _curCreditCount;
        }
    }
}

