using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Dragonchess
{
	using M = MonoBehaviour;
	using static MoveDict;
	using static ThreatDict;
	public enum Color { White, Black };
	public enum Layer { Upper, Middle, Lower };
	public enum PlayerType { Human, AI };
	public enum Status { Normal, Check, Checkmate, Stalemate }

	public class Gamestate
	{
		static public List<(Move, bool)> moveLog;

		public Player ActivePlayer;
		public Player EnemyPlayer;
		
		public Player P1;
		public AIDifficulty A1;
		public Player P2;
		public AIDifficulty A2;

		public Board upperBoard;
		public Board middleBoard;
		public Board lowerBoard;

		public Board[] boards;

		public bool gameOver;
		int moveNum;

		// ----------------------------------------------------
		public Gamestate (bool fromFile, PlayerType type1, AIDifficulty A1, PlayerType type2, AIDifficulty A2)
		{
			// If reading game data from a file (for debugging mostly),
			// make sure we have two "human" players (ie, manual players)
			if (fromFile)
			{
				P1 = new Human(Color.White, PlayerType.Human);
				P2 = new Human(Color.Black, PlayerType.Human);
			}
			// If accepting moves from Human/AI input
			if (type1 == PlayerType.Human)
					P1 = new Human(Color.White, PlayerType.Human);
				else
					P1 = new AI(Color.White, PlayerType.AI, A1);

				if (type2 == PlayerType.Human)
					P2 = new Human(Color.Black, PlayerType.Human);
				else
					P2 = new AI(Color.Black, PlayerType.AI, A2);

			boards = new Board[4];
			upperBoard = new Board(3, Layer.Upper);
			middleBoard = new Board(2, Layer.Middle);
			lowerBoard = new Board(1, Layer.Lower);

			boards[3] = upperBoard;
			boards[2] = middleBoard;
			boards[1] = lowerBoard;

			ActivePlayer = P1;
			EnemyPlayer = P2;
		}

		public static Gamestate CopyGamestate(Gamestate state)
		{
			PlayerType type1 = state.P1.type;
			AIDifficulty A1 = state.A1;
			PlayerType type2 = state.P2.type;
			AIDifficulty A2 = state.A2;

			Gamestate copy = new Gamestate(false, type1, A1, type2, A2);

			// Set new player1's type
			if (type1 == PlayerType.Human)
				copy.P1 = new Human(Color.White, PlayerType.Human);
			else
				copy.P1 = new AI(Color.White, PlayerType.AI, A1);
			// Set new player2's type
			if (type2 == PlayerType.Human)
				copy.P2 = new Human(Color.Black, PlayerType.Human);
			else
				copy.P2 = new AI(Color.Black, PlayerType.AI, A2);

			// Create new empty boards
			copy.boards = new Board[4];

			copy.upperBoard = new Board(3, Layer.Upper);
			copy.middleBoard = new Board(2, Layer.Middle);
			copy.lowerBoard = new Board(1, Layer.Lower);

			copy.boards[3] = copy.upperBoard;
			copy.boards[2] = copy.middleBoard;
			copy.boards[1] = copy.lowerBoard;

			// Set correct player's turn
			if (state.ActivePlayer.color == Color.White)
			{
				copy.ActivePlayer = copy.P1;
				copy.EnemyPlayer = copy.P2;
			}
			else
			{
				copy.ActivePlayer = copy.P2;
				copy.EnemyPlayer = copy.P1;
			}

			// Copy the pieces over for P1
			foreach (Piece p in state.P1.pieces)
			{
				Piece piece = Piece.NewPiece(copy, (int)p.type, copy.P1, p.pos.board, p.pos.row, p.pos.col);
			}
			// Copy the pieces over for P2
			foreach (Piece p in state.P2.pieces)
			{
				Piece piece = Piece.NewPiece(copy, (int)p.type, copy.P2, p.pos.board, p.pos.row, p.pos.col);
			}

			foreach (Piece p in state.P1.pieces)
			{
				foreach (Piece p2 in copy.P1.pieces)
				{
					if (p == p2)
						MonoBehaviour.print("ERROR: PIECES ARE LINKED");
				}
			}

			return copy;
		}

		public static int EvaluateGamestate(Gamestate state)
		{
			Player player = state.ActivePlayer;
			Player enemy = state.EnemyPlayer;
			int ret = 0;

			foreach (Piece p in state.P1.pieces)
				ret += p.value;

			foreach (Piece p in state.P2.pieces)
				ret -= p.value;

			return ret;
		}

		public static void PrintState(Gamestate state, int printNum=0)
		{
			string filename = "Assets/Out/gamestate";
			filename += printNum + ".txt";
			StreamWriter file = new StreamWriter(filename);

			List<Board> boards = new List<Board> { state.upperBoard, state.middleBoard, state.lowerBoard };
			foreach (Board board in boards)
			{
				for (int r = 7; r >= 0; r--)
				{
					string row = "";
					for (int c = 0; c < 12; c++)
					{
						if (board.squares[r,c].occupied)
						{
							Piece piece = board.squares[r, c].piece;
							if (piece.color == Color.White)
							{
								row += piece.nameChar.ToUpper();
							}
							else
							{
								row += piece.nameChar.ToLower();
							}
						}
						else
						{
							row += ".";
						}
						row += String.Format(" ");
					}
					row += String.Format("\n");
					file.WriteLine(row);
					//MonoBehaviour.print(row);
				}
				file.WriteLine("\n\n");

			}
			file.Close();
		}
		
		public static Status GetGameStatus(Gamestate state, Player player)
		{
			List<Piece> threats = ThreatsInRange(state, player);
			if (IsCheck(state, player, threats))
				return Status.Check;
			else if (IsCheckmate(state, player))
				return Status.Checkmate;
			else
				return Status.Normal;
		}

		public static List<Piece> ThreatsInRange(Gamestate state, Player p)
		{
			Piece king = GetKing(state, p);
			(int b, int r, int c) = (king.pos.board, king.pos.row, king.pos.col);

			/* Evaluate only the types of pieces capable of capturing the king
			 * on this board level. To do this, pretend the king is of that
			 * type, searching for such pieces "in range" of the king.*/

			Dictionary<(int, int, int), List<(int, int, int, int)>> threatsToKing;
			List<Piece> inRange = new List<Piece>();

			if (p.color == Color.White)
				threatsToKing = ThreatsToWhiteKing;
			else
				threatsToKing = ThreatsToBlackKing;

			foreach (var x in threatsToKing[(b, r, c)])
			{
				int pieceType = x.Item1;
				int board = x.Item2;
				int row = x.Item3;
				int col = x.Item4;
				Board threatBoard = GetBoard(state, board);

				if (threatBoard.squares[row, col].occupied)
				{
					Piece threat = threatBoard.squares[row, col].piece;
					if (threat.type == (PieceType)pieceType && threat.color != p.color)
					{
						inRange.Add(threat);
					}
				}
			}

			return inRange;
		}

		// Evaluates and returns whether or not Player p's king is in check
		public static bool IsCheck(Gamestate state, Player p, List<Piece> possibleThreats)
		{
			Dictionary<string, List<(int, int, int)>[,,]> dict = MoveDictionary;
			Piece king = GetKing(state, p);
			(int b, int r, int c) = ( king.pos.board, king.pos.row, king.pos.col );

			foreach (Piece enemy_piece in possibleThreats)
			{
				if (enemy_piece.color != king.color && enemy_piece.type != PieceType.King)
				{
					foreach (Move enemy_move in enemy_piece.GetMoves(state))
					{
						if ((enemy_move.type == MoveType.Capture || enemy_move.type == MoveType.Swoop) &&
							(enemy_move.end == king.pos)) {
							return true;
						}
					}
				}
			}
			return false;
		}

		// Given that the king is in check, see if it is also checkmate
		public static bool IsCheckmate(Gamestate state, Player player)
		{
			List<(Piece p, Move m)> safe_moves = new List<(Piece p, Move m)>();

			// Iterate through all the pieces - see if we can move ANY of
			// the player's pieces to escape check. If so, it is not checkmate.
			foreach (Piece p in player.pieces)
			{
				List<Move> potential_moves = p.GetMoves(state);
				MoveController.RemoveIllegal(state, player, ref potential_moves);
				if (potential_moves.Count > 0)
					return false;
			}
			return true;
		}

		public static Board GetBoard(Gamestate state, int b)
		{
			if (b == 3)
				return state.upperBoard;
			else if (b == 2)
				return state.middleBoard;
			else
				return state.lowerBoard;
		}

		public static Player GetEnemy(Gamestate state, Player player)
		{
			if (player == state.P1)
				return state.P2;
			else
				return state.P1;
		}

		public static Piece GetKing(Gamestate state, Player p)
		{
			return p.pieces.Find(x => (x.type == PieceType.King));
		}
	}
}
