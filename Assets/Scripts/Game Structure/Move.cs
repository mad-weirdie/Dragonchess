using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
	using static MoveDict;
	using M = MonoBehaviour;
    public enum MoveType { Regular, Capture, MoveOrCapture, Swoop, NULL };

    public class Move
    {
        public Piece piece;
        public Square start;
        public Square end;
        public MoveType type;
        public Piece captured;

        public Move(Piece p, Square s, Square e, MoveType t)
        {
            piece = p;
            start = s;
            end = e;
            type = t;
        }

        public bool IsType(MoveType t)
		{
            return (this.type == t);
		}

        // Check if a particular square is blocked from moving there
        public static bool IsBlocked(Gamestate state, Square current, int dir, int rShift, int cShift, int b, Color c)
        {
            Board board = GetBoard(state, b);
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
        public static Board GetBoard(Gamestate state, int board)
        {
            if (board == 3)
                return state.upperBoard;
            else if (board == 2)
                return state.middleBoard;
            else
                return state.lowerBoard;
        }

		public static void AddMoves(Gamestate state, List<(int,int,int)> possibleMoves, List<Move> moves, Square current, MoveType type)
		{
			if(type == MoveType.MoveOrCapture)
			{
				AddMoves(state, possibleMoves, moves, current, MoveType.Regular);
				AddMoves(state, possibleMoves, moves, current, MoveType.Capture);
				return;
			}

			foreach ((int b, int r, int c) in possibleMoves) {
				Board newBoard = GetBoard(state, b);
				Square end = newBoard.squares[r, c];
				Move next_move = new Move(current.piece, current, end, type);

				if (next_move.type == MoveType.Capture || next_move.type == MoveType.Swoop)
					next_move.captured = next_move.end.piece;

				if (IsValidMove(next_move))
				{
					moves.Add(next_move);
				}
			}
		}

		public static List<(int,int,int)> GetBlocked(Gamestate state, List<(int,int,int)> moves, List<string> slideTypes, Square current)
		{
			List<(int, int, int)> illegal = new List<(int, int, int)>();

			foreach (string t in slideTypes)
			{
				bool blocked = false;
				foreach (var x in MoveDictionary[t][current.board, current.row, current.col])
				{
					if (blocked)
					{
						illegal.Add(x);
					}
					else
					{
						int b = x.Item1;
						int r = x.Item2;
						int c = x.Item3;
						Board board = GetBoard(state, b);
						if (board.squares[r, c].occupied)
						{
							blocked = true;
						}
					}
				}
			}
			return illegal;
		}

		public static List<(int, int, int)> GetBlocked(SimpleState state, List<(int, int, int)> moves, List<string> slideTypes, int board, int row, int col)
		{
			List<(int, int, int)> illegal = new List<(int, int, int)>();

			foreach (string t in slideTypes)
			{
				bool blocked = false;
				foreach (var x in MoveDictionary[t][board, row, col])
				{
					if (blocked)
					{
						illegal.Add(x);
					}
					else
					{
						int b = x.Item1;
						int r = x.Item2;
						int c = x.Item3;
						if (state.boards[board][r*12+c] != 0)
						{
							blocked = true;
						}
					}
				}
			}
			return illegal;
		}

		public static void RemoveBlocked(Gamestate state, List<(int,int,int)> blocked, List<Move> moves)
		{
			List<Move> toRemove = new List<Move>();
			foreach(Move move in moves)
			{
				var tup = (move.end.board, move.end.row, move.end.col);
				if (blocked.Contains(tup))
					toRemove.Add(move);
			}
			foreach (Move illegal in toRemove)
				moves.Remove(illegal);
		}

		// Check whether or not we can move to a particular square.
		// If so, add the move to the argument "moves" passed in.
		public static void moveAttempt(Gamestate state, List<Move> moves, Square current,
        int dir, int rowShift, int colShift, int board, MoveType type)
        {
            if (type == MoveType.MoveOrCapture)
            {
                moveAttempt(state, moves, current, dir, rowShift, colShift, board, MoveType.Regular);
                moveAttempt(state, moves, current, dir, rowShift, colShift, board, MoveType.Capture);
                return;
            }

            Board newBoard = GetBoard(state, board);
            int new_row = current.row + rowShift*dir;
            int new_col = current.col + colShift*dir;
            Square endSquare;

            if (Square.IsValidSquare(new_row, new_col))
            {
                endSquare = newBoard.squares[new_row, new_col];
                Move next_move = new Move(current.piece, current, endSquare, type);

                if (next_move.type == MoveType.Capture || next_move.type == MoveType.Swoop)
                    next_move.captured = next_move.end.piece;

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
            if (m.end.col < 0 || m.end.col >= Board.width)
                return false;
            if (m.end.row < 0 || m.end.row >= Board.height)
                return false;

            // If movetype = Regular(move-only), check the square is free
            if (m.type == MoveType.Regular && m.end.IsOccupied())
                return false;
            
            // If Capture, check that the square contains enemy piece
            if (m.type == MoveType.Capture)
            {
                if (m.end.IsEmpty() || (m.start.piece.color == m.end.piece.color))
                        return false;
            }
            // Then, check for dragon swoop
            if (m.type == MoveType.Swoop)
            {
                Move swoop_move = new Move(m.start.piece, m.start, m.end, MoveType.Capture);
                return IsValidMove(swoop_move);
            }

            return true;
        }

        public string MoveToString()
		{
            string ret = piece.color + " " + (piece.type).ToString() + ": " + start.SquareName() + "-> " + end.SquareName();

            if (type == MoveType.Capture || type == MoveType.Swoop)
			{
                ret += " , captured: " + captured.color + " " + (captured.type).ToString();
			}

            return ret;
		}

		public string MoveToLogString(bool wasCheck)
		{
			string moveText = this.piece.nameChar;
			//moveText += (this.start.SquareName());
			// If the move was a capture, indicate with an 'x'
			if (this.type == MoveType.Capture)
			{
				moveText += "x";
				moveText += this.captured.nameChar;
			}
			// If the move was a remote capture, indicate with an '-'
			if (this.type == MoveType.Swoop)
				moveText += "#";
			// Coordinates of the square the piece moved to
			moveText += this.end.SquareName();
			// If the move put the opponent in check, indicate with a '+'
			if (wasCheck)
				moveText += "+";

			return moveText;
		}

		public static Move ConvertMove(Gamestate state, Move move)
		{
			Board startBoard;
			Board endBoard;

			if (move.start.board == 3)
				startBoard = state.upperBoard;
			else if (move.start.board == 2)
				startBoard = state.middleBoard;
			else
				startBoard = state.lowerBoard;

			if (move.end.board == 3)
				endBoard = state.upperBoard;
			else if (move.end.board == 2)
				endBoard = state.middleBoard;
			else
				endBoard = state.lowerBoard;

			Square start = startBoard.squares[move.start.row, move.start.col];
			Square end = endBoard.squares[move.end.row, move.end.col];
			Piece piece = start.piece;

			Move converted = new Move(piece, start, end, move.type);

			if (move.captured != null)
			{
				Board capturedBoard;

				if (move.end.board == 3)
					capturedBoard = state.upperBoard;
				else if (move.end.board == 2)
					capturedBoard = state.middleBoard;
				else
					capturedBoard = state.lowerBoard;

				Piece cap = capturedBoard.squares[move.end.row, move.end.col].piece;
				converted.captured = cap;
			}

			return converted;
		}
	}
}

