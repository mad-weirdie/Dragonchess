using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
namespace Dragonchess
{
	public class SimpleState
	{
		public static char[] pieceCodes = {'.', 's', 'g', 'r', 'w', 'o', 'u', 'h', 't', 'c', 'm', 'k', 'p', 'd', 'b', 'e'};
		public int[] b3;
		public int[] b2;
		public int[] b1;

		public int turn;    //white is 0
		public List<int> p1;
		public List<int> p2;

		public SimpleState()
		{
			b3 = new int[96];
			b2 = new int[96];
			b1 = new int[96];

			p1 = new List<int>();
			p2 = new List<int>();

			turn = 0;
		}

		public SimpleState(Gamestate state)
		{
			b3 = new int[96];
			b2 = new int[96];
			b1 = new int[96];

			p1 = new List<int>();
			p2 = new List<int>();

			turn = (int)state.ActivePlayer.color;

			for (int i = 0; i < 96; i++)
			{ 
				b3[i] = 0;
				b2[i] = 0;
				b1[i] = 0;
			}

			foreach (Piece piece in state.P1.pieces)
			{
				byte type = (byte)((int)(piece.type));
				byte board = (byte)(piece.pos.board);
				byte row = (byte)(piece.pos.row);
				byte col = (byte)(piece.pos.col);

				byte[] bytes = { type, board, row, col };
				int bPiece = BitConverter.ToInt32(bytes, 0);

				p1.Add(bPiece);
				int ind = piece.pos.row * 12 + piece.pos.col;
				if (piece.pos.board == state.upperBoard.layer_int_val)
				{
					b3[ind] = 1;
				}
				else if (piece.pos.board == state.middleBoard.layer_int_val)
				{
					b2[ind] = 1;
				}
				else
				{
					b1[ind] = 1;
				}
			}

			foreach (Piece piece in state.P2.pieces)
			{
				byte type = (byte)((int)(piece.type));
				type = (byte)(0x80 | type);
				byte board = (byte)(piece.pos.board);
				byte row = (byte)(piece.pos.row);
				byte col = (byte)(piece.pos.col);

				byte[] bytes = { type, board, row, col };
				int bPiece = BitConverter.ToInt32(bytes, 0);

				p2.Add(bPiece);
				int ind = piece.pos.row * 12 + piece.pos.col;
				if (piece.pos.board == state.upperBoard.layer_int_val)
				{
					b3[ind] = (-1);
				}
				else if (piece.pos.board == state.middleBoard.layer_int_val)
				{
					b2[ind] =  (-1);
				}
				else
				{
					b1[ind] = (-1);
				}
			}
		}

		public static SimpleState Copy(SimpleState state)
		{
			SimpleState copy = new SimpleState();
			copy.turn = state.turn;

			foreach (int p in state.p1)
				copy.p1.Add(p);
			foreach (int p in state.p2)
				copy.p2.Add(p);

			for (int i = 0; i < 96; i++)
			{
				copy.b3[i] = state.b3[i];
				copy.b2[i] = state.b2[i];
				copy.b1[i] = state.b1[i];
			}

			return copy;
		}

		public static void PrintState(Gamestate state)
		{
			SimpleState ss = new SimpleState(state);

			string filename = "Assets/Out/simplestate.txt";
			StreamWriter file = new StreamWriter(filename);

			List<int[]> boards = new List<int[]> { ss.b3, ss.b2, ss.b1 };
			foreach (int[] board in boards)
			{
				for (int r = 7; r >= 0; r--)
				{
					string row = "";
					for (int c = 0; c < 12; c++)
					{
						int ind = r*12 + c;
						if (board[ind] < 0)
						{
							row += pieceCodes[Math.Abs(board[ind])].ToString().ToUpper();
						}
						else
							row += pieceCodes[board[ind]];
						row += String.Format(" ");
					}
					file.WriteLine(row);
				}
				file.WriteLine("\n\n");
			}
			file.Close();
		}

		public long GenPseudoLegal(int piece)
		{
			byte[] byteArray = BitConverter.GetBytes(piece);
			byte type = byteArray[0];
			byte color = (byte)((type & 0x80) >> 7);
			type = (byte)(type & 0x7f);
			byte board = byteArray[1];
			byte row = byteArray[2];
			byte col = byteArray[3];
			int ind = (int)row * 12 + (int)col;

			/*
			MonoBehaviour.print("color: " + (int)color);
			MonoBehaviour.print("type: " + (int)type);
			MonoBehaviour.print("board: " + (int)board);
			MonoBehaviour.print("row: " + (int)row);
			MonoBehaviour.print("col: " + (int)col);
			*/

			int t = (int)type;


			if (t == 0)
				SylphBitMoves(this, color, ind);
			else if (t == 1)
				GriffonBitMoves(this, color, ind);
			else if (t == 2)
				DragonBitMoves(this, color, ind);
			else if (t == 3)
				WarriorBitMoves(this, color, ind);
			else if (t == 4)
				OliphantBitMoves(this, color, ind);
			else if (t == 5)
				UnicornBitMoves(this, color, ind);
			else if (t == 6)
				HeroBitMoves(this, color, ind);
			else if (t == 7)
				ThiefBitMoves(this, color, ind);
			else if (t == 8)
				ClericBitMoves(this, color, ind);
			else if (t == 9)
				MageBitMoves(this, color, ind);
			else if (t == 10)
				KingBitMoves(this, color, ind);
			else if (t == 11)
				PaladinBitMoves(this, color, ind);
			else if (t == 12)
				DwarfBitMoves(this, color, ind);
			else if (t == 13)
				BasiliskBitMoves(this, color, ind);
			else
				ElementalBitMoves(this, color, ind);

			return 0;
		}

		/*
			  northwest    north   northeast
			  noWe         nort         noEa
					  -13   -12  -11
						  \  |  /
			  west    -1 <-  0 -> +1    east
						  /  |  \
					  -11   +12  -13
			  soWe         sout         soEa
			  southwest    south   southeast

		*/
		/*On level 3:
         *  i)  can move one step diagonally forward or capture one step
         *      straight forward
         *  ii) can capture on the square directly below the sylph(on level 2)
         *  
         *  On level 2:
         *  i)  can move to the square directly above(on level 3) or to one of the player's
         *      six Sylph starting squares
         */
		public static List<long> SylphBitMoves(SimpleState state, int color, int ind)
		{
			List<long> moves = new List<long>();
			//int dir = 1;	// determines forwards vs backwards

			//if (color == 1)
				//dir = -1;

			// Movetypes: 0: regular, 1: capture, 2: swoop
			

			return moves;
		}

		public static List<int> GriffonBitMoves(SimpleState state, int color, int ind)
		{
			List<int> moves = new List<int>();
			return moves;
		}
		public static List<int> DragonBitMoves(SimpleState state, int color, int ind)
		{
			List<int> moves = new List<int>();
			return moves;
		}
		public static List<int> WarriorBitMoves(SimpleState state, int color, int ind)
		{
			List<int> moves = new List<int>();
			return moves;
		}

		public static List<int> OliphantBitMoves(SimpleState state, int color, int ind)
		{
			List<int> moves = new List<int>(); return moves;
		}

		public static List<int> UnicornBitMoves(SimpleState state, int color, int ind)
		{
			List<int> moves = new List<int>(); return moves;
		}

		public static List<int> HeroBitMoves(SimpleState state, int color, int ind)
		{
			List<int> moves = new List<int>(); return moves;
		}

		public static List<int> ThiefBitMoves(SimpleState state, int color, int ind)
		{
			List<int> moves = new List<int>(); return moves;
		}

		public static List<int> ClericBitMoves(SimpleState state, int color, int ind)
		{
			List<int> moves = new List<int>(); return moves;
		}

		public static List<int> MageBitMoves(SimpleState state, int color, int ind)
		{
			List<int> moves = new List<int>(); return moves;
		}

		public static List<int> KingBitMoves(SimpleState state, int color, int ind)
		{
			List<int> moves = new List<int>(); return moves;
		}

		public static List<int> PaladinBitMoves(SimpleState state, int color, int ind)
		{
			List<int> moves = new List<int>(); return moves;
		}

		public static List<int> DwarfBitMoves(SimpleState state, int color, int ind)
		{
			List<int> moves = new List<int>(); return moves;
		}

		public static List<int> BasiliskBitMoves(SimpleState state, int color, int ind)
		{
			List<int> moves = new List<int>(); return moves;
		}

		public static List<int> ElementalBitMoves(SimpleState state, int color, int ind)
		{
			List<int> moves = new List<int>(); return moves;
		}

	}
}
