using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    public class Square
    {
        static public string[] Letters = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l" };

        public GameObject cubeObject;
        public Material properMaterial;
        public Material invisible;
        public GameObject dot;

        public int row;
        public int col;
        public Board board;
        public Layer layer;

        public Color color;
        public bool occupied;
        public Piece piece;

        // Default constructor
        public Square()
        {
            row = 0;
            col = 0;
            color = Color.Black;
            occupied = false;
        }

        // Constructor for square at (r, c)
        // Automatically calculates square color
        public Square(int r, int c, Board b)
        {
            row = r;
            col = c;
            board = b;
            occupied = false;

            if (b.m_layer == 6)
                layer = Layer.Upper;
            else if (b.m_layer == 7)
                layer = Layer.Middle;
            else
                layer = Layer.Lower;
            
            if ((r + c) % 2 == 0)
                color = Color.Black;
            else
                color = Color.White;
        }

        public bool ContainsEnemyKing(Color c)
        {
            if (occupied)
                return ((this.piece.type == PieceType.King) && (this.piece.color != c));
            return false;
        }

        public string SquareName()
        {
            return GetColChar() + (row+1);
        }

        public string GetColChar()
        {
            return Letters[col];
        }

        public static void SetColor(Square s, Material m)
        {
            GameObject sObj = s.piece.pos.dot;
            sObj.GetComponent<Renderer>().material = m;
        }

        public bool IsOccupied()
        {
            return occupied;
        }

        public bool IsEmpty()
        {
            return !IsOccupied();
        }

        static public bool IsValidSquare(int row, int col)
        {
            if (col < 0 || col >= Board.width)
                return false;
            if (row < 0 || row >= Board.height)
                return false;

            /*
            if (m_type == MoveType.Capture && !m_end.IsOccupied())
                return false;
            */

            return true;
        }
    }
}
