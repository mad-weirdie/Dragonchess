using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    public class Move
    {
        public Piece piece;
        Square m_start;
        Square m_end;
        MoveType m_type;
        public Piece captured;

        public enum MoveType { Regular, Capture, MoveOrCapture, Swoop, NULL};

        public Move (Square start, Square end, MoveType type)
        {
            m_start = start;
            m_end = end;
            m_type = type;
        }

        public Move(Piece p, Square start, Square end, MoveType type)
        {
            piece = p;
            m_start = start;
            m_end = end;
            m_type = type;
        }

        public Square start { get { return m_start; } }

        public Square end { get { return m_end; } }

        public MoveType type { get { return m_type; } }

        // Check if a particular square is blocked from moving there
        public static bool IsBlocked(Square current, int dir, int rShift, int cShift, int b, Color c)
        {
            Board board = GetBoard(b);
            int new_row = current.row + rShift*dir;
            int new_col = current.col + cShift*dir;
            
            if (Square.IsValidSquare(new_row, new_col))
            {
                Square goal = board.squares[new_row, new_col];
                return (goal.occupied);
            }
            return false;
        }

        // Get the board of a particular index/layer value
        public static Board GetBoard(int board)
        {
            if (board == 3)
                return GameController.getUpperBoard();
            else if (board == 2)
                return GameController.getMiddleBoard();
            else
                return GameController.getLowerBoard();
        }

        // Check whether or not we can move to a particular square.
		// If so, add the move to the argument "moves" passed in.
        public static void moveAttempt(List<Move> moves, Square current,
        int dir, int rowShift, int colShift, int board, MoveType type)
        {
            if (type == MoveType.MoveOrCapture)
            {
                moveAttempt(moves, current, dir, rowShift, colShift, board, MoveType.Regular);
                moveAttempt(moves, current, dir, rowShift, colShift, board, MoveType.Capture);
                return;
            }

            Board newBoard = GetBoard(board);
            int new_row = current.row + rowShift*dir;
            int new_col = current.col + colShift*dir;
            Square endSquare;

            if (Square.IsValidSquare(new_row, new_col))
            {
                endSquare = newBoard.squares[new_row, new_col];
                Move next_move = new Move(current, endSquare, type);
                if (IsValidMove(next_move))
                {
                    moves.Add(next_move);
                }
            }
        }

        // Returns whether or not a move is valid based on the movetype
        public static bool IsValidMove (Move m)
        {
            Piece piece = m.start.piece;

            // First, check that the move goal isn't out of bounds
            if (m.m_end.col < 0 || m.m_end.col >= Board.width)
                return false;
            if (m.m_end.row < 0 || m.m_end.row >= Board.height)
                return false;

            // If movetype = Regular(move-only), check the square is free
            if (m.m_type == MoveType.Regular && m.end.IsOccupied())
                return false;
            
            // If Capture, check that the square contains enemy piece
            if (m.m_type == MoveType.Capture)
            {
                if (m.end.IsEmpty() || (m.start.piece.color == m.end.piece.color))
                        return false;
            }
            // Then, check for dragon swoop
            if (m.m_type == MoveType.Swoop)
            {
                Move swoop_move = new Move(m.m_start, m.m_end, MoveType.Capture);
                return IsValidMove(swoop_move);
            }

            return true;
        }

        public string MoveToString()
		{
            string ret = (piece.type).ToString() + ": " + start.SquareName() + "-> " + end.SquareName();

            if (type == MoveType.Capture || type == MoveType.Swoop)
			{
                ret += " , captured " + (end.piece.type).ToString();
			}

            return ret;
		}
    }
}

