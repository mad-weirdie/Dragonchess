using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Dragonchess
{
	using M = MonoBehaviour;
	public class BitPiece
	{
		public static int[] pieceValues = { 0, 1, 20, 90, 1, 15, 3, 10, 5, 20, 40, 900, 50, 2, 3, 4 };

		public static int PieceToInt(Piece piece)
		{
			byte type = (byte)((int)(piece.type));
			byte board = (byte)(piece.pos.board);
			byte row = (byte)(piece.pos.row);
			byte col = (byte)(piece.pos.col);

			// We store the color of the piece in the least significant bit of the board
			// byte (It is the only one with a small enough range value)
			if (piece.color == Color.Black)
			{
				board = (byte)(0x8 | board);
			}

			byte[] bytes = { type, board, row, col };
			int bPiece = BitConverter.ToInt32(bytes, 0);
			return bPiece;
		}

		public static int NewBitPiece(int color, int type, int board, int row, int col)
		{
			byte t = (byte)(type);
			byte b = (byte)(board);
			byte r = (byte)(row);
			byte c = (byte)(col);

			if (color == 1)
				b = (byte)(0x8 | b);
			byte[] bytes = { t, b, r, c };

			return BitConverter.ToInt32(bytes, 0);
		}

		public static Piece BitPieceToPiece(Game state, int piece)
		{
			int b = GetBoard(piece);
			int r = GetRow(piece);
			int c = GetCol(piece);
			return state.boards[b].squares[r, c].piece;
		}

		public static List<int> GetPieces(Gamestate state, bool isWhite)
		{
			List<int> pieces = new List<int>();
			for (int b = 3; b > 0; b--)
			{
				for (int i = 0; i < 96; i++)
				{
					int t = Math.Abs(state.boards[b][i]) - 1;
					int r = i / 12;
					int c = i % 12;
					if (state.boards[b][i] > 0 && isWhite)
						pieces.Add(NewBitPiece(0, t, b, r, c));
					else if (state.boards[b][i] < 0 && !isWhite)
						pieces.Add(NewBitPiece(1, t, b, r, c));
				}
			}
			return pieces;
		}

		static int[] UnpackBitPiece(int piece)
		{
			int[] ret = new int[5];
			byte[] byteArray = BitConverter.GetBytes(piece);

			byte type = byteArray[0];
			byte board = byteArray[1];
			byte row = byteArray[2];
			byte col = byteArray[3];

			int ind = (int)row * 12 + (int)col;
			int color = (board & 0x8) >> 3;
			board = (byte)(board & 0x7);
			int t = (int)type;

			ret[0] = type;
			ret[1] = color;
			ret[2] = board;
			ret[3] = row;
			ret[4] = col;

			return ret;
		}

		public static int PieceType(int piece)
		{
			return UnpackBitPiece(piece)[0];
		}

		public static int GetColor(int piece)
		{
			return UnpackBitPiece(piece)[1];
		}

		public static int GetBoard(int piece)
		{
			return UnpackBitPiece(piece)[2];
		}

		public static int GetRow(int piece)
		{
			return UnpackBitPiece(piece)[3];
		}

		public static int GetCol(int piece)
		{
			return UnpackBitPiece(piece)[4];
		}

		public static int GetIndex(int piece)
		{
			return GetRow(piece)*12 + GetCol(piece);
		}

		public static int GetPieceValue(int piece)
		{
			int[] x = UnpackBitPiece(piece);
			int pieceType = x[0];
			return pieceValues[pieceType + 1];
		}

		public static string PieceToString(int piece)
		{
			int[] x = UnpackBitPiece(piece);
			return ((Color)x[1] + " " + (PieceType)x[0] + " " + x[2] + "(" + x[3] + ", " + x[4] + ")");
		}

	}
}
