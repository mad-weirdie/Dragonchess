using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    public class Player
    {
        Color m_color;
        public enum Color { White, Black };

        // Initialize player with color c
        public Player(Color c)
        {
            m_color = c;
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
