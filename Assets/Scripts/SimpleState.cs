using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
	public class SimpleState
	{
		public int[,] b3;
		public int[,] b2;
		public int[,] b1;

		public int turn;    //white is 0
		public List<int> p1;
		public List<int> p2;

		public SimpleState()
		{
			b3 = new int[8, 12];
			b2 = new int[8, 12];
			b1 = new int[8, 12];

			p1 = new List<int>();
			p2 = new List<int>();

			turn = 0;
		}

		public SimpleState(Gamestate state)
		{
			b3 = new int[8, 12];
			b2 = new int[8, 12];
			b1 = new int[8, 12];

			p1 = new List<int>();
			p2 = new List<int>();

			turn = (int)state.ActivePlayer.color;
			for (int r = 0; r < 8; r++)
			{
				for (int c = 0; c < 12; c++)
				{
					b3[r, c] = 0;
					b2[r, c] = 0;
					b1[r, c] = 0;
				}
			}

			foreach (Piece piece in state.P1.pieces)
			{
				p1.Add((int)piece.type + 1);

				if (piece.pos.board == state.upperBoard)
				{
					b3[piece.pos.row, piece.pos.col] = (int)piece.type + 1;
				}
				else if (piece.pos.board == state.middleBoard)
				{
					b2[piece.pos.row, piece.pos.col] = (int)piece.type + 1;

				}
				else
				{
					b1[piece.pos.row, piece.pos.col] = (int)piece.type + 1;
				}
			}

			foreach (Piece piece in state.P2.pieces)
			{
				p2.Add((int)piece.type + 1);

				if (piece.pos.board == state.upperBoard)
				{
					b3[piece.pos.row, piece.pos.col] = ((int)piece.type + 1)*(-1);
				}
				else if (piece.pos.board == state.middleBoard)
				{
					b2[piece.pos.row, piece.pos.col] = ((int)piece.type + 1) * (-1);

				}
				else
				{
					b1[piece.pos.row, piece.pos.col] = ((int)piece.type + 1) * (-1);
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

			for (int r = 0; r < 8; r++)
			{
				for (int c = 0; c < 12; c++)
				{
					copy.b3[r, c] = state.b3[r, c];
					copy.b2[r, c] = state.b2[r, c];
					copy.b1[r, c] = state.b1[r, c];
				}
			}

			return copy;
		}


	}
}
