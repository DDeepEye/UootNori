using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PatternSystem;



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
        public abstract void Run();
    }

    public class Kill : FieldAttribute
    {
        public override void Run()
        {
            
        }
    }

    public class AllKill : FieldAttribute
    {
        public override void Run()
        {
            
        }
    }

    public class Send1toSend2 : FieldAttribute
    {
        public override void Run()
        {

        }
    }

    public class AddPieces : FieldAttribute
    {
        public override void Run()
        {
        }
    }

    public class RemovePieces : FieldAttribute
    {
        public override void Run()
        {
        }
    }

    public class OneMoreThrow : FieldAttribute
    {
        public override void Run()
        {
        }
    }

    public class Shot: FieldAttribute
    {
        public override void Run()
        {
        }
    }

    public class ChangeWay : FieldAttribute
    {
        public override void Run()
        {
            
        }
    }
    public class Exit : FieldAttribute
    {
        public override void Run()
        {

        }
    }

    public class ExitSchedule : FieldAttribute
    {
        public override void Run()
        {

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
        static GameObject s_originPieces;
        private FieldData _curField = null;
        public int _wayKind = 0;
        public bool _isGoalin = false;
        public bool _goalinSchedule = false;
        List<PiecesData> _piecess = new List<PiecesData>();
        GameObject _pieces;
        PLAYER_KIND _playerKind;
        public PiecesMoveContainer(PLAYER_KIND kind)
        {
            _playerKind = kind;
            if(s_originPieces == null)
                s_originPieces = Resources.Load("Uoot_N") as GameObject;
            _pieces = GameObject.Instantiate(s_originPieces);
            _pieces.transform.position = GameData.GetStartRoad()._field.GetSelfField().transform.position;
        }
        public void Add(PiecesData pieces)
        {
            pieces._state = PIECES_STATE.IN_FIELD;
            _piecess.Add(pieces);
        }

        public int GetPiecesNum()
        {
            return _piecess.Count;
        }

        public PiecesData GetPieces(int index)
        {
            return _piecess[index];
        }

        public void Killed()
        {
            foreach (PiecesData pieces in _piecess)
            {
                pieces._state = PIECES_STATE.OUT_FIELD;
            }
        }

        public PatternSystem.Arrange Move(Animal animal)
        {
            List<Container> containers = new List<Container>();
            int forwardNum = GameData.GetForwardNum(animal);

            /*
            if (_goalinSchedule)
            {
                _isGoalin = true;
                containers.Add(new Timer(_pieces, 0.1f));
                containers.Add(new GoalIn(_pieces, this));
            }
            else
            {
                int forwardNum = GameData.GetForwardNum(animal);
                List<Vector3> way = GameData.GetWay(_wayKind);

                int startPoint = 0;
                if (_curField == null)
                {
                    _curField = GameData.GetWayToField(_wayKind, null, forwardNum - 1);
                }
                else
                {
                    FieldData startField;
                    if(forwardNum > 0)
                        startField = GameData.GetWayToField(_wayKind, _curField, 1);
                    else
                        startField = GameData.GetWayToField(_wayKind, _curField, -1);
                    _curField = GameData.GetWayToField(_wayKind, _curField, forwardNum);
                    startPoint = GameData.GetWayToIndex(_wayKind, startField);
                }

                Vector3 offsetPoint = _pieces.transform.position;
                if (forwardNum > 0)
                {
                    for (int i = 0; i < forwardNum; ++i)
                    {
                        if (startPoint + i >= way.Count)
                        {
                            _isGoalin = true;
                            containers.Add(new Timer(_pieces, 0.1f));
                            containers.Add(new GoalIn(_pieces, this));
                            break;
                        }
                        Vector3 p = way[startPoint + i] - offsetPoint;
                        offsetPoint = way[startPoint + i];
                        containers.Add(new Timer(_pieces, 0.1f));
                        containers.Add(new Move(_pieces, p, 0.15f));
                    }
                }
                else
                {
                    if (startPoint > 0)
                    {
                        Vector3 p = way[startPoint + forwardNum] - offsetPoint;
                        offsetPoint = way[startPoint + forwardNum];
                        containers.Add(new Timer(_pieces, 0.1f));
                        containers.Add(new Move(_pieces, p, 0.15f));
                    }
                    else
                    {
                        _goalinSchedule = true;
                        Vector3 p = GameData.GetExitField().GetSelfField().transform.position;
                        containers.Add(new Timer(_pieces, 0.1f));
                        containers.Add(new Move(_pieces, p, 0.15f));
                    }
                }
            }
            */
            return new Arrange(_pieces, Arrange.ArrangeType.SERIES, containers, 1);
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
    }

    public class FieldData
    {
        GameObject _selfField;
        List<FieldAttribute> _attributes = new List<FieldAttribute>();

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
                att.Run();
            }
        }
    }

    public class Road
    {
        public FieldData _field;
        public Road _next;
        public Road _prev;
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
        public static PlayerData[] _players = new PlayerData[(int)PLAYER_KIND.MAX];
        private static PLAYER_KIND _curTurn = PLAYER_KIND.PLAYER_1;
        public static PLAYER_KIND CurTurn {get {return _curTurn;}}
        public static FieldData[] _fields = new FieldData[FIELD_MAXNUM];

        public static Road[] s_roads = new Road[WAYKIND];

        public static List<FieldData>[] _roads = new List<FieldData>[ROAD_MAXNUM];
        public static List<List<FieldData>>[] _ways = new List<List<FieldData>>[WAYKIND];

        static Dictionary<Animal, int> s_animalToForwardNum = new Dictionary<Animal, int>();

        static Dictionary<PLAYER_KIND, List<PiecesMoveContainer>> s_moveContainers = new Dictionary<PLAYER_KIND, List<PiecesMoveContainer>>();

        public static void Init()
        {
            for(int i = 0; i < _fields.Length; ++i)
            {
                _fields[i] = new FieldData();
            }
            _fields[0].AddAttribute(new ExitSchedule());
            _fields[5].AddAttribute(new ChangeWay());

            _fields[6].AddAttribute(new Kill());
            _fields[7].AddAttribute(new AddPieces());
            _fields[9].AddAttribute(new RemovePieces());

            _fields[10].AddAttribute(new ChangeWay());

            _fields[11].AddAttribute(new AddPieces());
            _fields[12].AddAttribute(new OneMoreThrow());
            _fields[13].AddAttribute(new RemovePieces());
            _fields[14].AddAttribute(new Send1toSend2());

            _fields[15].AddAttribute(new ChangeWay());

            _fields[17].AddAttribute(new Send1toSend2());
            _fields[19].AddAttribute(new AllKill());
            _fields[20].AddAttribute(new Exit());

            _fields[22].AddAttribute(new Kill());
            _fields[23].AddAttribute(new ChangeWay());
            _fields[25].AddAttribute(new Kill());

            _fields[26].AddAttribute(new Shot());
            _fields[27].AddAttribute(new Send1toSend2());

            WayInit();

            s_animalToForwardNum.Add(Animal.DO, 1);
            s_animalToForwardNum.Add(Animal.GE, 2);
            s_animalToForwardNum.Add(Animal.KUL, 3);
            s_animalToForwardNum.Add(Animal.UOOT, 4);
            s_animalToForwardNum.Add(Animal.MO, 5);
            s_animalToForwardNum.Add(Animal.BACK_DO, -1);
            for (int i = 0; i < _players.Length; ++i)
            {
                _players[i] = new PlayerData();
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
        public static void TurnSave()
        {
        }

        public static FieldData GetExitField()
        {
            return _fields[19];
        }

        public static Road GetStartRoad()
        {
            return s_roads[0];
        }

        public static Road GetWayChangetoRoad(int wayKind, Road road)
        {
            return null;
        }

        public static Road Next(Road cur)
        {
            return cur._next;
        }

        public static Road Prev(Road cur)
        {
            return cur._prev;
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
            mover.Add(new PiecesData());
            PatternSystem.Arrange arr = mover.Move(animal);
            if(arr != null)
                s_moveContainers[_curTurn].Add(mover);
            return arr;
        }

        public static List<PiecesMoveContainer> GetPiecesMover(PLAYER_KIND kind)
        {
            return s_moveContainers[kind];
        }

        public static PatternSystem.Arrange MoveContainer(PiecesMoveContainer mover, Animal animal)
        {
            PatternSystem.Arrange arr = mover.Move(animal);
            return arr;
        }

        public enum MoverDestroyKind
        {
            DESTROY,
            GOALIN,
            ADD,
        }

        public static void DestoryMover(PiecesMoveContainer destroyMover, MoverDestroyKind destroyKind)
        {
            switch (destroyKind)
            {
                case MoverDestroyKind.ADD:
                    break;
                case MoverDestroyKind.DESTROY:
                    break;
                case MoverDestroyKind.GOALIN:
                    for(int i = 0; i < s_moveContainers.Count; ++i)
                    {
                        if (s_moveContainers[(PLAYER_KIND)i].Contains(destroyMover))
                        {
                            s_moveContainers[(PLAYER_KIND)i].Remove(destroyMover);
                            return;
                        }
                    }
                    break;
            }
        }

    }


}

