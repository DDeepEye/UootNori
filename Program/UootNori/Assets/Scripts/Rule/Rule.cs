using UnityEngine;
using System.Collections;
using System.Collections.Generic;



namespace UootNori
{
    public enum Animal
    {
        DO,
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

    public struct PlayerData
    {   
        public PlayerData(int piecesMax = GameData.PIECESMAX)
        {
            _pieces = new GameObject[piecesMax];
            _piecesNum = piecesMax;
        }

        public GameObject[] _pieces;
        public int _piecesNum;
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
        public const int PLAYERNUM = 2;
        public const int FIELD_MAXNUM = 29;
        public const int ROAD_MAXNUM = 8;
        public const int WAYKIND = 4;

        public static List<Animal> _curAnimals = new List<Animal>();
        public static PlayerData[] _players = { new PlayerData(), new PlayerData() };
        public static FieldData[] _fields = new FieldData[FIELD_MAXNUM];

        public static List<FieldData>[] _roads = new List<FieldData>[ROAD_MAXNUM];
        public static List<List<FieldData>>[] _ways = new List<List<FieldData>>[WAYKIND];

        static Dictionary<Animal, int> s_animalToForwardNum = new Dictionary<Animal, int>();

        public static void Init()
        {
            for(int i = 0; i < _fields.Length; ++i)
            {
                _fields[i] = new FieldData();
            }
            _fields[4].AddAttribute(new ChangeWay());

            _fields[5].AddAttribute(new Kill());
            _fields[6].AddAttribute(new AddPieces());
            _fields[8].AddAttribute(new RemovePieces());

            _fields[9].AddAttribute(new ChangeWay());

            _fields[10].AddAttribute(new AddPieces());
            _fields[11].AddAttribute(new OneMoreThrow());
            _fields[12].AddAttribute(new RemovePieces());
            _fields[13].AddAttribute(new Send1toSend2());

            _fields[14].AddAttribute(new ChangeWay());

            _fields[16].AddAttribute(new Send1toSend2());
            _fields[18].AddAttribute(new AllKill());
            _fields[19].AddAttribute(new Exit());

            _fields[21].AddAttribute(new Kill());
            _fields[22].AddAttribute(new ChangeWay());
            _fields[24].AddAttribute(new Kill());

            _fields[25].AddAttribute(new Shot());
            _fields[26].AddAttribute(new Send1toSend2());

            WayInit();

            s_animalToForwardNum.Add(Animal.DO, 1);
            s_animalToForwardNum.Add(Animal.GE, 2);
            s_animalToForwardNum.Add(Animal.KUL, 3);
            s_animalToForwardNum.Add(Animal.UOOT, 4);
            s_animalToForwardNum.Add(Animal.MO, 5);
        }

        private static void WayInit()
        {
            for (int i = 0; i < _fields.Length; ++i )
            {
                _fields[i].SetField(FieldAdder.GetFields(i));
            }

            _roads[0] = new List<FieldData>();
            for (int i = 0; i < 5; ++i)
            {   
                _roads[0].Add(_fields[i]);
            }

            _roads[1] = new List<FieldData>();
            for (int i = 5; i < 10; ++i)
            {
                _roads[1].Add(_fields[i]);
            }

            _roads[2] = new List<FieldData>();
            for (int i = 10; i < 15; ++i)
            {
                _roads[2].Add(_fields[i]);
            }

            _roads[3] = new List<FieldData>();
            for (int i = 15; i < 20; ++i)
            {
                _roads[3].Add(_fields[i]);
            }

            _roads[4] = new List<FieldData>();
            for (int i = 20; i < 23; ++i)
            {
                _roads[4].Add(_fields[i]);
            }

            _roads[5] = new List<FieldData>();
            for (int i = 23; i < 25; ++i)
            {
                _roads[5].Add(_fields[i]);
            }
            _roads[5].Add(_fields[19]);

            _roads[6] = new List<FieldData>();
            for (int i = 27; i < 29; ++i)
            {
                _roads[6].Add(_fields[i]);
            }
            _roads[6].Add(_fields[14]);

            _roads[7] = new List<FieldData>();
            for (int i = 25; i < 27; ++i)
            {
                _roads[7].Add(_fields[i]);
            }
            _roads[7].Add(_fields[22]);

            _ways[0] = new List<List<FieldData>>();
            _ways[0].Add(_roads[0]);
            _ways[0].Add(_roads[1]);
            _ways[0].Add(_roads[2]);
            _ways[0].Add(_roads[3]);

            _ways[1] = new List<List<FieldData>>();
            _ways[1].Add(_roads[0]);
            _ways[1].Add(_roads[4]);
            _ways[1].Add(_roads[5]);

            _ways[2] = new List<List<FieldData>>();
            _ways[2].Add(_roads[0]);
            _ways[2].Add(_roads[4]);
            _ways[2].Add(_roads[6]);
            _ways[2].Add(_roads[3]);

            _ways[3] = new List<List<FieldData>>();
            _ways[3].Add(_roads[0]);
            _ways[3].Add(_roads[1]);
            _ways[3].Add(_roads[7]);
            _ways[3].Add(_roads[5]);
        }
        public static void TurnSave()
        {
        }

        public static FieldData GetExitField()
        {
            return _fields[19];
        }

        public static List<Vector3> GetWay(int index)
        {
            List<Vector3> way = new List<Vector3>();
            foreach (List<FieldData> fields in _ways[index])
            {
                foreach (FieldData field in fields)
                {
                    way.Add(field.GetSelfField().transform.position);
                }
            }
            return way;
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

        public static int GetForwardNum(Animal animal)
        {
            return s_animalToForwardNum[animal];
        }
    }


}

