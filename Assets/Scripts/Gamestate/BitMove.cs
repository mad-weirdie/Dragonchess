using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Dragonchess
{
	using static MoveDict;
	using static ThreatDict;
	using static Move;
	using static Square;
	using M = MonoBehaviour;
	using static Zobrist;
	using static PiecePosEval;
	using static Gamestate;
	using static BitPiece;
	using static BitMoveController;

	public class BitMove
	{
		// Pack all the move data into a single integer and return it
		public static int CreateBitMove(int type, int sBoard, int sInd, int eBoard, int eInd, int flags)
		{
			int move = 0;

			move = (move | flags) << 8;
			move = (move | eInd) << 4;
			move = (move | eBoard) << 8;
			move = (move | sInd) << 4;
			move = (move | sBoard) << 4;
			move = (move | type);

			return move;
		}

		public static int MoveToBitMove(Game state, Move m)
		{
			int t = (int)m.type;
			int sB = m.start.board;
			int eB = m.end.board;
			int sI = m.start.row * 12 + m.start.col;
			int eI = m.end.row * 12 + m.end.col;
			return CreateBitMove(t, sB, sI, eB, eI, 0);
		}

		// Convert the BitMove (an integer) into an instance of the more human-readable "Move" class
		public static Move BitMoveToMove(Game state, int m)
		{
			int[] vals = UnpackBitMove(m);
			int t = vals[0];
			int sBoard = vals[1];
			int sInd = vals[2];
			int eBoard = vals[3];
			int eInd = vals[4];
			int flags = vals[5];

			Square start = state.boards[sBoard].squares[sInd / 12, sInd % 12];
			Square end = state.boards[eBoard].squares[eInd / 12, eInd % 12];
			Piece piece = start.piece;
			Move move = new Move(piece, start, end, (MoveType)t);
			if ((MoveType)t == MoveType.Capture || (MoveType)t == MoveType.Swoop)
				move.captured = state.boards[eBoard].squares[eInd / 12, eInd % 12].piece;

			return move;
		}

		// Extract all the pieces of information stored in the BitMove into an array of values
		static int[] UnpackBitMove(int move)
		{
			int[] ret = new int[6];

			int type = move & 0xF;
			int sBoard = (move >> 4) & 0xF;
			int sInd = (move >> 8) & 0xFF;
			int eBoard = (move >> 16) & 0xF;
			int eInd = (move >> 20) & 0xFF;
			int flags = (move >> 28) & 0xF;

			ret[0] = type;
			ret[1] = sBoard;
			ret[2] = sInd;
			ret[3] = eBoard;
			ret[4] = eInd;
			ret[5] = flags;

			return ret;
		}

		//===================================================================================================
		// Public functions to retrieve specific aspects of a bitmove:

		public static int BitMoveType(int move)
		{
			return move & 0xF; ;
		}

		public static int StartBoard(int move)
		{
			return UnpackBitMove(move)[1];
		}

		public static int StartIndex(int move)
		{
			return (move >> 8) & 0xFF;
		}

		public static int EndBoard(int move)
		{
			return UnpackBitMove(move)[3];
		}

		public static int EndIndex(int move)
		{
			return (move >> 20) & 0xFF;
		}

		public static int GetStartPiece(Gamestate state, int move)
		{
			int sBoard = StartBoard(move);
			int sInd = StartIndex(move);
			int startVal = state.boards[sBoard][sInd];
			return startVal;
		}

		public static int GetEndPiece(Gamestate state, int move)
		{
			int eBoard = EndBoard(move);
			int eInd = EndIndex(move);
			int endVal = state.boards[eBoard][eInd];
			return endVal;
		}

		public static bool IsValidBitMove(Gamestate state, int move, int color)
		{
			int[] vals = UnpackBitMove(move);
			MoveType type = (MoveType)vals[0];
			int startBoard = StartBoard(move);
			int endBoard = EndBoard(move);

			// First, check the validity of the move goal
			if (startBoard < 1 || startBoard > 3)
			{
				M.print("startBoard out of bounds: " + startBoard);
				return false;
			}
			if (endBoard < 1 || endBoard > 3)
			{
				M.print("endBoard out of bounds: " + endBoard);
				return false;
			}

			if (vals[4] < 0 || vals[4] > 95)
				return false;

			// If movetype = Regular(move-only), check the square is free
			if (type == MoveType.Regular && state.boards[vals[3]][vals[4]] != 0)
				return false;

			// If Capture, check that the square contains enemy piece
			if (type == MoveType.Capture)
			{
				int enemy = state.boards[vals[3]][vals[4]];

				bool movingIsWhite = color == 0;
				bool captureIsWhite = enemy > 0;
				if (enemy == 0)
					return false;
				else
				{
					if (movingIsWhite == captureIsWhite)
						return false;
				}
			}
			// Then, check for dragon swoop
			if (type == MoveType.Swoop)
			{
				int swoop_move = (int)(move & 0xFFFFFFF0) | ((int)MoveType.Capture);
				return IsValidBitMove(state, swoop_move, color);
			}

			return true;
		}

		public static string BitMoveToString(Gamestate state, int move)
		{
			string binary = Convert.ToString(move, 2).PadLeft(32, '0');
			string betterBin = "";
			for (int i = 0; i < 32; i++)
			{
				betterBin += binary[i];
				if ((i + 1) % 4 == 0)
					betterBin += ' ';
			}
			//M.print("binary: " + betterBin);
			int[] vals = UnpackBitMove(move);
			int type = vals[0];
			int sBoard = vals[1];
			int sInd = vals[2];
			int eBoard = vals[3];
			int eInd = vals[4];
			int flags = vals[5];

			Color color;
			if (state.boards[sBoard][sInd] > 0)
				color = Color.White;
			else
				color = Color.Black;

			PieceType ptype = (PieceType)(Math.Abs(state.boards[sBoard][sInd]) - 1);
			int r1 = sInd / 12;
			int c1 = sInd % 12;
			int r2 = eInd / 12;
			int c2 = eInd % 12;

			MoveType mtype = (MoveType)type;
			string ret = (color + " " + ptype + ": " + sBoard + Letters[c1] +
				(1 + r1) + "->" + eBoard + Letters[c2] + (1 + r2) + " : " + mtype);
			return ret;
		}

		public static int EvaluateBitMove(Gamestate state, int move)
		{
			int start = GetStartPiece(state, move);
			int end = GetEndPiece(state, move);

			DoBitMove(ref state, move);
			int ret = EvaluateGamestate(state);
			UndoBitMove(ref state, move, start, end);
			return ret;
		}
	}
}
