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

        public enum MoveType { Regular, Capture, MoveOrCapture, Swoop };

        public Move (Square start, Square end, MoveType type)
        {
            m_start = start;
            m_end = end;
            m_type = type;
        }

        public Square start { get { return m_start; } }

        public Square end { get { return m_end; } }

        public MoveType type { get { return m_type; } }

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
            MonoBehaviour.print("new_row: " + new_row + "   new_col: " + new_col);
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
            // First, check that the move goal isn't out of bounds
            if (m_end.col < 0 || m_end.col >= Board.width)
                return false;
            if (m_end.row < 0 || m_end.row >= Board.height)
                return false;

            // If movetype = Regular(move-only), check the square is free
            if (m_type == MoveType.Regular)
            {
                if (end.IsOccupied())
                    return false;
            }

            // If movetype = Capture, check the square is occupied by another
            // piece of the opposing team's color
            if (m_type == MoveType.Capture)
            {
                if (end.IsEmpty())
                {
                    return false;
                }
                else
                {
                    if (start.piece.color == end.piece.color)
                        return false;
                }
            }

            /*
            if (m_type == MoveType.Capture && !m_end.IsOccupied())
                return false;
            */

            return true;
        }
    }
}

