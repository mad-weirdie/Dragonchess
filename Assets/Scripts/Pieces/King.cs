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
        public override List<Move> GetMoves()
        {
            List<Move> moves = new List<Move>();
            Square current_square = this.pos;
            Layer layer = current_square.layer;
            int dir = 1;

            // Level 2 moves
            if (layer == Layer.Middle)
            {
                // Regular King moves
                Move.moveAttempt(moves, current_square, dir, 1, 0, 2, move_cap);
                Move.moveAttempt(moves, current_square, dir, 1, 1, 2, move_cap);
                Move.moveAttempt(moves, current_square, dir, 0, 1, 2, move_cap);
                Move.moveAttempt(moves, current_square, dir, -1, 1, 2, move_cap);
                Move.moveAttempt(moves, current_square, dir, -1, 0, 2, move_cap);
                Move.moveAttempt(moves, current_square, dir, -1, -1, 2, move_cap);
                Move.moveAttempt(moves, current_square, dir, 0, -1, 2, move_cap);
                Move.moveAttempt(moves, current_square, dir, 1, -1, 2, move_cap);

                // Straight down
                Move.moveAttempt(moves, current_square, dir, 0, 0, 1, move_cap);
                // Straight up
                Move.moveAttempt(moves, current_square, dir, 0, 0, 3, move_cap);
            }
            // Levels 1 and 3 moves (only back to level 2)
            else
            {
                Move.moveAttempt(moves, current_square, dir, 0, 0, 2, regular);
            }

            return moves;
        }
    }
}

