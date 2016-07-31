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
            GameObject.Destroy(mover.Pieces);

            GameData.FieldInNumToMoverPiecesIsSame(mover.PlayerKind);
        }
    }

    public class AllKill : FieldAttribute
    {
        public override void Run(PiecesMoveContainer mover)
        {
            for(PLAYER_KIND i = PLAYER_KIND.PLAYER_1; i < PLAYER_KIND.MAX; ++i)
            {
                List<PiecesMoveContainer> movers = GameData.GetPiecesMover(i);
                foreach (PiecesMoveContainer m in movers)
                {
                    GameData.s_players[(int)i].Out(m.GetPiecesNum());
                    GameObject.Destroy(m.Pieces);
                    m.CurRoad._field.Mover = null;
                }
                movers.Clear();

                GameData.FieldInNumToMoverPiecesIsSame(i);
            }
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
            mover.Containers.Containers.Add(new Timer(null, 0.1f));
            mover.Containers.Containers.Add(new Move(mover.Pieces, p, 0.15f));
            mover.Containers.Containers.Add(new Timer(null, 0.1f));
            mover.Containers.Containers.Add(new FieldSet(_sendRoad._field, mover));            
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

    public class Shot: FieldAttribute
    {
        public override void Run(PiecesMoveContainer mover)
        {
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
                }
            }
            _field.Mover = _mover;
            _field.FieldRun();
            Debug.Log(GameData.CurTurn.ToString() + " FieldIn -> " + GameData.s_players[(int)GameData.CurTurn].GetInFieldNum().ToString() + " FieldOut -> " + GameData.s_players[(int)GameData.CurTurn].GetOutFieldNum().ToString());
        }
    }


    public class UootThrowPlayer : Container
    {
        string _state;
        public UootThrowPlayer(string state)
        {
            _state = state;
        }
        public override void Run()
        {
            if (IsDone)
                return;
            _isDone = true;
            UootThrow.s_uootAni.Play(_state);                       
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

        string[] _moverName = {"Uoot_N", "Uoot_N (1)"};
        public PiecesMoveContainer(PLAYER_KIND kind)
        {
            _playerKind = kind;
            if(s_originPiecess[(int)GameData.CurTurn] == null)
                s_originPiecess[(int)GameData.CurTurn] = Resources.Load(_moverName[(int)GameData.CurTurn]) as GameObject;
            _pieces = GameObject.Instantiate(s_originPiecess[(int)GameData.CurTurn]);

            _curRoad = GameData.GetStartRoad();
            _pieces.transform.position = _curRoad._field.GetSelfField().transform.position;
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
        }

        public int GetPiecesNum()
        {
            return _piecesNum;
        }

        public PatternSystem.Arrange StartPointToStay()
        {
            List<Container> containers = new List<Container>();
            _curRoad = GameData.GetStartRoad();
            containers.Add(new Timer(_pieces, 0.3f));
            containers.Add(new FieldSet(_curRoad._field, this));
            _arrange = new Arrange(_pieces, Arrange.ArrangeType.SERIES, containers, 1);
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

            _animal = animal;

            if (_goalinSchedule)
            {
                if(animal == Animal.BACK_DO)
                {
                    _curRoad = GameData.GetAllKillRoad();
                    Vector3 offsetPoint = _pieces.transform.position;
                    Vector3 p = _curRoad._field.GetSelfField().transform.position - offsetPoint;
                    containers.Add(new Timer(_pieces, 0.1f));
                    containers.Add(new Move(_pieces, p, 0.15f));
                    containers.Add(new Timer(_pieces, 0.1f));
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
                            offsetPoint = _curRoad._field.GetSelfField().transform.position;
                            containers.Add(new Timer(null, 0.1f));
                            containers.Add(new Move(_pieces, p, 0.15f));
                        }
                    }
                    
                    if (!_isGoalin)
                    {
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
                        containers.Add(new Timer(_pieces, 0.1f));
                        containers.Add(new Move(_pieces, p, 0.15f));
                    }

                    if (!_isGoalin)
                    {
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
        public PlayerData(int piecesMax = GameData.PIECESMAX)
        {
            _pieces = new PiecesData[piecesMax];
            for(int i = 0; i < piecesMax; ++i)
            {
                _pieces[i] = new PiecesData();
            }
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
        public const int PIECESMAX = 5;

        public const int FIELD_MAXNUM = 30;
        public const int ROAD_MAXNUM = 8;
        public const int WAYKIND = 4;

        public static List<Animal> _curAnimals = new List<Animal>();
        public static PlayerData[] s_players = new PlayerData[(int)PLAYER_KIND.MAX];
        private static PLAYER_KIND _curTurn = PLAYER_KIND.PLAYER_1;
        public static PLAYER_KIND CurTurn {get {return _curTurn;}}
        public static FieldData[] _fields = new FieldData[FIELD_MAXNUM];

        public static Road[] s_roads = new Road[WAYKIND];

        public static Road s_allKillRoad;

        public static List<FieldData>[] _roads = new List<FieldData>[ROAD_MAXNUM];
        public static List<List<FieldData>>[] _ways = new List<List<FieldData>>[WAYKIND];

        static Dictionary<Animal, int> s_animalToForwardNum = new Dictionary<Animal, int>();

        static Dictionary<PLAYER_KIND, List<PiecesMoveContainer>> s_moveContainers = new Dictionary<PLAYER_KIND, List<PiecesMoveContainer>>();

        static bool s_oneMoreUootThrow = false;
        public static bool IsOneMoreUootThrow { get { return s_oneMoreUootThrow; } }
        public static void OneMoreUootThrowCheck() { s_oneMoreUootThrow = false; }
        public static void OneMoreUootThrow() { s_oneMoreUootThrow = true; }

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

            _fields[26].AddAttribute(new Shot());

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
                s_players[i] = new PlayerData();
                s_moveContainers.Add((PLAYER_KIND)i, new List<PiecesMoveContainer>());
            }
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

        public static void Reset()
        {
            
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

        public static Animal GetAnimal(int index)
        {
            return _curAnimals[index];
        }

        public static void RemoveAnimal(int index)
        {
            _curAnimals.RemoveAt(index);
        }

        public static Animal GetLastAnimal()
        {
            if(_curAnimals.Count > 0)
                return _curAnimals[_curAnimals.Count - 1];
            return Animal.NONE;
        }

        public static int GetForwardNum(Animal animal)
        {
            if(!s_animalToForwardNum.ContainsKey(animal))
                Debug.Log("Forward Animal : " + ((int)animal).ToString());
                
            return s_animalToForwardNum[animal];
        }

        public static void NextTurn()
        {
            _curTurn = _curTurn == PLAYER_KIND.PLAYER_1 ? PLAYER_KIND.PLAYER_2 : PLAYER_KIND.PLAYER_1;
        }

        public static PatternSystem.Arrange NewInField(Animal animal)
        {
            PiecesMoveContainer mover = new PiecesMoveContainer(_curTurn);
            PatternSystem.Arrange arr = null;
            if (animal != Animal.BACK_DO)
                arr = mover.Move(animal);
            else
                arr = mover.StartPointToStay();

            if(arr != null)
                s_moveContainers[_curTurn].Add(mover);

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

    }


}

