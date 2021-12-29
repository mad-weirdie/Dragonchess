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

        public bool RemoteCapture(Piece enemy)
        {
            enemy.location.occupied = false;
            Destroy(enemy.pieceGameObject);
            Destroy(enemy);
            return true;
        }

        public bool Capture(Piece enemy)
        {
            GameObject sObj = this.location.cubeObject;
            sObj.GetComponent<Renderer>().material = this.location.properMaterial;
            enemy.location.piece = this;
            this.location.occupied = false;
            this.location = enemy.location;
            Destroy(enemy.pieceGameObject);
            Destroy(enemy);
            return true;
        }

        virtual public void MoveTo(Square s)
        {
            // Set new position of the piece's GameObject on the board
            Vector3 pos = s.cubeObject.transform.position;
            pos.y += 1.0f / Board.square_scale;
            this.pieceGameObject.transform.position = pos;
            GameObject sObj = this.location.cubeObject;
            sObj.GetComponent<Renderer>().material = this.location.properMaterial;

            // Link piece and square to each other
            print(this.location.row + " " + this.location.col);
            this.location.occupied = false;
            this.location = s;
            this.location.occupied = true;
            s.piece = this;

            // Set proper rendering layer (for the overhead cameras)
            this.pieceGameObject.layer = s.board.m_layer + 3;
            foreach (Transform child in this.transform)
                child.gameObject.layer = s.board.m_layer + 3;
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
