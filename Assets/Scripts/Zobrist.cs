using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Dragonchess
{
	using static Gamestate;
	using static BitPiece;
	using static BitMove;
	using static BitMoveController;

	public class Zobrist : MonoBehaviour
	{
		public static long[] keys;
		public static Dictionary<int, long> pieceZobristVals;
		public static long whiteToMove = 6148914691236517205;
		public static long blackToMove = 1229782938247303441;

		static Zobrist()
		{
			pieceZobristVals = new Dictionary<int, long>();

			int from = 1111111;
			int to = Int32.MaxValue;

			var random = new System.Random();
			keys = new long[8642];
			HashSet<long> magicHashKeys = new HashSet<long>();
			
			while (magicHashKeys.Count < 8642)
			{
				int left = random.Next(from, to);
				int right = random.Next(from, to);
				long key = left << 32 | right;
				if (key != 6148914691236517205 && key != 1229782938247303441)
					magicHashKeys.Add(key);
			}

			List<long> magicKeys = magicHashKeys.ToList();
			int piece;
			for (int b = 3; b > 0; b--)
			{
				for (int r = 0; r < 8; r++)
				{
					for (int c = 0; c < 12; c++)
					{
						for (int t = 0; t < 15; t++)
						{
							piece = NewBitPiece(0, t, b, r, c);
							pieceZobristVals[piece] = magicKeys[0];
							magicKeys.RemoveAt(0);
							piece = NewBitPiece(1, t, b, r, c);
							pieceZobristVals[piece] = magicKeys[0];
							magicKeys.RemoveAt(0);
						}
					}
				}
			}
		}

		public static long GetStateHash(Gamestate state)
		{
			List<int> p1 = GetPieces(state, true);
			List<int> p2 = GetPieces(state, false);
			long hashval = 0;

			for (int i = 0; i < p1.Count; i++)
			{
				int piece = p1[i];
				long pieceHash = pieceZobristVals[piece];
				hashval = hashval ^ pieceHash;
			}
			for (int i = 0; i < p2.Count; i++)
			{
				int piece = p2[i];
				long pieceHash = pieceZobristVals[piece];
				hashval = hashval ^ pieceHash;
			}

			if (state.WhiteToMove)
				hashval = hashval ^ whiteToMove;
			else
				hashval = hashval ^ blackToMove;

			return hashval;
		}
	}
}
