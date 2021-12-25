using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    /* ----------- King ------------
     * 
     */
    public class King : Piece
    {
        public King() : base(PieceType.King) { }
        public override ArrayList GetMoves()
        {
            ArrayList moves = new ArrayList();
            Square current_square = this.location;
            Layer layer = current_square.layer;
            int dir;

            // Definition of "forward, left, right" changes based on piece color
            if (this.color == Color.White)
                dir = 1;
            else
                dir = -1;

            // Level 2 moves
            if (layer == Layer.Middle)
            {
                // Regular King moves
                Move.moveAttempt(moves, current_square, dir, 1, 0, 2, regular);
                Move.moveAttempt(moves, current_square, dir, 1, 1, 2, capture);
                Move.moveAttempt(moves, current_square, dir, 0, 1, 2, regular);
                Move.moveAttempt(moves, current_square, dir, -1, 1, 2, capture);
                Move.moveAttempt(moves, current_square, dir, -1, 0, 2, regular);
                Move.moveAttempt(moves, current_square, dir, -1, -1, 2, capture);
                Move.moveAttempt(moves, current_square, dir, 0, -1, 2, regular);
                Move.moveAttempt(moves, current_square, dir, 1, -1, 2, capture);

                Move.moveAttempt(moves, current_square, dir, 1, 0, 2, regular);
                Move.moveAttempt(moves, current_square, dir, 1, 1, 2, capture);
                Move.moveAttempt(moves, current_square, dir, 0, 1, 2, regular);
                Move.moveAttempt(moves, current_square, dir, -1, 1, 2, capture);
                Move.moveAttempt(moves, current_square, dir, -1, 0, 2, regular);
                Move.moveAttempt(moves, current_square, dir, -1, -1, 2, capture);
                Move.moveAttempt(moves, current_square, dir, 0, -1, 2, regular);
                Move.moveAttempt(moves, current_square, dir, 1, -1, 2, capture);

                // Straight down
                Move.moveAttempt(moves, current_square, dir, 0, 0, 1, regular);
                Move.moveAttempt(moves, current_square, dir, 0, 0, 1, capture);
                // Straight up
                Move.moveAttempt(moves, current_square, dir, 0, 0, 3, regular);
                Move.moveAttempt(moves, current_square, dir, 0, 0, 3, capture);
            }

            else
            {
                Move.moveAttempt(moves, current_square, dir, 0, 0, 2, regular);
            }

            return moves;
        }
    }
}

