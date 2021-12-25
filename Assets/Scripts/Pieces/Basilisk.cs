using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    /* ----------- Basilisk ------------
     * 
     */
    public class Basilisk : Piece
    {
        public Basilisk() : base(PieceType.Basilisk) { }
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

            /* Basilisk moves
             * - can move and capture one step diagonally forward or straight
             *   forward on level 1, or move one step straight backward
             * 
             * - automatically freezes(immobilizes) an enemy piece on the
             *   squaredirectly above on level 2, whether the Basilisk moves
             *   to the space below or the enemy moves to the space above, and
             *   until the Basilisk moves away or is captured.
             */

            // One step forward (move or capture)
            Move.moveAttempt(moves, current_square, dir, 1, 0, 1, regular);
            Move.moveAttempt(moves, current_square, dir, 1, 0, 1, capture);

            // One step backwards (move only)
            Move.moveAttempt(moves, current_square, dir, -1, 0, 1, regular);

            // Right forward diagonal (move or capture)
            Move.moveAttempt(moves, current_square, dir, 1, 1, 1, regular);
            Move.moveAttempt(moves, current_square, dir, 1, 1, 1, capture);

            // Left forward diagonal (move or capture)
            Move.moveAttempt(moves, current_square, dir, 1, -1, 1, regular);
            Move.moveAttempt(moves, current_square, dir, 1, -1, 1, capture);

            return moves;
        }
    }
}

