using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Dragonchess
{
	using static SimpleState;

    public class Node
	{
        public SimpleState state;
        public List<Node> children;
		public int moveToState;
		public int value;

        public Node(SimpleState s, int m)
		{
			state = s;
			children = new List<Node>();
			value = EvaluateSimpleState(s);
			moveToState = m;
		}
	}
    public class Tree
	{
        public Node root;

        public Tree(SimpleState state)
		{
            root = new Node(state, 0);
		}
	}

    public class MiniMax
    {
		// Minimax with alpha-beta pruning
		static int minimax(Node current, int depth, int maxDepth,
						   bool maximizingPlayer, int alpha, int beta)
		{
			SimpleState state = current.state;

			// Terminating condition, i.e leaf node is reached
			if (depth == maxDepth)
			{
				return current.value;
			}

			List<int> moves = GetPossibleMoves(state, maximizingPlayer);
			//intController.RemoveIllegal(state, state.ActivePlayer, ref moves);

			if (maximizingPlayer)
			{
				int m = 0;
				foreach (int move in moves)
				{
					m++;
					SimpleState nextState = CopySimpleState(state);
					DoBitMove(nextState, move);  // False = don't update UI
					Node next = new Node(nextState, move);
					current.children.Add(next);

					int val = minimax(next, depth+1, maxDepth, false, alpha, beta);
					alpha = Math.Max(alpha, val);
					current.value = alpha;

					// Alpha Beta Pruning
					if (beta <= alpha)
					{
						//MonoBehaviour.print("pruning");
						return alpha;
					}
				}
				return alpha;
			}
			else
			{
				foreach (int move in moves)
				{
					SimpleState nextState = CopySimpleState(state);
					DoBitMove(nextState, move);  // False = don't update UI
					Node next = new Node(nextState, move);
					current.children.Add(next);

					int val = minimax(next, depth+1, maxDepth, true, alpha, beta);
					beta = Math.Min(beta, val);
					current.value = beta;
					
					// Alpha Beta Pruning
					if (beta <= alpha)
					{
						//MonoBehaviour.print("pruning");
						return beta;
					}
				}
				return beta;
			}
		}

		public static int GetMiniMaxMove(SimpleState state, int searchDepth, List<int> moves)
		{
			Tree minimaxTree = new Tree(CopySimpleState(state));
			bool isMaximizing;

			if (state.WhiteToMove)
				isMaximizing = true;
			else
				isMaximizing = false;

			int best = minimax(minimaxTree.root, 0, searchDepth, isMaximizing, int.MinValue, int.MaxValue);
			EvaluateSimpleState(state);
			//MonoBehaviour.print("Best possible board value: " + best);

			foreach (Node child in minimaxTree.root.children)
			{
				if (child.value == best) 
					return child.moveToState;
			}
			return 0;
		}
    }
}

