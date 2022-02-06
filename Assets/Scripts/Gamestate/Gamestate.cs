using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace Dragonchess
{
	using static MoveDict;
	using static ThreatDict;
	using static Move;
	using static Square;
	using M = MonoBehaviour;
	using static Zobrist;
	using static PiecePosEval;
	using static Game;
	using static BitPiece;
	using static BitMove;
	using static BitMoveController;

	public class Gamestate
	{
		public static char[] pieceCodes = { '.', 's', 'g', 'r', 'w', 'o', 'u', 'h', 't', 'c', 'm', 'k', 'p', 'd', 'b', 'e' };

		public int[][] boards;
		public int[] b3;
		public int[] b2;
		public int[] b1;

		public bool WhiteToMove;
		Status gameStatus;

		static public Dictionary<long, int> stateEvals;
		static public Dictionary<long, bool> isCheckDict;

		static Gamestate()
		{
			stateEvals = new Dictionary<long, int>();
			isCheckDict = new Dictionary<long, bool>();
		}

		public Gamestate()
		{
			boards = new int[4][];
			b3 = new int[96];
			b2 = new int[96];
			b1 = new int[96];

			boards[3] = b3;
			boards[2] = b2;
			boards[1] = b1;

			for (int i = 0; i < 96; i++)
			{
				b3[i] = 0;
				b2[i] = 0;
				b1[i] = 0;
			}

			WhiteToMove = true;
			gameStatus = Status.Normal;
		}

		public Gamestate(Game state)
		{
			boards = new int[4][];

			b3 = new int[96];
			b2 = new int[96];
			b1 = new int[96];

			boards[3] = b3;
			boards[2] = b2;
			boards[1] = b1;

			if (state.ActivePlayer.color == Color.White)
				WhiteToMove = true;
			else
				WhiteToMove = false;

			for (int i = 0; i < 96; i++)
			{
				b3[i] = 0;
				b2[i] = 0;
				b1[i] = 0;
			}

			for (int i = 0; i < state.P1.pieces.Count; i++)
			{
				Piece piece = state.P1.pieces[i];
				int bPiece = PieceToInt(piece);
				int ind = piece.pos.row * 12 + piece.pos.col;

				if (piece.pos.board == 3)
					b3[ind] = 1 * ((int)piece.type + 1);
				else if (piece.pos.board == 2)
					b2[ind] = 1 * ((int)piece.type + 1);
				else
					b1[ind] = 1 * ((int)piece.type + 1);
			}

			for (int i = 0; i < state.P2.pieces.Count; i++)
			{
				Piece piece = state.P2.pieces[i];
				int bPiece = PieceToInt(piece);
				int ind = piece.pos.row * 12 + piece.pos.col;

				if (piece.pos.board == 3)
					b3[ind] = (-1) * ((int)piece.type + 1);
				else if (piece.pos.board == 2)
					b2[ind] = (-1) * ((int)piece.type + 1);
				else
					b1[ind] = (-1) * ((int)piece.type + 1);
			}
			UpdateGameStatus(this);
			gameStatus = GetGameStatus(this);
		}

		public static Gamestate NewGame()
		{
			Gamestate state = new Gamestate();
			string[] lines = File.ReadAllLines("Assets/Resources/state_init.txt");

			// This iterates through the 3 boards in the file
			for (int ind = 0; ind <= 18; ind += 9)
			{
				// First by row...
				for (int row = 0; row < 8; row++)
				{
					string[] chars = lines[row + ind].Trim().Split(' ');
					// Then by column...
					for (int col = 0; col < chars.Length; col++)
					{
						char c = chars[col][0];
						if (c != '.')
						{
							int board = 3 - (ind / 9);
							int piece_type = LoadState.pieceCodeDict[c] + 1;
							// Add the new piece, linking it to a piece GameObject as well
							if (Char.IsLower(c))
								state.boards[board][row * 12 + col] = piece_type;
							else
								state.boards[board][row * 12 + col] = -1*piece_type;
						}
					}
				}
			}
			return state;

		}

		public static int EvaluateGamestate(Gamestate state)
		{
			long stateHash = GetStateHash(state);
			int ret = 0;

			if (stateEvals.ContainsKey(stateHash))
				return stateEvals[stateHash];

			List<int> p1 = GetPieces(state, true);
			List<int> p2 = GetPieces(state, false);

			// Evaluation based on current player 1 pieces and locations
			for (int i = 0; i < p1.Count; i++)
			{
				int p = p1[i];
				int t = PieceType(p);
				int b = GetBoard(p);
				int r = GetRow(p);
				int c = GetCol(p);

				ret += GetPieceValue(p);
				ret += PositionEval[t][3 - b, r * 12 + c];
			}

			// Evaluation based on current player 2 pieces and locations
			for (int i = 0; i < p2.Count; i++)
			{
				int p = p2[i];
				int t = PieceType(p);
				int b = GetBoard(p);
				int r = GetRow(p);
				int c = GetCol(p);

				ret -= GetPieceValue(p);
				// The evaluation table is flipped for black pieces
				int flipRow = 7 - r;
				ret -= PositionEval[t][3 - b, flipRow * 12 + c];
			}

			// Check for checkmate:
			if (state.gameStatus == Status.Checkmate && state.WhiteToMove)
				ret -= 5000000;

			if (state.gameStatus == Status.Checkmate && !state.WhiteToMove)
				ret += 5000000;

			stateEvals[stateHash] = ret;
			return ret;
		}

		public static void UpdateGameStatus(Gamestate state)
		{
			if (IsCheck(state, state.WhiteToMove))
			{
				if (IsCheckmate(state, state.WhiteToMove))
					state.gameStatus = Status.Checkmate;
				else
					state.gameStatus = Status.Check;
			}
			else if (GetPossibleMoves(state, state.WhiteToMove).Count == 0)
				state.gameStatus = Status.Stalemate;
			else
				state.gameStatus = Status.Normal;
		}

		public static Status GetGameStatus(Gamestate state)
		{
			return state.gameStatus;
		}

		// Evaluates and returns whether or not Player p's king is in check
		public static bool IsCheck(Gamestate state, bool whiteKing)
		{
			int king = GetKing(state, whiteKing);
			if (king == -1) return true;
			List<int> possibleThreats = ThreatsInRange(state, whiteKing);
			(int b, int i) = (GetBoard(king), GetIndex(king));

			for (int t = 0; t < possibleThreats.Count; t++)
			{
				int enemy_piece = possibleThreats[t];
				// Get the color of the enemy piece
				int c = GetColor(enemy_piece);
				bool enemyIsWhite = c == 0;
				if (enemyIsWhite != whiteKing)
				{
					// Get all the possible enemy moves to see if player's king in check...
					List<int> enemy_moves = GetBitMoves(state, enemy_piece, false);
					for (int m = 0; m < enemy_moves.Count; m++)						// NOTICE: we don't need to check if						
					{                                                               // enemy moves are legal, since you don't
						int enemy_move = enemy_moves[m];							// actually have to be able to MAKE the move
						MoveType moveType = (MoveType)BitMoveType(enemy_move);		// to put the enemy king in check!
						int endBoard = EndBoard(enemy_move);                                
						int endInd = EndIndex(enemy_move);
						
						if ((moveType == MoveType.Capture || moveType == MoveType.Swoop) &&
							(endBoard == b && endInd == i))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// Given that the king is in check, see if it is also checkmate
		public static bool IsCheckmate(Gamestate state, bool whiteKing)
		{
			// If we're not in check, don't both looking for checkmate!
			if (!IsCheck(state, whiteKing))
				return false;

			List<int> pieces = GetPieces(state, whiteKing);
			// Iterate through all the pieces - see if we can move ANY of
			// the player's pieces to escape check. If so, it is not checkmate.
			for (int i = 0; i < pieces.Count; i++)
			{
				int p = pieces[i];
				List<int> potential_moves = GetBitMoves(state, p, true);
				if (potential_moves.Count > 0)
					return false;
			}
			return true;
		}

		public static int GetKing(Gamestate state, bool whiteKing)
		{
			int kingColor = 0;
			if (!whiteKing) kingColor = 1;

			for (int b = 3; b > 0; b--)
			{
				for (int i = 0; i < 96; i++)
				{
					int pieceColor = 0;
					if (state.boards[b][i] < 0) pieceColor = 1;
					int t = Math.Abs(state.boards[b][i])-1;
					if ((PieceType)t == PieceType.King && kingColor == pieceColor)
					{
						return NewBitPiece(kingColor, t, b, i / 12, i % 12);
					}
				}
			}
			M.print("ERROR: could not locate the king: COLOR: " + whiteKing);
			return -1;
		}

		// Print out a string representation of the current board state
		public static void PrintState(Gamestate state, int fileNum)
		{
			string filename = "Assets/Out/Gamestate" + fileNum + ".txt";
			StreamWriter file = new StreamWriter(filename);

			string toPrint = "";

			for (int b = 3; b > 0; b--)
			{
				int[] board = state.boards[b];
				for (int r = 7; r >= 0; r--)
				{
					string row = "";
					for (int c = 0; c < 12; c++)
					{
						int ind = r * 12 + c;
						if (board[ind] < 0)
						{
							row += pieceCodes[Math.Abs(board[ind])].ToString().ToUpper();
						}
						else
							row += pieceCodes[board[ind]];
						row += String.Format(" ");
					}
					file.WriteLine(row);
					toPrint += row;
					toPrint += "\n";
				}
				file.WriteLine("");
				toPrint += ("");
			}
			file.Close();
			M.print(toPrint);
		}
	}
}
