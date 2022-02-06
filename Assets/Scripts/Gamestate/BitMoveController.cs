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
	using static BitMove;

	public class BitMoveController
	{
		// Get the moves for a particular piece: legal or pseudo-legal based on the flag
		public static List<int> GetBitMoves(Gamestate state, int piece, bool removeIllegal = true)
		{
			List<int> possibleMoves;
			// Extract the piece data from the int piece
			int t = PieceType(piece);
			int color = GetColor(piece);
			int board = GetBoard(piece);
			int row = GetRow(piece);
			int col = GetCol(piece);
			// Calculate the 1D array index based on the row/col
			int ind = (int)row * 12 + (int)col;

			if (board == 2)
			{
				int below = state.boards[1][ind];
				if ((PieceType)(Math.Abs(below)-1) == PieceType.Basilisk)
				{
					if (below < 0 && color == 0 || below > 0 && color == 1)
						return new List<int>();
				}
			}

			// Get moves based on the piece type
			if (t == 0)
				possibleMoves = SylphBitMoves(state, color, board, ind);
			else if (t == 1)
				possibleMoves = GriffonBitMoves(state, color, board, ind);
			else if (t == 2)
				possibleMoves = DragonBitMoves(state, color, board, ind);
			else if (t == 3)
				possibleMoves = WarriorBitMoves(state, color, board, ind);
			else if (t == 4)
				possibleMoves = OliphantBitMoves(state, color, board, ind);
			else if (t == 5)
				possibleMoves = UnicornBitMoves(state, color, board, ind);
			else if (t == 6)
				possibleMoves = HeroBitMoves(state, color, board, ind);
			else if (t == 7)
				possibleMoves = ThiefBitMoves(state, color, board, ind);
			else if (t == 8)
				possibleMoves = ClericBitMoves(state, color, board, ind);
			else if (t == 9)
				possibleMoves = MageBitMoves(state, color, board, ind);
			else if (t == 10)
				possibleMoves = KingBitMoves(state, color, board, ind);
			else if (t == 11)
				possibleMoves = PaladinBitMoves(state, color, board, ind);
			else if (t == 12)
				possibleMoves = DwarfBitMoves(state, color, board, ind);
			else if (t == 13)
				possibleMoves = BasiliskBitMoves(state, color, board, ind);
			else
				possibleMoves = ElementalBitMoves(state, color, board, ind);

			if (removeIllegal)
				RemoveIllegal(state, (color==0), ref possibleMoves);

			return possibleMoves;
		}

		// Get all the moves for one player (white if isWhite, black otherwise) in the given state
		public static List<int> GetPossibleMoves(Gamestate state, bool isWhite)
		{
			List<int> possibleMoves = new List<int>();
			List<int> pieceMoves = new List<int>();
			List<int> pieces = GetPieces(state, isWhite);
			for (int i = 0; i < pieces.Count; i++)
			{
				int piece = pieces[i];
				pieceMoves = GetBitMoves(state, piece, true);
				if (pieceMoves.Count != 0)
					possibleMoves.AddRange(pieceMoves);
			}
			return possibleMoves;
		}

		// Find any illegal moves in a list and remove them - player cannot put themselves in check
		public static void RemoveIllegal(Gamestate state, bool isWhite, ref List<int> moves)
		{
			List<int> illegal = new List<int>();

			for (int i = 0; i < moves.Count; i++)
			{
				int move = moves[i];
				// Store the current state so we can undo the move
				int start = GetStartPiece(state, move);
				int end = GetEndPiece(state, move);

				// If the move put the king in check, it is an illegal move
				DoBitMove(ref state, move);
				if (IsCheck(state, isWhite))
					illegal.Add(move);
				UndoBitMove(ref state, move, start, end);
			}

			for (int i = 0; i < illegal.Count; i++)
			{
				int illegal_move = illegal[i];
				moves.Remove(illegal_move);
			}
		}

		// Unpack the move stored in int move, changing the passed-in state
		public static void DoBitMove(ref Gamestate state, int move)
		{
			int type = BitMoveType(move);
			int sBoard = StartBoard(move);
			int sInd = StartIndex(move);
			int eBoard = EndBoard(move);
			int eInd = EndIndex(move);

			PieceType piece = (PieceType)(Math.Abs(state.boards[sBoard][sInd]) - 1);

			// Change the value on the board representation
			if ((MoveType)type != MoveType.Swoop)
			{
				state.boards[eBoard][eInd] = state.boards[sBoard][sInd];
				state.boards[sBoard][sInd] = 0;
			}
			else
				state.boards[eBoard][eInd] = 0;

			// Switch turns 
			state.WhiteToMove = !state.WhiteToMove;
		}

		// Undo a move, setting the two indices back to the values they were at before
		public static void UndoBitMove(ref Gamestate state, int move, int start, int end)
		{
			int sB = StartBoard(move);
			int sI = StartIndex(move);
			int eB = EndBoard(move);
			int eI = EndIndex(move);
			state.boards[sB][sI] = start;
			state.boards[eB][eI] = end;

			PieceType startType = (PieceType)PieceType(start);
			int startColor = GetColor(start);
			PieceType endType = (PieceType)PieceType(end);
			int endColor = GetColor(end);

			// Switch turns 
			state.WhiteToMove = !state.WhiteToMove;
		}

		//===================================================================================================

		// Checks if a move is valid and if so, adds it to the list of moves
		public static void VerifyAndAddMoves(Gamestate state, ref List<int> moves, string key, int board, int ind, int color, MoveType type)
		{
			if (type == MoveType.MoveOrCapture)
			{
				VerifyAndAddMoves(state, ref moves, key, board, ind, color, MoveType.Regular);
				VerifyAndAddMoves(state, ref moves, key, board, ind, color, MoveType.Capture);
				return;
			}

			List<(int, int, int)> possibleMoves = MoveDictionary[key][board, ind / 12, ind % 12];

			for (int i = 0; i < possibleMoves.Count; i++)
			{
				(int b, int r, int c) = possibleMoves[i];
				int move = CreateBitMove((int)type, board, ind, b, (r * 12 + c), 0);
				if (IsValidBitMove(state, move, color))
					moves.Add(move);
			}
		}

		// Checks if a move is valid, considering sliding piece rules, and if so, adds it to the list of moves
		public static void CheckBlockAndAdd(Gamestate state, ref List<int> moves, string key, int board, int ind, int color, MoveType type, List<string> slideTypes)
		{
			if (type == MoveType.MoveOrCapture)
			{
				CheckBlockAndAdd(state, ref moves, key, board, ind, color, MoveType.Regular, slideTypes);
				CheckBlockAndAdd(state, ref moves, key, board, ind, color, MoveType.Capture, slideTypes);
				return;
			}

			List<(int, int, int)> possibleMoves = MoveDictionary[key][board, ind / 12, ind % 12];
			List<(int, int, int)> blocked = GetBlocked(state, possibleMoves, slideTypes, board, ind / 12, ind % 12);

			for (int i = 0; i < possibleMoves.Count; i++)
			{
				(int b, int r, int c) = possibleMoves[i];
				if (!blocked.Contains((b, r, c)))
				{
					int move = CreateBitMove((int)type, board, ind, b, (r * 12 + c), 0);
					if (IsValidBitMove(state, move, color))
						moves.Add(move);
				}
			}
		}

		// Gets a list of all the enemy threats in range of the king passed in
		public static List<int> ThreatsInRange(Gamestate state, bool isWhite)
		{
			/* Evaluate only the types of pieces capable of capturing the king
			 * on this board level. To do this, pretend the king is of that
			 * type, searching for such pieces "in range" of the king.*/

			Dictionary<(int, int, int), List<(int, int, int, int)>> threatsToKing;
			int king = GetKing(state, isWhite);
			(int b, int r, int c) = (GetBoard(king), GetRow(king), GetCol(king));
			List<int> inRange = new List<int>();

			if (isWhite)
				threatsToKing = ThreatsToWhiteKing;
			else
				threatsToKing = ThreatsToBlackKing;

			List<(int, int, int, int)> threats = threatsToKing[(b, r, c)];

			for (int i = 0; i < threats.Count; i++)
			{
				var x = threats[i];
				PieceType pieceType = (PieceType)x.Item1;
				int board = x.Item2;
				int ind = x.Item3 * 12 + x.Item4;

				if (state.boards[board][ind] != 0)
				{
					int threatValue = state.boards[board][ind];
					int threatType = Math.Abs(threatValue) - 1;

					int threatColor = 0;
					if (threatValue < 0)
						threatColor = 1;

					int tPiece = NewBitPiece(threatColor, threatType, board, ind / 12, ind % 12);

					if ((PieceType)threatType == pieceType)
					{
						int kingColor = 0;
						if (!isWhite) kingColor = 1;

						if (threatColor != kingColor)
						{
							inRange.Add(tPiece);
						}
					}
				}
			}
			return inRange;
		}

		//==========================================================================================
		// Bitmove: 32-bit integer storing information about a move
		//==========================================================================================
		//	   byte7	| byte6 & byte5	|	byte4	 |	 byte3 & byte2 |	byte1	  |		byte0
		// -------------|---------------|------------|-----------------|--------------|-------------
		//	   0000		|	0000 0000	|	0000	 |	 0000 0000	   |	0000	  |		0000
		// -------------|---------------|------------|-----------------|--------------|-------------
		//	 moveFlags	|	endIndex	|  endBoard	 |  startIndex	   |  startBoard  |	   moveType
		//==========================================================================================

		public static List<int> SylphBitMoves(Gamestate state, int color, int board, int ind)
		{
			List<int> moves = new List<int>();

			if (color == 0 && board == 3)
			{
				VerifyAndAddMoves(state, ref moves, "TopWSylph", board, ind, color, MoveType.Regular);
				VerifyAndAddMoves(state, ref moves, "TopWSylphTake", board, ind, color, MoveType.Capture);
			}
			else if (color == 1 && board == 3)
			{
				VerifyAndAddMoves(state, ref moves, "TopBSylph", board, ind, color, MoveType.Regular);
				VerifyAndAddMoves(state, ref moves, "TopBSylphTake", board, ind, color, MoveType.Capture);
			}
			else if (color == 0 && board == 2)
			{
				VerifyAndAddMoves(state, ref moves, "MidWSylph", board, ind, color, MoveType.Regular);
			}
			else if (color == 1 && board == 2)
			{
				VerifyAndAddMoves(state, ref moves, "MidBSylph", board, ind, color, MoveType.Regular);
			}
			return moves;
		}

		public static List<int> GriffonBitMoves(Gamestate state, int color, int board, int ind)
		{
			List<int> moves = new List<int>();
			if (board == 3)
				VerifyAndAddMoves(state, ref moves, "TopGrif", board, ind, color, MoveType.MoveOrCapture);
			if (board == 2)
				VerifyAndAddMoves(state, ref moves, "MidGrif", board, ind, color, MoveType.MoveOrCapture);
			return moves;
		}

		public static List<int> DragonBitMoves(Gamestate state, int color, int board, int ind)
		{
			List<int> moves = new List<int>();
			List<string> slideTypes = new List<string> { "diagUR", "diagUL", "diagDR", "diagDL" };

			CheckBlockAndAdd(state, ref moves, "Dragon", board, ind, color, MoveType.MoveOrCapture, slideTypes);
			VerifyAndAddMoves(state, ref moves, "SwoopDragon", board, ind, color, MoveType.Swoop);
			return moves;
		}

		public static List<int> WarriorBitMoves(Gamestate state, int color, int board, int ind)
		{
			List<int> moves = new List<int>();
			if (color == 0)
			{
				VerifyAndAddMoves(state, ref moves, "WWarrior", board, ind, color, MoveType.Regular);
				VerifyAndAddMoves(state, ref moves, "WWarriorTake", board, ind, color, MoveType.Capture);
			}
			else
			{
				VerifyAndAddMoves(state, ref moves, "BWarrior", board, ind, color, MoveType.Regular);
				VerifyAndAddMoves(state, ref moves, "BWarriorTake", board, ind, color, MoveType.Capture);
			}
			return moves;
		}

		public static List<int> OliphantBitMoves(Gamestate state, int color, int board, int ind)
		{
			List<int> moves = new List<int>();
			List<string> slideTypes = new List<string> { "forward", "backward", "left", "right" };
			CheckBlockAndAdd(state, ref moves, "Oliphant", board, ind, color, MoveType.MoveOrCapture, slideTypes);

			return moves;
		}

		public static List<int> UnicornBitMoves(Gamestate state, int color, int board, int ind)
		{
			List<int> moves = new List<int>();
			VerifyAndAddMoves(state, ref moves, "Unicorn", board, ind, color, MoveType.MoveOrCapture);
			return moves;
		}

		public static List<int> HeroBitMoves(Gamestate state, int color, int board, int ind)
		{
			List<int> moves = new List<int>();
			if (board == 3)
				VerifyAndAddMoves(state, ref moves, "TopHero", board, ind, color, MoveType.MoveOrCapture);
			else if (board == 2)
				VerifyAndAddMoves(state, ref moves, "MidHero", board, ind, color, MoveType.MoveOrCapture);
			else
				VerifyAndAddMoves(state, ref moves, "BotHero", board, ind, color, MoveType.MoveOrCapture);
			return moves;
		}

		public static List<int> ThiefBitMoves(Gamestate state, int color, int board, int ind)
		{
			List<int> moves = new List<int>();
			List<string> slideTypes = new List<string> { "diagUR", "diagUL", "diagDR", "diagDL" };
			CheckBlockAndAdd(state, ref moves, "Thief", board, ind, color, MoveType.MoveOrCapture, slideTypes);
			return moves;
		}

		public static List<int> ClericBitMoves(Gamestate state, int color, int board, int ind)
		{
			List<int> moves = new List<int>();
			if (board == 3)
				VerifyAndAddMoves(state, ref moves, "TopCleric", board, ind, color, MoveType.MoveOrCapture);
			else if (board == 2)
				VerifyAndAddMoves(state, ref moves, "MidCleric", board, ind, color, MoveType.MoveOrCapture);
			else
				VerifyAndAddMoves(state, ref moves, "BotCleric", board, ind, color, MoveType.MoveOrCapture);
			return moves;
		}

		public static List<int> MageBitMoves(Gamestate state, int color, int board, int ind)
		{
			List<int> moves = new List<int>();
			List<string> slideTypes = new List<string> {
				"diagUR", "diagUL", "diagDR", "diagDL",
				"forward", "backward", "left", "right"};

			if (board == 3)
				CheckBlockAndAdd(state, ref moves, "TopMage", board, ind, color, MoveType.MoveOrCapture, slideTypes);
			else if (board == 2)
				CheckBlockAndAdd(state, ref moves, "MidMage", board, ind, color, MoveType.MoveOrCapture, slideTypes);
			else
				CheckBlockAndAdd(state, ref moves, "BotMage", board, ind, color, MoveType.MoveOrCapture, slideTypes);

			return moves;
		}

		public static List<int> KingBitMoves(Gamestate state, int color, int board, int ind)
		{
			List<int> moves = new List<int>();
			if (board == 3)
				VerifyAndAddMoves(state, ref moves, "TopKing", board, ind, color, MoveType.MoveOrCapture);
			else if (board == 2)
				VerifyAndAddMoves(state, ref moves, "MidKing", board, ind, color, MoveType.MoveOrCapture);
			else
				VerifyAndAddMoves(state, ref moves, "BotKing", board, ind, color, MoveType.MoveOrCapture);
			return moves;
		}

		public static List<int> PaladinBitMoves(Gamestate state, int color, int board, int ind)
		{
			List<int> moves = new List<int>();
			if (board == 3)
				VerifyAndAddMoves(state, ref moves, "TopPaladin", board, ind, color, MoveType.MoveOrCapture);
			else if (board == 2)
				VerifyAndAddMoves(state, ref moves, "MidPaladin", board, ind, color, MoveType.MoveOrCapture);
			else
				VerifyAndAddMoves(state, ref moves, "BotPaladin", board, ind, color, MoveType.MoveOrCapture);
			return moves;
		}

		public static List<int> DwarfBitMoves(Gamestate state, int color, int board, int ind)
		{
			List<int> moves = new List<int>();
			if (color == 0 && board == 1)
			{
				VerifyAndAddMoves(state, ref moves, "BotWDwarf", board, ind, color, MoveType.Regular);
				VerifyAndAddMoves(state, ref moves, "BotWDwarfTake", board, ind, color, MoveType.Capture);
			}
			else if (color == 1 && board == 1)
			{
				VerifyAndAddMoves(state, ref moves, "BotBDwarf", board, ind, color, MoveType.Regular);
				VerifyAndAddMoves(state, ref moves, "BotBDwarfTake", board, ind, color, MoveType.Capture);
			}
			else if (color == 0 && board == 2)
			{
				VerifyAndAddMoves(state, ref moves, "MidWDwarf", board, ind, color, MoveType.Regular);
				VerifyAndAddMoves(state, ref moves, "MidWDwarfTake", board, ind, color, MoveType.Capture);
			}
			else if (color == 1 && board == 2)
			{
				VerifyAndAddMoves(state, ref moves, "MidBDwarf", board, ind, color, MoveType.Regular);
				VerifyAndAddMoves(state, ref moves, "MidBDwarfTake", board, ind, color, MoveType.Capture);
			}
			return moves;
		}

		public static List<int> BasiliskBitMoves(Gamestate state, int color, int board, int ind)
		{
			List<int> moves = new List<int>();
			if (color == 0)
			{
				VerifyAndAddMoves(state, ref moves, "WBasilisk", board, ind, color, MoveType.MoveOrCapture);
				VerifyAndAddMoves(state, ref moves, "WBackwardsBasilisk", board, ind, color, MoveType.Regular);
			}
			else
			{
				VerifyAndAddMoves(state, ref moves, "BBasilisk", board, ind, color, MoveType.MoveOrCapture);
				VerifyAndAddMoves(state, ref moves, "BBackwardsBasilisk", board, ind, color, MoveType.Regular);
			}
			return moves;
		}

		public static List<int> ElementalBitMoves(Gamestate state, int color, int board, int ind)
		{
			List<int> moves = new List<int>();

			if (board == 1)
			{
				List<string> slideTypes = new List<string> { "forward", "backward", "left", "right" };

				CheckBlockAndAdd(state, ref moves, "BotElemental", board, ind, color, MoveType.MoveOrCapture, slideTypes);
				VerifyAndAddMoves(state, ref moves, "BotElementalSlide", board, ind, color, MoveType.Regular);
			}
			else if (board == 2)
			{
				VerifyAndAddMoves(state, ref moves, "MidElemental", board, ind, color, MoveType.MoveOrCapture);
			}

			return moves;
		}

	}
}
