using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Dragonchess
{
    using GC = GameController;
    using MC = MoveController;
	using M = MonoBehaviour;
	using static Game;
    using static MoveController;
	using static NegaMax;
	using static Gamestate;
	using static BitPiece;
	using static BitMove;
	using static BitMoveController;

	public enum AIDifficulty { Trivial, Easy, Medium, Hard, Expert};

    public class AI : Player
    {
		AIDifficulty difficulty;

        public AI(Color c, PlayerType t, AIDifficulty d) : base(c, PlayerType.AI) { this.difficulty = d; }

        override public void GetMove(Game state)
        {
			Gamestate sState = new Gamestate(state);
			List<int> moves = BitMoveController.GetPossibleMoves(sState, this.color == Color.White);
			Move next;

			int minmaxdepth = 3;

			if (this.difficulty == AIDifficulty.Trivial)
				next = DumbRandom(state, moves);
			else if (this.difficulty == AIDifficulty.Easy)
				next = SimpleEval(state, (int)this.color, moves);
			else if (this.difficulty == AIDifficulty.Medium)
				next = NegamaxEval(state, minmaxdepth);
			else
				next = DumbRandom(state, moves);

			if (this.color == Color.White)
				MonoBehaviour.print("P1(AI-" + this.difficulty + ") has chosen: " + next.MoveToString());
			if (this.color == Color.Black)
				MonoBehaviour.print("P2(AI-" + this.difficulty + ") has chosen: " + next.MoveToString());

			GameController.OnMoveReceived(state, state.ActivePlayer, next);
		}

		override public void OnClick(Game state) { return; }

        // ---------------------------- PART 1 ----------------------------
        // The Absolute Worst (TM) Chess AI imaginable - makes random moves
        public Move DumbRandom(Game state, List<int> moves)
        {
            int move = UnityEngine.Random.Range(0, moves.Count);
			return BitMoveToMove(state, moves[move]);
        }

        // Evaluate the board based on the current pieces
        public static float boardEval(Gamestate state)
		{
            int ret = 0;
			for (int b = 3; b > 0; b--)
			{
				for (int i = 0; i < 96; i++)
				{
					int val = state.boards[b][i];
					if (val != 0)
					{
						int type = -1 + Math.Abs(val);
						int weight = BitPiece.pieceValues[type+1];
						ret += (val / (type + 1)) * weight;
					}
				}
			}
			
            return ret;
		}

        // ---------------------------- PART 2 ----------------------------
        // Simple Board Evaluation select move with the highest evaluation
        public Move SimpleEval(Game state, int color, List<int> moves)
        {
			Gamestate s = new Gamestate(state);
			List<int> highEvalMoves = new List<int>();
            float currentBoardVal = boardEval(s);
            float moveVal = currentBoardVal;
            float maxVal = currentBoardVal;

			int enemyColor = (color + 1) % 2;

			for (int i = 0; i < moves.Count; i++)
			{
				int m = moves[i];
                moveVal = currentBoardVal;
				if ((MoveType)BitMoveType(m) == MoveType.Capture)
				{
					int pieceType = -1 + (Math.Abs(GetEndPiece(s, m)));
					int endPiece = NewBitPiece(enemyColor, pieceType, EndBoard(m), EndIndex(m)/12, EndIndex(m)%12);
					moveVal += GetPieceValue(endPiece);
				}
				if (moveVal > maxVal)
                {
                    highEvalMoves.Clear();
                    highEvalMoves.Add(m);
                    maxVal = moveVal;
                }
                else if (moveVal == maxVal)
                    highEvalMoves.Add(m);
			}

            int move = UnityEngine.Random.Range(0, highEvalMoves.Count-1);
            int moveChoice = highEvalMoves[move];
            return BitMoveToMove(state, moveChoice);
        }

        // ---------------------------- PART 3 ----------------------------
        // MinMax Board Evaluation to depth d
        public Move NegamaxEval(Game state, int d)
        {
			Gamestate sState = new Gamestate(state);
			int move = GetNegaMaxMove(sState, d);
			Move m = BitMoveToMove(state, move);
			return m;
        }
    }
}
