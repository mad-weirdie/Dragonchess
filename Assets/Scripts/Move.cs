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

        public void DoMove(Piece piece, Square goal)
		{

		}

        // Checks whether or not moving the king to a goal square would put
        // the king in check (an invalid move).
        public static bool WouldCheck(Piece piece, Square goal)
        {
            Piece prev_owner = null;
            bool was_occupied = goal.occupied;
            if (was_occupied)
                prev_owner = goal.piece;

            Square current = piece.pos;
            bool isbadmove = false;
            piece.MoveTo(goal);

            if (piece.color == Color.White)
            {
                if (GameController.IsCheck(GameController.P2))
                    isbadmove = true;
            }
            else
            {
                if (GameController.IsCheck(GameController.P1))
                    isbadmove = true;
            }

            piece.MoveTo(current);
            if (was_occupied)
                prev_owner.MoveTo(goal);
            return isbadmove;
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

            // check if move would put the king in check
            if (piece.type == PieceType.King && WouldCheck(piece, m.end))
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
    }
}

