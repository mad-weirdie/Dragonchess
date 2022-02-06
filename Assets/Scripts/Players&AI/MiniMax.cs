using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Dragonchess
{
	using static Gamestate;
	using static BitPiece;
	using static BitMove;
	using static BitMoveController;
	using M = MonoBehaviour;

	public class Node
	{
        public List<Node> children;
		public int moveToState;
		public int value;

        public Node(Gamestate s, int m)
		{
			children = new List<Node>();
			value = EvaluateGamestate(s);
			moveToState = m;
		}
	}
    public class Tree
	{
        public Node root;

        public Tree(Gamestate state)
		{
            root = new Node(state, 0);
		}
	}

    public class MiniMax
    {
		public Tree mTree;
		public List<int>[] bestMaxs;
		public List<int>[] bestMins;
		int maxDepth;

		public MiniMax(Gamestate state, int d)
		{
			bestMaxs = new List<int>[maxDepth+1];
			bestMins = new List<int>[maxDepth+1];
			for (int i = 0; i < maxDepth+1; i++)
			{
				bestMaxs[i] = new List<int>();
				bestMins[i] = new List<int>();
			}
			mTree = new Tree(state);
			maxDepth = d;
		}

		// Minimax with alpha-beta pruning
		static int minimax(ref Gamestate state, Node current, int depth, int maxDepth,
						   bool maximizingPlayer, int alpha, int beta)
		{
			// Terminating condition, i.e leaf node is reached
			if (depth == maxDepth)
				return current.value;
			List<int> moves = GetPossibleMoves(state, maximizingPlayer);

			if (maximizingPlayer)
			{
				for (int i = 0; i < moves.Count; i++)
				{
					int move = moves[i];
					int start = GetStartPiece(state, move);
					int end = GetEndPiece(state, move);

					DoBitMove(ref state, move);  // False = don't update UI
					Node next = new Node(state, move);
					current.children.Add(next);
					int val = minimax(ref state, next, depth+1, maxDepth, false, alpha, beta);
					alpha = Math.Max(alpha, val);
					current.value = alpha;
					UndoBitMove(ref state, move, start, end);

					// Alpha Beta Pruning
					if (beta <= alpha)
						return alpha;
				}
				return alpha;
			}
			else
			{
				for (int i = 0; i < moves.Count; i++)
				{
					int move = moves[i];
					int start = GetStartPiece(state, move);
					int end = GetEndPiece(state, move);

					DoBitMove(ref state, move);  // False = don't update UI
					Node next = new Node(state, move);
					current.children.Add(next);
					int val = minimax(ref state, next, depth+1, maxDepth, true, alpha, beta);
					beta = Math.Min(beta, val);
					current.value = beta;
					UndoBitMove(ref state, move, start, end);

					// Alpha Beta Pruning
					if (beta <= alpha)
						return beta;
				}
				return beta;
			}
		}

		public static int GetMiniMaxMove(Gamestate state, int searchDepth, List<int> moves)
		{
			MiniMax miniMax = new MiniMax(state, searchDepth);

			int best = minimax(ref state, miniMax.mTree.root, 0, searchDepth, state.WhiteToMove, int.MinValue, int.MaxValue);
			List<int> bestMoves = new List<int>();
			List<Node> children = miniMax.mTree.root.children;

			for (int i = 0; i < children.Count; i++)
			{
				Node child = children[i];
				if (child.value == best)
					return child.moveToState;
			}
			M.print("ERROR: NO MOVES FOUND. BEST: " + best);
			return 0;
		}
    }
}

