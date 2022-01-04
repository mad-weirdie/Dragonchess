using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    public class AI : Player
    {
        public AI(Color c, PlayerType t) : base(c, PlayerType.AI) { }

        override public Move GetMove()
        {
            MonoBehaviour.print("calling AI getmove");
            return DumbRandom();
        }

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
            MonoBehaviour.print("The AI has chosen to move the " + piece.type + " at " + piece.pos.SquareName() + " to " + AIMove.end.SquareName() + ". MoveType: " + AIMove.type);
            return moves[move];
        }
    }
}
