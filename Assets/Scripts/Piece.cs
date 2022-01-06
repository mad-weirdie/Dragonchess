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
        Paladin, Dwarf, Basilisk, Elemental, NULL
    };

    public class Piece : MonoBehaviour
    {
        public static Move.MoveType capture = Move.MoveType.Capture;
        public static Move.MoveType regular = Move.MoveType.Regular;
        public static Move.MoveType move_cap = Move.MoveType.MoveOrCapture;
        public static Move.MoveType swoop = Move.MoveType.Swoop;
        public bool frozen;

        public List<Move> availableMoves;
        public PieceType m_type;
        public Player player;
        public GameObject pieceGameObject;
        Color m_color;

        public int value;

        public string nameChar;
        Square m_pos;
        Board m_board;

        void Start()
		{
            availableMoves = new List<Move>();
            frozen = false;
        }

        public Piece() { }

        public Piece(PieceType type)
        {
            m_type = type;
        }

        // GetMoves to be overridden by child classes
        virtual public List<Move> GetMoves() { return null; }

        public bool IsAvailableMove(Move move)
		{
            foreach (Move m in availableMoves)
			{
                if (move == m)
                    return true;
			}
            return false;
		}

        public bool CanMoveTo(Square move)
        {
            foreach (Move m in availableMoves)
            {
                if (m.end == move)
                    return true;
            }
            return false;
        }

        virtual public void MoveTo(Square s)
        {
            // Set new position of the piece's GameObject on the board
            Vector3 pos = s.cubeObject.transform.position;
            pos.y += 1.0f / Board.square_scale;
            this.pieceGameObject.transform.position = pos;
            this.pos.dot.GetComponent<Renderer>().material = this.pos.invisible;

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

        public bool Capture(Piece enemy)
        {
            Square s = enemy.pos;
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
            (enemy.pieceGameObject).SetActive(false);

            return true;
        }

        public bool RemoteCapture(Piece enemy)
        {
            if (enemy.color == Color.White)
                GameController.P1.pieces.Remove(enemy);
            else
                GameController.P2.pieces.Remove(enemy);

            enemy.pos.occupied = false;
            (enemy.pieceGameObject).SetActive(false);
            return true;
        }

        public static Piece ObjToPiece(GameObject obj)
		{
            return obj.GetComponent<Piece>();
        }

        public Move GetMoveWithGoal(Square s)
		{
            if (s == null)
                print("error: s is null");
            foreach (Move m in availableMoves)
			{
                if (m.end == s)
                    return m;
			}
            return null;
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
