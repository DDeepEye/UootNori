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

    public class Send2toSend1 : FieldAttribute
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

    public class ChangeWay : FieldAttribute
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

    public class GameData
    {
        public const int PIECESMAX = 5;
        public const int PLAYERNUM = 2;

        public static List<Animal> _curAnimals = new List<Animal>();
        public static PlayerData[] _players = { new PlayerData(), new PlayerData() };
    }


}


