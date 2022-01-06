using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    using GC = GameController;
    using MC = MoveController;
    public class AI : Player
    {
        public AI(Color c, PlayerType t) : base(c, PlayerType.AI) { }

        override public Move GetMove()
        {
            //return DumbRandom();
            return MinMaxEval(3);
        }

        // ---------------------------- PART 1 ----------------------------
        // The Absolute Worst (TM) Chess AI imaginable - makes random moves
        public Move DumbRandom()
        {
            List<Move> moves = new List<Move>();
            Piece piece = null;
            int p, move = 0;

            while (moves.Count == 0)
            {
                p = Random.Range(0, this.pieces.Count);
                piece = pieces[p];
                moves = piece.GetMoves();
                MoveController.RemoveIllegal(this, ref moves);
            }

            move = Random.Range(0, moves.Count);
            Move AIMove = moves[move];
            MonoBehaviour.print("The AI has chosen to move the " +
                piece.type + " at " + piece.pos.SquareName() + " to " +
                AIMove.end.SquareName() + ". MoveType: " + AIMove.type);
            return moves[move];
        }

        // Evaluate the board based on the current pieces
        public float boardEval()
		{
            int ret = 0;
            foreach (Piece p in pieces)
                ret += p.value;
            foreach (Piece p in GC.GetEnemy(this).pieces)
		    {
                ret -= p.value;
		    }
            return ret;
		}

        public bool WouldCheckmate(Move m)
        {
            if (m.type == Move.MoveType.Capture)
            {
                // Pretend to move attacking piece
                Piece attacking_piece = m.start.piece;
                Piece captured_piece = m.end.piece;
                m.start.occupied = false;

                // Pretend to capture attacked piece
                attacking_piece.pos = m.end;
                m.end.occupied = true;
                m.end.piece = attacking_piece;

                // Remove captured piece from enemy's list of pieces
                GC.GetEnemy(this).pieces.Remove(captured_piece);

                if (GC.IsCheck(GC.GetEnemy(this)))
                {
                    if (GC.IsCheckmate(GC.GetEnemy(this)))
                    {
                        GC.GetEnemy(this).pieces.Add(captured_piece);
                        return true;
                    }
                }

                // Move attacking piece back to original location
                m.start.occupied = true;
                m.start.piece = attacking_piece;
                m.end.piece = captured_piece;
                attacking_piece.pos = m.start;
                captured_piece.pos = m.end;

                // Re-add captured piece back to enemy's list of pieces
                GC.GetEnemy(this).pieces.Add(captured_piece);
            }
            else if (m.type == Move.MoveType.Swoop)
            {
                // Pretend to capture the attacked piece
                Piece attacking_piece = m.start.piece;
                Piece captured_piece = m.end.piece;
                m.end.occupied = false;

                // Remove captured piece from enemy's list of pieces
                GC.GetEnemy(this).pieces.Remove(captured_piece);

                if (GC.IsCheck(GC.GetEnemy(this)))
                {
                    if (GC.IsCheckmate(GC.GetEnemy(this)))
                    {
                        GC.GetEnemy(this).pieces.Add(captured_piece);
                        return true;
                    }
                }

                GC.GetEnemy(this).pieces.Add(captured_piece);
            }
            else if (m.type == Move.MoveType.Regular)
            {
                Piece moving_piece = m.start.piece;
                m.start.occupied = false;
                m.end.occupied = true;
                moving_piece.pos = m.end;
                m.end.piece = moving_piece;

                if (GC.IsCheck(GC.GetEnemy(this)))
                    if (GC.IsCheckmate(GC.GetEnemy(this)))
                        return true;

                m.start.occupied = true;
                m.end.occupied = false;
                m.start.piece = moving_piece;
                moving_piece.pos = m.start;
                m.end.piece = null;
            }
            return false;
        }

        // ---------------------------- PART 2 ----------------------------
        // Simple Board Evaluation select move with the highest evaluation
        public Move SimpleEval()
        {
            List<Move> highEvalMoves = new List<Move>();
            float currentBoardVal = boardEval();
            float maxVal = currentBoardVal;

            List<Piece> piecesCopy = new List<Piece>(pieces);

            foreach (Piece p in piecesCopy)
			{
                List<Move> moves = p.GetMoves();
                MoveController.RemoveIllegal(this, ref moves);
                foreach (Move m in moves)
				{
                    if (WouldCheckmate(m))
                        return m;

                    if (m.type == Move.MoveType.Capture)
                        currentBoardVal += m.end.piece.value;
                    if (currentBoardVal > maxVal)
                    {
                        highEvalMoves.Clear();
                        highEvalMoves.Add(m);
                        maxVal = currentBoardVal;
                    }
                    else if (currentBoardVal == maxVal)
                        highEvalMoves.Add(m);
				}
			}

            int move = Random.Range(0, highEvalMoves.Count);
            Move moveChoice = highEvalMoves[move];
            moveChoice.piece = moveChoice.start.piece;
            MonoBehaviour.print("The AI has chosen " + moveChoice.MoveToString());
            return moveChoice;
        }

        // ---------------------------- PART 3 ----------------------------
        // MinMax Board Evaluation to depth d
        public Move MinMaxEval(int d)
        {
            List<Move> highEvalMoves = new List<Move>();
            float currentBoardVal = boardEval();
            float maxVal = currentBoardVal;

            List<Piece> piecesCopy = new List<Piece>(pieces);

            foreach (Piece p in piecesCopy)
            {
                List<Move> moves = p.GetMoves();
                MoveController.RemoveIllegal(this, ref moves);
                foreach (Move m in moves)
                {
                    if (WouldCheckmate(m))
                        return m;

                    if (m.type == Move.MoveType.Capture)
                        currentBoardVal += m.end.piece.value;
                    if (currentBoardVal > maxVal)
                    {
                        highEvalMoves.Clear();
                        highEvalMoves.Add(m);
                        maxVal = currentBoardVal;
                    }
                    else if (currentBoardVal == maxVal)
                        highEvalMoves.Add(m);
                }
            }

            int move = Random.Range(0, highEvalMoves.Count);
            Move moveChoice = highEvalMoves[move];
            moveChoice.piece = moveChoice.start.piece;
            MonoBehaviour.print("The AI has chosen " + moveChoice.MoveToString());
            return moveChoice;
        }
    }
}
