using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Dragonchess
{
    public enum PieceType
    {
        Sylph, Griffon, Dragon, Warrior, Oliphant,
        Unicorn, Hero, Thief, Cleric, Mage, King,
        Paladin, Dwarf, Basilisk, Elemental
    };

    public class Piece : MonoBehaviour
    {
        public static Move.MoveType capture = Move.MoveType.Capture;
        public static Move.MoveType regular = Move.MoveType.Regular;
        public static Move.MoveType move_cap = Move.MoveType.MoveOrCapture;
        public static Move.MoveType swoop = Move.MoveType.Swoop;

        Color m_color;
        public PieceType m_type;
        public GameObject pieceGameObject;
        bool isActive;

        Square m_location;
        Board m_board;

        public Piece() { }

        public Piece(PieceType type)
        {
            m_type = type;
        }

        // GetMoves to be overridden by child classes
        virtual public ArrayList GetMoves() { return null; }

        public bool Capture(Piece enemy)
        {
            enemy.location.piece = this;
            this.location.occupied = false;
            this.location = enemy.location;
            Destroy(enemy.pieceGameObject);
            Destroy(enemy);
            return true;
        }

        public Square location
        {
            get
            {
                return m_location;
            }
            set
            {
                m_location = value;
            }
        }

        public Board board { get; set; }

        public Color color
        {
            get
            {
                return m_color;
            }
            set
            {
                m_color = value;
            }
        }

        public bool active { get; set; }

        public PieceType type
        {
            get
            {
                return m_type;
            }
            set
            {
                m_type = value;
            }
        }

    }
}
