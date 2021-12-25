using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    public class Move
    {
        MoveType m_type;
        Square m_start;
        Square m_end;

        public enum MoveType { Regular, Capture };

        public Move (Square start, Square end, MoveType type)
        {
            m_start = start;
            m_end = end;
            m_type = type;
        }

        public Square start
        {
            get
            {
                return m_start;
            }
        }

        public Square end
        {
            get
            {
                return m_end;
            }
        }

        public MoveType type
        {
            get
            {
                return m_type;
            }
        }

        public bool IsValidMove ()
        {

            if (m_start.col < 0 || m_start.col >= Board.width)
                return false;
            if (m_start.row < 0 || m_start.row >= Board.height)
                return false;
            if (m_end.col < 0 || m_end.col >= Board.width)
                return false;
            if (m_end.row < 0 || m_end.row >= Board.height)
                return false;

            /*
            if (m_type == MoveType.Capture && !m_end.IsOccupied())
                return false;
            */

            return true;
        }
    }
}

