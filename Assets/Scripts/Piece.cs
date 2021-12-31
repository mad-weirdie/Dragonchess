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
        public bool frozen = false;

        Square m_pos;
        Board m_board;

        public Piece() { }

        public Piece(PieceType type)
        {
            m_type = type;
        }

        // GetMoves to be overridden by child classes
        virtual public List<Move> GetMoves() { return null; }

        public List<Square> GetThreats()
        {
            List<Square> threats = new List<Square>();
            foreach (Move move in this.GetMoves())
            {
                if (move.type == Move.MoveType.Capture)
                    threats.Add(move.end);
            }
            return threats;
        }

        public bool ThreatToKing()
        {
            foreach (Square s in this.GetThreats())
            {
                if (s.ContainsEnemyKing(this.color))
                {
                    print("contains enemy king.");
                    return true;
                }
            }
            return false;
        }

        public bool RemoteCapture(Piece enemy)
        {
            enemy.pos.occupied = false;
            Destroy(enemy.pieceGameObject);
            Destroy(enemy);
            return true;
        }

        public bool Capture(Piece enemy)
        {
            if (enemy.type == PieceType.Basilisk)
            {
                if (GameController.getMiddleBoard().squares[enemy.row, enemy.col].occupied)
                {
                    Square basilisk = GameController.getLowerBoard().squares[enemy.row, enemy.col];
                    Square above = GameController.getMiddleBoard().squares[enemy.row, enemy.col];

                    print("captured basilisk. unfreezing square.");
                    basilisk.piece.pieceGameObject.GetComponent<Basilisk>().UnfreezeSquare(above);
                    above.piece.frozen = false;
                }
            }
            if (enemy.color == Color.White)
                GameController.P1.pieces.Remove(enemy);
            else
                GameController.P2.pieces.Remove(enemy);

            this.pos.dot.GetComponent<Renderer>().material = this.pos.invisible;
            enemy.pos.piece = this;
            this.pos.occupied = false;
            this.pos = enemy.pos;
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
            this.pos.dot.GetComponent<Renderer>().material = this.pos.properMaterial;

            // Link piece and square to each other
            this.pos.occupied = false;
            this.pos = s;
            this.pos.occupied = true;
            s.piece = this;

            // Set proper rendering layer (for the overhead cameras)
            this.pieceGameObject.layer = s.board.m_layer + 3;
            foreach (Transform child in this.transform)
                child.gameObject.layer = s.board.m_layer + 3;

            if (s.board == GameController.getMiddleBoard())
            {
                if (GameController.getLowerBoard().squares[s.row, s.col].occupied)
                {
                    Piece checkB = GameController.getLowerBoard().squares[s.row, s.col].piece;
                    if (checkB.type == PieceType.Basilisk)
                    {
                        GameController.getLowerBoard().squares[s.row, s.col].piece.pieceGameObject.GetComponent<Basilisk>().FreezeSquare(s);
                        this.frozen = true;
                    }
                }
            }
        }

        public Square pos
        {
            get
            {
                return m_pos;
            }
            set
            {
                m_pos = value;
            }
        }

        public int row
        {
            get
            {
                return m_pos.row;
            }
        }

        public int col
        {
            get
            {
                return m_pos.col;
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
