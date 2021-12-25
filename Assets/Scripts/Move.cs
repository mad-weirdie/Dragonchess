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

        public static void moveAttempt(ArrayList moves, Square current,
        int dir, int rowShift, int colShift, int board, MoveType type)
        {
            Board newBoard;
            Square endSquare;
            rowShift = rowShift * dir;
            colShift = colShift * dir;

            if (board == 3)
                newBoard = GameController.getUpperBoard();
            else if (board == 2)
                newBoard = GameController.getMiddleBoard();
            else
                newBoard = GameController.getLowerBoard();

            int new_row = current.row + rowShift;
            int new_col = current.col + colShift;
            if (Square.IsValidSquare(new_row, new_col))
            {
                endSquare = newBoard.squares[new_row, new_col];
                Move next_move = new Move(current, endSquare, type);
                if (next_move.IsValidMove())
                {
                    moves.Add(next_move);
                }
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

