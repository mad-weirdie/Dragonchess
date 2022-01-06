using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    /* ----------- Elemental ------------
     * On level 1:
        - can move and capture one or two steps orthogonally
        - can move one step diagonally;
        - can capture in the following pattern: one step orthogonally followed
          by the square directly above on level 2.

       On level 2:
        - can move and capture in the following pattern: the square directly
          below on level 1 followed by one step orthogonally.
     */
    public class Elemental : Piece
    {
        public Elemental() : base(PieceType.Elemental) { nameChar = "E"; value = 4; }
        public override List<Move> GetMoves()
        {
            List<Move> moves = new List<Move>();
            Square current_square = this.pos;
            Layer layer = current_square.layer;
            int dir;

            // Definition of "forward, left, right" changes based on piece color
            if (this.color == Color.White)
                dir = 1;
            else
                dir = -1;

            int[] orthog_shift = { 1, 2, -1, -2 };

            if (layer == Layer.Lower)
            {
                // Move and capture one or two steps orthogonally
                foreach (int shift in orthog_shift)
                {
                    if (Math.Abs(shift) == 2)
                    {
                        if (!Move.IsBlocked(current_square, dir, 0, shift / 2, 1, this.color))
                        {
                            Move.moveAttempt(moves, current_square, dir, 0, shift, 1, regular);
                            Move.moveAttempt(moves, current_square, dir, 0, shift, 1, capture);
                        }
                        if (!Move.IsBlocked(current_square, dir, shift / 2, 0, 1, this.color))
                        {
                            Move.moveAttempt(moves, current_square, dir, shift, 0, 1, regular);
                            Move.moveAttempt(moves, current_square, dir, shift, 0, 1, capture);
                        }
                    }
                    else
                    {
                        Move.moveAttempt(moves, current_square, dir, shift, 0, 1, regular);
                        Move.moveAttempt(moves, current_square, dir, shift, 0, 1, capture);
                        Move.moveAttempt(moves, current_square, dir, 0, shift, 1, regular);
                        Move.moveAttempt(moves, current_square, dir, 0, shift, 1, capture);
                    }

                    // Capture in the following pattern: one step orthogonally
                    // followed by the square directly above on level 2.
                    if (Math.Abs(shift) == 1)
                    {
                        Move.moveAttempt(moves, current_square, dir, shift, 0, 2, capture);
                        Move.moveAttempt(moves, current_square, dir, 0, shift, 2, capture);
                    }
                }
                // Can MOVE one step diagonally
                Move.moveAttempt(moves, current_square, dir, 1, 1, 1, regular);
                Move.moveAttempt(moves, current_square, dir, 1, -1, 1, regular);
                Move.moveAttempt(moves, current_square, dir, -1, 1, 1, regular);
                Move.moveAttempt(moves, current_square, dir, -1, -1, 1, regular);
            }
            // On level 2:
            // - can move and capture in the following pattern: the square directly
            //   below on level 1 followed by one step orthogonally.
            if (layer == Layer.Middle)
            {
                Move.moveAttempt(moves, current_square, dir, 1, 0, 1, regular);
                Move.moveAttempt(moves, current_square, dir, 0, 1, 1, regular);
                Move.moveAttempt(moves, current_square, dir, -1, 0, 1, regular);
                Move.moveAttempt(moves, current_square, dir, 0, -1, 1, regular);

                Move.moveAttempt(moves, current_square, dir, 1, 0, 1, capture);
                Move.moveAttempt(moves, current_square, dir, 0, 1, 1, capture);
                Move.moveAttempt(moves, current_square, dir, -1, 0, 1, capture);
                Move.moveAttempt(moves, current_square, dir, 0, -1, 1, capture);
            }
            return moves;
        }
    }
}

