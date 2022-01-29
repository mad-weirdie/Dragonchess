using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    using GC = GameController;
    using MC = MoveController;
	using M = MonoBehaviour;
	using static Gamestate;
    using static MoveController;
	using static MiniMax;
	using static SimpleState;

    public class AI : Player
    {
        public AI(Color c, PlayerType t) : base(c, PlayerType.AI) { }

        override public void GetMove(Gamestate state)
        {
			SimpleState sState = new SimpleState(state);
			List<int> possibleMoves = SimpleState.GetPossibleMoves(sState, this.color == Color.White);

			//Move next = DumbRandom();
			//Move next = SimpleEval(state, all_possible_moves);
			int minmaxdepth = 3;
			Move next = MinMaxEval(state, minmaxdepth, possibleMoves);
			MonoBehaviour.print("The " + color + " AI has chosen " + next.MoveToString());
			GameController.OnMoveReceived(state, state.ActivePlayer, next);
		}

		override public void OnClick(Gamestate state) { return; }

        // ---------------------------- PART 1 ----------------------------
        // The Absolute Worst (TM) Chess AI imaginable - makes random moves
        public Move DumbRandom(Gamestate state, List<Move> moves)
        {
            Piece piece = null;
            int move = 0;

            move = Random.Range(0, moves.Count);
            Move AIMove = moves[move];
            MonoBehaviour.print("The AI has chosen to move the " +
                piece.type + " at " + piece.pos.SquareName() + " to " +
                AIMove.end.SquareName() + ". MoveType: " + AIMove.type);
            return moves[move];
        }

        // Evaluate the board based on the current pieces
        public static float boardEval(Gamestate state, List<Move> moves)
		{
			Player player = state.ActivePlayer;
            int ret = 0;
            foreach (Piece p in player.pieces)
                ret += p.value;
            foreach (Piece p in GetEnemy(state, player).pieces)
		    {
                ret -= p.value;
		    }
            return ret;
		}

        // ---------------------------- PART 2 ----------------------------
        // Simple Board Evaluation select move with the highest evaluation
        public Move SimpleEval(Gamestate state, List<Move> moves)
        {
            List<Move> highEvalMoves = new List<Move>();
            float currentBoardVal = boardEval(state, moves);
            float moveVal = currentBoardVal;
            float maxVal = currentBoardVal;

            List<Piece> piecesCopy = new List<Piece>(pieces);

            foreach (Move m in moves)
			{
                moveVal = currentBoardVal;
                //if (GC.MC.WouldCheckmate(m))
                    // return m;

                if (m.IsType(MoveType.Capture))
                    moveVal += m.end.piece.value;
                if (moveVal > maxVal)
                {
					MonoBehaviour.print("found better move: " + m.MoveToString());
                    highEvalMoves.Clear();
                    highEvalMoves.Add(m);
                    maxVal = moveVal;
                }
                else if (moveVal == maxVal)
                    highEvalMoves.Add(m);
			}

            int move = Random.Range(0, highEvalMoves.Count-1);
            Move moveChoice = highEvalMoves[move];
            moveChoice.piece = moveChoice.start.piece;
            MonoBehaviour.print("The AI has chosen " + moveChoice.MoveToString());
            return moveChoice;
        }

        // ---------------------------- PART 3 ----------------------------
        // MinMax Board Evaluation to depth d\
        
        public Move MinMaxEval(Gamestate state, int d, List<int> possibleMoves)
        {
			SimpleState sState = new SimpleState(state);
			int move = GetMiniMaxMove(sState, d, possibleMoves);
			Move m = BitMoveToMove(state, move);
			return m;
        }
    }
}