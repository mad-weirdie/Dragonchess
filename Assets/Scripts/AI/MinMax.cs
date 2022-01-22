using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Dragonchess
{
	using static Gamestate;
	using static MoveController;

    public class Node
	{
        public Gamestate state;
        public List<Node> children;
		public Move moveToState;
		public int value;

        public Node(Gamestate s, Move m)
		{
			state = s;
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
            root = new Node(state, null);
		}
	}

    public class MiniMax
    {
		// Minimax with alpha-beta pruning
		static int minimax(Node current, int depth, int maxDepth,
						   bool maximizingPlayer, int alpha, int beta)
		{
			Gamestate state = current.state;

			// Terminating condition, i.e leaf node is reached
			if (depth == maxDepth)
			{
				return current.value;
			}

			List<Move> moves = state.ActivePlayer.GetPossibleMoves(state);
			MoveController.RemoveIllegal(state, state.ActivePlayer, ref moves);

			if (maximizingPlayer)
			{
				int best = int.MinValue;
				foreach (Move move in moves)
				{
					Gamestate nextState = CopyGamestate(state);
					DoMove(nextState, Move.Convert(nextState, move), false);  // False = don't update UI
					Node next = new Node(nextState, move);
					current.children.Add(next);

					int val = minimax(next, depth+1, maxDepth, true, alpha, beta);
					best = Math.Max(best, val);
					alpha = Math.Max(alpha, best);
					current.value = best;

					// Alpha Beta Pruning
					if (beta <= alpha)
						break;
				}
				return best;
			}
			else
			{
				int best = int.MaxValue;
				foreach (Move move in moves)
				{
					Gamestate nextState = CopyGamestate(state);
					DoMove(nextState, Move.Convert(nextState, move), false);  // False = don't update UI
					Node next = new Node(nextState, move);
					current.children.Add(next);

					int val = minimax(next, depth+1, maxDepth, false, alpha, beta);
					best = Math.Min(best, val);
					beta = Math.Min(beta, best);
					current.value = best;

					// Alpha Beta Pruning
					if (beta <= alpha)
						break;
				}
				return best;
			}
		}

		public static Move GetMiniMaxMove(Gamestate state, int searchDepth, List<Move> moves)
		{
			Tree minimaxTree = new Tree(CopyGamestate(state));
			bool isMaximizing;

			if (state.ActivePlayer.color == Color.White)
				isMaximizing = true;
			else
				isMaximizing = false;

			int best = minimax(minimaxTree.root, 0, 2, isMaximizing, int.MinValue, int.MaxValue);

			foreach (Node child in minimaxTree.root.children)
			{
				if (child.value == best)
					return Move.Convert(state, child.moveToState);
			}
			return null;
		}
    }
}
