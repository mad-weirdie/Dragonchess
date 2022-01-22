using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    public class Square
    {
        static public string[] Letters = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l" };

        public int row;
        public int col;
        public int board;
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
            board = b.layer_int_val;
            occupied = false;

            if (b.layer_int_val == 3)
                layer = Layer.Upper;
            else if (b.layer_int_val == 2)
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
            return (board) + GetColChar() + (row+1);
        }

        public string GetColChar()
        {
            return Letters[col];
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
            return true;
        }
    }
}
