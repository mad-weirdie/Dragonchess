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
		public int moveToState;
		public int value;

        public Node(int v, int m)
		{
			moveToState = m;
			value = v;
		}
	}

    public class Tree
	{
        public Node root;

        public Tree(Gamestate state)
		{
            root = new Node(int.MinValue, 0);
		}
	}

	public class NegaMax
	{
		public Tree tree;
		Gamestate gamestate;
		int maxDepth;

		public NegaMax(Gamestate state, int d)
		{
			gamestate = state;
			tree = new Tree(state);
			maxDepth = d;
		}

		// NegaMax with alpha-beta pruning
		public int NegaMaxSearch(Node current, int depth, int color, int alpha, int beta)
		{
			// Terminating condition, i.e leaf node is reached
			if (depth == maxDepth)
				return color*current.value;

			// Get possible moves from this state
			List<int> moves = GetPossibleMoves(gamestate, gamestate.WhiteToMove);
			// TODO: THIS IS THE STEP WHERE WE ORDER THE MOVES!

			int value = int.MinValue;

			// Evaluate every move we could take from the current gamestate
			for (int i = 0; i < moves.Count; i++)
			{
				int move = moves[i];
				// Store information about the move so that we may undo it after
				int start = GetStartPiece(gamestate, move);
				int end = GetEndPiece(gamestate, move);

				// Do the bit move
				DoBitMove(ref gamestate, move);
				// Create & add new node based on the updated gamestate
				Node next = new Node(EvaluateGamestate(gamestate), move);
				value = Math.Max(value, -NegaMaxSearch(next, depth + 1, -color, -beta, -alpha));
				// Undo the bit move
				UndoBitMove(ref gamestate, move, start, end);
				alpha = Math.Max(alpha, value);

				if (depth == 0 && alpha > current.value)
				{
					current.moveToState = next.moveToState;
				}
				current.value = alpha;

				// Alpha Beta Pruning
				if (alpha >= beta)
					return value;
			}
			return value;
		}
		public static int GetNegaMaxMove(Gamestate state, int searchDepth)
		{
			NegaMax NegaMax = new NegaMax(state, searchDepth);
			int best;
			if (state.WhiteToMove)
				best = NegaMax.NegaMaxSearch(NegaMax.tree.root, 0, 1, int.MinValue, int.MaxValue);
			else
				best = NegaMax.NegaMaxSearch(NegaMax.tree.root, 0, -1, int.MinValue, int.MaxValue);

			//M.print("best value found by Negamax: " + best);
			return NegaMax.tree.root.moveToState;
		}
    }

	/*
	 *	Negamax with alpha beta pruning and transposition tables
		Transposition tables selectively memoize the values of nodes in the game
		tree. Transposition is a term reference that a given game board position
		can be reached in more than one way with differing game move sequences.

		When negamax searches the game tree, and encounters the same node multiple
		times, a transposition table can return a previously computed value of the
		node, skipping potentially lengthy and duplicate re-computation of the node's
		value. Negamax performance improves particularly for game trees with many paths
		that lead to a given node in common.

		The pseudo code that adds transposition table functions to negamax with alpha/beta pruning is given as follows:[1]

		function negamax(node, depth, α, β, color) is
			alphaOrig := α

			(* Transposition Table Lookup; node is the lookup key for ttEntry *)
			ttEntry := transpositionTableLookup(node)
			if ttEntry is valid and ttEntry.depth ≥ depth then
				if ttEntry.flag = EXACT then
					return ttEntry.value
				else if ttEntry.flag = LOWERBOUND then
					α := max(α, ttEntry.value)
				else if ttEntry.flag = UPPERBOUND then
					β := min(β, ttEntry.value)

				if α ≥ β then
					return ttEntry.value

			if depth = 0 or node is a terminal node then
				return color × the heuristic value of node

			childNodes := generateMoves(node)
			childNodes := orderMoves(childNodes)
			value := −∞
			for each child in childNodes do
				value := max(value, −negamax(child, depth − 1, −β, −α, −color))
				α := max(α, value)
				if α ≥ β then
					break

			(* Transposition Table Store; node is the lookup key for ttEntry *)
			ttEntry.value := value
			if value ≤ alphaOrig then
				ttEntry.flag := UPPERBOUND
			else if value ≥ β then
				ttEntry.flag := LOWERBOUND
			else
				ttEntry.flag := EXACT
			ttEntry.depth := depth	
			transpositionTableStore(node, ttEntry)

			return value
	 */
}

