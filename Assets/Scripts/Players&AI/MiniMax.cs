using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using System.Threading;

/*
 * Transposition Table ideas:
 * https://en.wikipedia.org/wiki/Negamax#cite_note-Schaeffer-2
 */

namespace Dragonchess
{
	using static Gamestate;
	using static BitMove;
	using static BitMoveController;
	using static Zobrist;
	using M = MonoBehaviour;
	public enum TTFlag { EXACT, LOWERBOUND, UPPERBOUND };
	public class Node
	{
		public int value;
		public int moveToState;

        public Node(int v, int m)
		{

			value = v;
			moveToState = m;
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

	public class TranspositionTable
	{
		public Dictionary<long, TTableEntry> table;
		
		public TranspositionTable()
		{
			table = new Dictionary<long, TTableEntry>();
		}

		public TTableEntry TTLookup(long key)
		{
			TTableEntry val;
			if (!table.TryGetValue(key, out val))
				return null;
			else return val;
		}

		public void TTStore(long key, TTableEntry entry)
		{
			table[key] = entry;
		}
	}

	public class TTableEntry
	{
		public int value;
		public int move;
		public int depth;
		public TTFlag flag;

		public TTableEntry()
		{

		}

		public TTableEntry(int v, int d, TTFlag f)
		{
			value = v;
			depth = d;
			flag = f;
		}
	}

	public class MiniMax
	{
		public static TranspositionTable transpositionTable;
		public Tree tree;
		public long original_key;
		int maxDepth;
		public int movesEvaluated;

		static MiniMax()
		{
			transpositionTable = new TranspositionTable();
		}

		public MiniMax(Gamestate s, int d)
		{
			movesEvaluated = 0;
			tree = new Tree(s);
			original_key = GetStateHash(s, s.WhiteToMove);
			maxDepth = d;
		}

		// MiniMax with alpha-beta pruning
		public (int, int) MiniMaxSearch(ref Gamestate state, int depth, int alpha, int beta)
		{
			movesEvaluated++;
			int alphaOrig = alpha;
			int best_move = 0;
			if (depth == 0)
				return (EvaluateGamestate(state), best_move);
			//==========================================================================
			// Transposition Table Lookup
			//==========================================================================
			long key = GetStateHash(state, state.WhiteToMove);

			TTableEntry ttEntry = transpositionTable.TTLookup(key);
			if (ttEntry != null)
			{
				if (ttEntry.depth >= depth)
				{
					if (ttEntry.flag == TTFlag.EXACT)
						return (ttEntry.value, ttEntry.move);
					else if (ttEntry.flag == TTFlag.LOWERBOUND)
						alpha = Math.Max(alpha, ttEntry.value);
					else if (ttEntry.flag == TTFlag.UPPERBOUND)
						beta = Math.Min(beta, ttEntry.value);

					if (alpha >= beta)
						return (ttEntry.value, ttEntry.move);
				}
			}
			//==========================================================================
			// MiniMax Search (implemented using "Negamax" variation
			//==========================================================================
			List<int> moves = GetPossibleMoves(state, state.WhiteToMove);

			if (moves.Count == 0)
				return (EvaluateGamestate(state), best_move);
			//==========================================================================
			// TODO: THIS IS THE STEP WHERE WE ORDER THE MOVES!
			//==========================================================================
			/* (1) PV-Move : principal variation from previous iteration
			 * (2) Captures:
			 *		(a) Capturing the last moved piece with the least valuable attacker
			 *		(b) 
			 * 
			 * 
			 */

			if (ttEntry != null)
			{
				moves.Remove(ttEntry.move);
				moves.Insert(0, ttEntry.move);
			}

			int best_value = int.MinValue;
			// Iterate through the possible moves from this state, pruning as needed

			for (int i = 0; i < moves.Count; i++)
			{
				int move = moves[i];
				int start = GetStartPiece(state, move);
				int end = GetEndPiece(state, move);

				DoBitMove(ref state, move);
				int goodness = -MiniMaxSearch(ref state, depth-1, -beta, -alpha).Item1;
				UndoBitMove(ref state, move, start, end);

				if (best_value < goodness)
				{
					best_value = goodness;
					best_move = move;
				}
				if (alpha < best_value)
					alpha = best_value;
				
				// Alpha Beta Pruning
				if (alpha >= beta)
					break;
			}

			//==========================================================================
			// Transposition Table Store; node is the lookup key for ttEntry 
			//==========================================================================
			ttEntry = new TTableEntry();
			ttEntry.value = best_value;
			ttEntry.move = best_move;

			if (best_value <= alphaOrig)
				ttEntry.flag = TTFlag.UPPERBOUND;
			else if (best_value >= beta)
				ttEntry.flag = TTFlag.LOWERBOUND;
			else
				ttEntry.flag = TTFlag.EXACT;
			ttEntry.depth = depth;
			transpositionTable.TTStore(key, ttEntry);

			return (best_value, best_move);
			//==========================================================================
		}

		// Get the best move using the minimax algorithm
		public static int GetMiniMaxMove(Gamestate state, int searchDepth)
		{
			MiniMax MiniMax = new MiniMax(state, searchDepth);
			// Do preliminary search to order the moves...
			MiniMax.MiniMaxSearch(ref state, 1, int.MinValue + 100, int.MaxValue - 100);

			// Stopwatch to track how long this function takes...
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			(int bestVal, int bestMove) = MiniMax.MiniMaxSearch(ref state, searchDepth, int.MinValue+100, int.MaxValue-100);
			stopwatch.Stop();

			M.print("STATES EVALUATED: " + MiniMax.movesEvaluated);
			M.print("Time elapsed: " + stopwatch.Elapsed);
			return bestMove;
		}
    }
}

