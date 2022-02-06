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
	using static Gamestate;
	using static ThreatDict;
	public enum Color { White, Black };
	public enum Layer { Upper, Middle, Lower };
	public enum PlayerType { Human, AI };
	public enum Status { Normal, Check, Checkmate, Stalemate }

	public class Game
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
		public Game (bool fromFile, PlayerType type1, AIDifficulty A1, PlayerType type2, AIDifficulty A2)
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

		public static Game CopyGame(Game state)
		{
			PlayerType type1 = state.P1.type;
			AIDifficulty A1 = state.A1;
			PlayerType type2 = state.P2.type;
			AIDifficulty A2 = state.A2;

			Game copy = new Game(false, type1, A1, type2, A2);

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
			for (int i = 0; i < state.P1.pieces.Count; i++)
			{
				Piece p = state.P1.pieces[i];
				Piece piece = Piece.NewPiece(copy, (int)p.type, copy.P1, p.pos.board, p.pos.row, p.pos.col);
			}
			// Copy the pieces over for P2
			for (int i = 0; i < state.P2.pieces.Count; i++)
			{
				Piece p = state.P2.pieces[i];
				Piece piece = Piece.NewPiece(copy, (int)p.type, copy.P2, p.pos.board, p.pos.row, p.pos.col);
			}

			return copy;
		}

		public static int EvaluateGame(Game state)
		{
			Player player = state.ActivePlayer;
			Player enemy = state.EnemyPlayer;
			int ret = 0;

			for (int i = 0; i < state.P1.pieces.Count; i++)
			{
				Piece p = state.P1.pieces[i];
				ret += p.value;
			}
			for (int i = 0; i < state.P2.pieces.Count; i++)
			{
				Piece p = state.P2.pieces[i];
				ret -= p.value;
			}

			return ret;
		}

		public static void PrintState(Game state, int printNum=0)
		{
			string filename = "Assets/Out/Game";
			filename += printNum + ".txt";
			StreamWriter file = new StreamWriter(filename);

			List<Board> boards = new List<Board> { state.upperBoard, state.middleBoard, state.lowerBoard };
			for (int i = 0; i < 3; i++)
			{
				Board board = boards[i];
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
		
		public static Player GetEnemy(Game state, Player player)
		{
			if (player == state.P1)
				return state.P2;
			else
				return state.P1;
		}

		public static Piece GetKing(Game state, Player p)
		{
			return p.pieces.Find(x => (x.type == PieceType.King));
		}
	}
}
