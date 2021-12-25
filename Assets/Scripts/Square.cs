using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    public class Square
    {
        GameObject cubeGameObject;
        public Material properMaterial;

        int m_row;
        int m_col;
        Board m_board;
        Layer m_layer;

        Color m_color;
        bool m_occupied;
        Piece m_piece;

        // Default constructor
        public Square()
        {
            m_row = 0;
            m_col = 0;
            m_color = Color.Black;
            m_occupied = false;
        }

        // Constructor for square at (r, c)
        // Automatically calculates square color
        public Square(int r, int c, Board b)
        {
            m_row = r;
            m_col = c;
            m_board = b;
            m_occupied = false;

            if (b.m_layer == 6)
                m_layer = Layer.Upper;
            else if (b.m_layer == 7)
                m_layer = Layer.Middle;
            else
                m_layer = Layer.Lower;
            
            if ((r + c) % 2 == 0)
                m_color = Color.Black;
            else
                m_color = Color.White;
        }

        public bool IsOccupied()
        {
            return m_occupied;
        }

        public bool IsEmpty()
        {
            return !IsOccupied();
        }

        public Board board
        {
            get
            {
                return m_board;
            }
        }

        public int col
        {
            get
            {
                return m_col;
            }
            set
            {
                m_col = value;
            }
        }

        public int row
        {
            get
            {
                return m_row;
            }
            set
            {
                m_row = value;
            }
        }

        public Layer layer
        {
            get
            {
                return m_layer;
            }
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

        public Piece piece
        {
            get
            {
                return m_piece;
            }
            set
            {
                m_piece = value;
            }
        }

        public GameObject cubeObject
        {
            get
            {
                return cubeGameObject;
            }
            set
            {
                cubeGameObject = value;
            }
        }

        public bool occupied
        {
            get
            {
                return m_occupied;
            }
            set
            {
                m_occupied = value;
            }
        }

    }

}
