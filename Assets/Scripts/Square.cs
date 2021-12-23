using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    public class Square
    {
        Color m_color;

        int m_row;
        int m_col;
        Layer m_layer;

        bool occupied;
        Piece m_piece;

        // Default constructor
        public Square()
        {
            m_row = 0;
            m_col = 0;
            m_layer = Layer.Middle;
            m_color = Color.Black;
            occupied = false;
        }

        // Constructor for square at (r, c)
        // Automatically calculates square color
        public Square(int r, int c, Layer l)
        {
            m_row = r;
            m_col = c;
            m_layer = l;

            occupied = false;
            if ((r + c) % 2 == 0)
                m_color = Color.Black;
            else
                m_color = Color.White;
        }

        public Color color
        {
            get
            {
                return m_color;
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

        public int col
        {
            get
            {
                return m_col;
            }
        }

        public int row
        {
            get
            {
                return m_row;
            }
        }

        public Layer layer
        {
            get
            {
                return m_layer;
            }
        }


        public bool IsOccupied()
        {
            return this.occupied;
        }

        public bool IsEmpty()
        {
            return !this.IsOccupied();
        }

    }

}
