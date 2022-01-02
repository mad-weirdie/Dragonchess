using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    public class Player
    {
        Color m_color;
        PlayerType m_type;
        public List<Piece> pieces;
        public bool inCheck = false;
        public Move prevMove;

        // Initialize player with color c
        public Player(Color c, PlayerType t)
        {
            m_color = c;
            m_type = t;
            pieces = new List<Piece>();
        }

        public void AddPiece(Piece p)
        {
            pieces.Add(p);
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

        public PlayerType type
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

        public bool isWhite()
        {
            return (this.color == Color.White);
        }

        public bool isBlack()
        {
            return (this.color == Color.Black);
        }
    }
}
