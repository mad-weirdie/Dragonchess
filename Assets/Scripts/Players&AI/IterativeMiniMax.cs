using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
	using static Gamestate;
	using static BitPiece;
	using static BitMove;
	using static BitMoveController;
	using M = MonoBehaviour;
	using static MiniMax;

	public class IterativeMiniMax
	{
		public MiniMax MMSearcher;
		public int maxDepth;

		public IterativeMiniMax(Gamestate s, int d)
		{
			maxDepth = d;
			MMSearcher = new MiniMax(s, d);
		}
	}
}
