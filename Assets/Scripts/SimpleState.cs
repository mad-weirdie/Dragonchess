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
	using static PiecePosEval;

	public class SimpleState
	{
		public static char[] pieceCodes = {'.', 's', 'g', 'r', 'w', 'o', 'u', 'h', 't', 'c', 'm', 'k', 'p', 'd', 'b', 'e'};
		public static int[] pieceValues = { 0, 1, 20, 90, 1, 15, 3, 10, 5, 20, 40, 900, 50, 2, 3, 4};

		public int[][] boards;
		public int[] b3;
		public int[] b2;
		public int[] b1;

		public bool WhiteToMove;
		public int[] k1;
		public int[] k2;

		public SimpleState()
		{
			boards = new int[4][];
			b3 = new int[96];
			b2 = new int[96];
			b1 = new int[96];

			boards[3] = b3;
			boards[2] = b2;
			boards[1] = b1;

			k1 = new int[3];
			k2 = new int[3];

			WhiteToMove = true;
		}

		public SimpleState(Gamestate state)
		{
			boards = new int[4][];

			b3 = new int[96];
			b2 = new int[96];
			b1 = new int[96];

			boards[3] = b3;
			boards[2] = b2;
			boards[1] = b1;

			k1 = new int[3];
			k2 = new int[3];

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

			foreach (Piece piece in state.P1.pieces)
			{
				int bPiece = PieceToInt(piece);
				int ind = piece.pos.row * 12 + piece.pos.col;

				if (piece.pos.board == state.upperBoard.layer_int_val)
					b3[ind] = 1 * ((int)piece.type + 1);
				else if (piece.pos.board == state.middleBoard.layer_int_val)
					b2[ind] = 1 * ((int)piece.type + 1);
				else
					b1[ind] = 1 * ((int)piece.type + 1);

				if (piece.type == PieceType.King)
				{
					k1[0] = piece.pos.board;
					k1[1] = piece.pos.row;
					k1[2] = piece.pos.col;
				}
			}

			foreach (Piece piece in state.P2.pieces)
			{
				int bPiece = PieceToInt(piece);
				int ind = piece.pos.row * 12 + piece.pos.col;
				
				if (piece.pos.board == state.upperBoard.layer_int_val)
					b3[ind] = (-1) * ((int)piece.type + 1);
				else if (piece.pos.board == state.middleBoard.layer_int_val)
					b2[ind] = (-1) * ((int)piece.type + 1);
				else
					b1[ind] = (-1) * ((int)piece.type + 1);

				if (piece.type == PieceType.King)
				{
					k2[0] = piece.pos.board;
					k2[1] = piece.pos.row;
					k2[2] = piece.pos.col;
				}
			}
		}

		public static SimpleState CopySimpleState(SimpleState state)
		{
			SimpleState copy = new SimpleState();
		
			for (int i = 0; i < 96; i++)
			{
				copy.b3[i] = state.b3[i];
				copy.b2[i] = state.b2[i];
				copy.b1[i] = state.b1[i];
			}

			copy.boards[3] = copy.b3;
			copy.boards[2] = copy.b2;
			copy.boards[1] = copy.b1;
			copy.k1[0] = state.k1[0];
			copy.k1[1] = state.k1[1];
			copy.k1[2] = state.k1[2];
			copy.k2[0] = state.k2[0];
			copy.k2[1] = state.k2[1];
			copy.k2[2] = state.k2[2];

			copy.WhiteToMove = state.WhiteToMove;

			return copy;
		}

		public static int EvaluateSimpleState(SimpleState state)
		{
			int ret = 0;
			List<int> p1 = GetPieces(state, true);
			List<int> p2 = GetPieces(state, false);

			// Evaluation based on current player 1 pieces and locations
			foreach (int p in p1)
			{
				int[] x = UnpackBitPiece(p);
				int t = x[0];
				int b = x[2];
				int r = x[3];
				int c = x[4];

				ret += GetPieceValue(p);
				ret += PositionEval[t][3 - b, r * 12 + c];
			}

			// Evaluation based on current player 2 pieces and locations
			foreach (int p in p2)
			{
				int[] x = UnpackBitPiece(p);
				int t = x[0];
				int b = x[2];
				int r = x[3];
				int c = x[4];

				ret -= GetPieceValue(p);
				// The evaluation table is flipped for black pieces
				int flipRow = 7 - r;
				ret -= PositionEval[t][3-b, flipRow*12+c];
			}

			// Check state:
			if (IsSSCheck(state, 0))
			{
				ret -= 500;
				if (IsSSCheckmate(state, 0))
					ret -= 5000;
			}
			if (IsSSCheck(state, 1))
			{
				ret += 500;
				if (IsSSCheckmate(state, 1))
					ret += 5000;
			}

			return ret;
		}

		public static List<int> SSThreatsInRange(SimpleState state, bool isWhite, int[] king)
		{
			(int b, int r, int c) = (king[0], king[1], king[2]);
			/* Evaluate only the types of pieces capable of capturing the king
			 * on this board level. To do this, pretend the king is of that
			 * type, searching for such pieces "in range" of the king.*/

			Dictionary<(int, int, int), List<(int, int, int, int)>> threatsToKing;
			List<int> inRange = new List<int>();

			if (isWhite)
				threatsToKing = ThreatsToWhiteKing;
			else
				threatsToKing = ThreatsToBlackKing;

			foreach (var x in threatsToKing[(b, r, c)])
			{
				PieceType pieceType = (PieceType)x.Item1;
				int board = x.Item2;
				int ind = x.Item3* 12 + x.Item4;

				if (state.boards[board][ind] != 0)
				{
					int threatValue = state.boards[board][ind];
					int threatType = Math.Abs(threatValue) - 1;

					// We store the color of the piece in the least significant bit of the board
					// byte (It is the only one with a small enough range value)
					int threatColor = 0;
					if (threatValue < 0)
						threatColor = 1;

					int tPiece = GetBitPiece(threatColor, threatType, board, ind / 12, ind % 12);
					
					if ((PieceType)threatType == pieceType)
					{
						int kingColor = 0;
						if (!isWhite)
							kingColor = 1;

						if (threatColor != kingColor)
						{
							if ((PieceType)threatType == PieceType.Dragon ||
							(PieceType)threatType == PieceType.Mage ||
							(PieceType)threatType == PieceType.Thief ||
							(PieceType)threatType == PieceType.Oliphant ||
							(PieceType)threatType == PieceType.Elemental)
							{
								List<int> moves = GetBitMoves(state, tPiece, false);
								if (moves.Count > 0)
								{
									inRange.Add(tPiece);
								}
							}
							else
							{
								List<int> moves = GetBitMoves(state, tPiece, false);
								if (moves.Count > 0)
								{
									//M.print("found (regular) threat in range.");
									inRange.Add(tPiece);
								}
							}
						}
					}
				}
			}
			return inRange;
		}

		// Evaluates and returns whether or not Player p's king is in check
		public static bool IsSSCheck(SimpleState state, int kingColor)
		{
			List<int> possibleThreats;
			int[] king;

			if (kingColor == 0)
			{
				king = state.k1;
				possibleThreats = SSThreatsInRange(state, true, state.k1);
			}
			else
			{
				king = state.k2;
				possibleThreats = SSThreatsInRange(state, false, state.k2);
			}

			Dictionary<string, List<(int, int, int)>[,,]> dict = MoveDictionary;

			(int b, int r, int c) = (king[0], king[1], king[2]);

			foreach (int enemy_piece in possibleThreats)
			{
				int[] vals = UnpackBitPiece(enemy_piece);

				if (vals[1] != kingColor)
				{
					foreach (int enemy_move in GetBitMoves(state, enemy_piece, false))
					{
						int[] m = UnpackBitMove(enemy_move);
						if (((MoveType)m[0] == MoveType.Capture || (MoveType)m[0] == MoveType.Swoop) &&
							(m[3] == king[0] && m[4] == king[1]*12+king[2]))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// Given that the king is in check, see if it is also checkmate
		public static bool IsSSCheckmate(SimpleState state, int kColor)
		{
			List<(Piece p, Move m)> safe_moves = new List<(Piece p, Move m)>();
			List<int> pieces = GetPieces(state, kColor == 0);
			// Iterate through all the pieces - see if we can move ANY of
			// the player's pieces to escape check. If so, it is not checkmate.
			foreach (int p in pieces)
			{
				List<int> potential_moves = GetBitMoves(state, p, false);
				if (potential_moves.Count > 0)
					return false;
			}
			return true;
		}

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

		public static int GetBitPiece(int color, int type, int board, int row, int col)
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

		public static List<int> GetPieces(SimpleState state, bool isWhite)
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
						pieces.Add(GetBitPiece(0, t, b, r, c));
					else if (state.boards[b][i] < 0 && !isWhite)
						pieces.Add(GetBitPiece(1, t, b, r, c));
				}
			}
			return pieces;
		}

		public static int[] UnpackBitPiece(int piece)
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

		public static void PrintBitPiece(int piece)
		{
			int[] x = UnpackBitPiece(piece);
			M.print((Color)x[1] + " " + (PieceType)x[0] + " " + x[2] + "(" + x[3] + ", " + x[4] + ")");
		}

		public static int GetPieceValue(int piece)
		{
			int[] x = UnpackBitPiece(piece);
			int pieceType = x[0];
			return pieceValues[pieceType+1];
		}

		// Bitmove: 32-bit integer storing information about a move

		//	   byte7	| byte6 & byte5	|	byte4	 |	 byte3 & byte2 |	byte1	  |		byte0
		// -------------|---------------|------------|-----------------|--------------|-------------
		//	   0000		|	0000 0000	|	0000	 |	 0000 0000	   |	0000	  |		0000
		// -------------|---------------|------------|-----------------|--------------|-------------
		//	 moveFlags	|	endIndex	|  endBoard	 |  startIndex	   |  startBoard  |	   moveType
		//###############################################################################################


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

		public static Move BitMoveToMove(Gamestate state, int m)
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

		public static List<int> GetBitMoves(SimpleState state, int piece, bool removeIllegal=true)
		{
			List<int> possibleMoves;

			byte[] byteArray = BitConverter.GetBytes(piece);
			byte t = byteArray[0];
			byte board = byteArray[1];
			byte row = byteArray[2];
			byte col = byteArray[3];
			int ind = (int)row * 12 + (int)col;
			int color = (board & 0x8) >> 3;
			board = (byte)(board & 0x7);

			if (board == 2)
			{
				int below = state.boards[1][ind];

				if ((color == 0 && below < 0) || (color == 1 && below > 0))
				{
					if ((PieceType)(Math.Abs(below) - 1) == PieceType.Basilisk)
					{
						return new List<int>();
					}
				}
			}

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

			bool isWhite = (color == 0);
			
			if (removeIllegal)
				RemoveIllegal(state, isWhite, ref possibleMoves);

			return possibleMoves;
		}

		public static void RemoveIllegal(SimpleState state, bool isWhite, ref List<int> moves)
		{
			List<int> illegal = new List<int>();
			List<int> threats;

			foreach (int m in moves)
			{
				SimpleState copy = CopySimpleState(state);
				DoBitMove(copy, m);
				int kColor;
				if (isWhite) kColor = 0;
				else kColor = 1;

				if (IsSSCheck(copy, kColor))
					illegal.Add(m);
			}

			foreach (int illegal_move in illegal)
				moves.Remove(illegal_move);
		}

		public static List<int> GetPossibleMoves(SimpleState state, bool isWhite)
		{
			List<int> possibleMoves = new List<int>();
			List<int> pieceMoves = new List<int>();
			List<int> pieces = GetPieces(state, isWhite);

			foreach (int piece in pieces)
			{
				pieceMoves = GetBitMoves(state, piece);
				possibleMoves.AddRange(pieceMoves);
			}
			return possibleMoves;
		}

		//###############################################################################################

		public static int[] UnpackBitMove(int move)
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

		public static bool IsValidBitMove(SimpleState state, int move, int color)
		{
			int[] vals = UnpackBitMove(move);
			MoveType type = (MoveType)vals[0];

			// First, check that the move goal isn't out of bounds
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

		public static void PrintBitMove(SimpleState state, int move)
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

			PieceType ptype = (PieceType)(Math.Abs(state.boards[sBoard][sInd])-1);
			int r1 = sInd / 12;
			int c1 = sInd % 12;
			int r2 = eInd / 12;
			int c2 = eInd % 12;

			MoveType mtype = (MoveType)type;
			M.print(color + " " + ptype + ": " + sBoard + Letters[c1] +
				(1 + r1) + "->" + eBoard + Letters[c2] + (1 + r2) + " : " + mtype);
		}

		public static void DoBitMove(SimpleState state, int move)
		{
			int[] vals = UnpackBitMove(move);
			int type = vals[0];
			int sBoard = vals[1];
			int sInd = vals[2];
			int eBoard = vals[3];
			int eInd = vals[4];
			int flags = vals[5];

			if ((PieceType)(Math.Abs(state.boards[sBoard][sInd])-1) == PieceType.King)
			{
				int[] king;
				if (state.boards[sBoard][sInd] > 0)
					king = state.k1;
				else
					king = state.k2;
				king[0] = eBoard;
				king[1] = eInd / 12;
				king[2] = eInd % 12;
			}
			if ((MoveType)type != MoveType.Swoop)
			{
				// Change the value on the board representation
				state.boards[eBoard][eInd] = state.boards[sBoard][sInd];
				state.boards[sBoard][sInd] = 0;
			}
			else
			{
				state.boards[eBoard][eInd] = 0;
			}

			// Switch turns 
			state.WhiteToMove = !state.WhiteToMove;
		}

		public static void UndoBitMove(SimpleState state, int move)
		{

		}

		//###############################################################################################

		public static void VerifyAndAddMoves(SimpleState state, ref List<int> moves, string key, int board, int ind, int color, MoveType type)
		{
			if (type == MoveType.MoveOrCapture)
			{
				VerifyAndAddMoves(state, ref moves, key, board, ind, color, MoveType.Regular);
				VerifyAndAddMoves(state, ref moves, key, board, ind, color, MoveType.Capture);
				return;
			}

			List<(int, int, int)> possibleMoves = MoveDictionary[key][board, ind/12, ind%12];

			foreach ((int b, int r, int c) in possibleMoves)
			{
				int move = CreateBitMove((int)type, board, ind, b, (r*12+c), 0);
				if (IsValidBitMove(state, move, color))
					moves.Add(move);
			}
		}

		public static void CheckBlockAndAdd(SimpleState state, ref List<int> moves, string key, int board, int ind, int color, MoveType type, List<string> slideTypes)
		{
			if (type == MoveType.MoveOrCapture)
			{
				CheckBlockAndAdd(state, ref moves, key, board, ind, color, MoveType.Regular, slideTypes);
				CheckBlockAndAdd(state, ref moves, key, board, ind, color, MoveType.Capture, slideTypes);
				return;
			}
			
			List<(int, int, int)> possibleMoves = MoveDictionary[key][board, ind / 12, ind % 12];
			List<(int, int, int)> blocked = GetBlocked(state, possibleMoves, slideTypes, board, ind/12, ind%12);

			foreach ((int b, int r, int c) in possibleMoves)
			{
				if (!blocked.Contains((b, r, c)))
				{
					int move = CreateBitMove((int)type, board, ind, b, (r * 12 + c), 0);
					if (IsValidBitMove(state, move, color))
						moves.Add(move);
				}
			}
		}

		//###############################################################################################

		public static List<int> SylphBitMoves(SimpleState state, int color, int board, int ind)
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

		public static List<int> GriffonBitMoves(SimpleState state, int color, int board, int ind)
		{
			List<int> moves = new List<int>();
			if (board == 3)
				VerifyAndAddMoves(state, ref moves, "TopGrif", board, ind, color, MoveType.MoveOrCapture);
			if (board == 2)
				VerifyAndAddMoves(state, ref moves, "MidGrif", board, ind, color, MoveType.MoveOrCapture);
			return moves;
		}

		public static List<int> DragonBitMoves(SimpleState state, int color, int board, int ind)
		{
			List<int> moves = new List<int>();
			List<string> slideTypes = new List<string> { "diagUR", "diagUL", "diagDR", "diagDL" };
		
			CheckBlockAndAdd(state, ref moves, "Dragon", board, ind, color, MoveType.MoveOrCapture, slideTypes);
			VerifyAndAddMoves(state, ref moves, "SwoopDragon", board, ind, color, MoveType.Swoop);
			return moves;
		}
		
		public static List<int> WarriorBitMoves(SimpleState state, int color, int board, int ind)
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

		public static List<int> OliphantBitMoves(SimpleState state, int color, int board, int ind)
		{
			List<int> moves = new List<int>();
			List<string> slideTypes = new List<string> { "forward", "backward", "left", "right" };
			CheckBlockAndAdd(state, ref moves, "Oliphant", board, ind, color, MoveType.MoveOrCapture, slideTypes);

			return moves;
		}

		public static List<int> UnicornBitMoves(SimpleState state, int color, int board, int ind)
		{
			List<int> moves = new List<int>();
			VerifyAndAddMoves(state, ref moves, "Unicorn", board, ind, color, MoveType.MoveOrCapture);
			return moves;
		}

		public static List<int> HeroBitMoves(SimpleState state, int color, int board, int ind)
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

		public static List<int> ThiefBitMoves(SimpleState state, int color, int board, int ind)
		{
			List<int> moves = new List<int>();
			List<string> slideTypes = new List<string> { "diagUR", "diagUL", "diagDR", "diagDL" };
			CheckBlockAndAdd(state, ref moves, "Thief", board, ind, color, MoveType.MoveOrCapture, slideTypes);
			return moves;
		}

		public static List<int> ClericBitMoves(SimpleState state, int color, int board, int ind)
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

		public static List<int> MageBitMoves(SimpleState state, int color, int board, int ind)
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

		public static List<int> KingBitMoves(SimpleState state, int color, int board, int ind)
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

		public static List<int> PaladinBitMoves(SimpleState state, int color, int board, int ind)
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

		public static List<int> DwarfBitMoves(SimpleState state, int color, int board, int ind)
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

		public static List<int> BasiliskBitMoves(SimpleState state, int color, int board, int ind)
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

		public static List<int> ElementalBitMoves(SimpleState state, int color, int board, int ind)
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

		public static void PrintState(SimpleState state, int fileNum)
		{
			string filename = "Assets/Out/simplestate" + fileNum + ".txt";
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
				file.WriteLine("\n\n");
				toPrint += ("\n\n");
			}
			file.Close();
			M.print(toPrint);
		}
	}
}
