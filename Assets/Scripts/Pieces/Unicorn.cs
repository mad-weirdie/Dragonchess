using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    /* ----------- Unicorn ------------
     * Moves and captures like a chess Knight - level 2 only
     */
    public class Unicorn : Piece
    {
        public Unicorn() : base(PieceType.Unicorn) { nameChar = "U"; value = 3; }

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
            
            // forward-right L
            Move.moveAttempt(moves, current_square, dir, 2, 1, 2, regular);
            Move.moveAttempt(moves, current_square, dir, 2, 1, 2, capture);
            // right-forward L
            Move.moveAttempt(moves, current_square, dir, 1, 2, 2, regular);
            Move.moveAttempt(moves, current_square, dir, 1, 2, 2, capture);
            // forward-left L
            Move.moveAttempt(moves, current_square, dir, 2, -1, 2, regular);
            Move.moveAttempt(moves, current_square, dir, 2, -1, 2, capture);
            // left-forward L
            Move.moveAttempt(moves, current_square, dir, 1, -2, 2, regular);
            Move.moveAttempt(moves, current_square, dir, 1, -2, 2, capture);
            // backward-right L
            Move.moveAttempt(moves, current_square, dir, -2, 1, 2, regular);
            Move.moveAttempt(moves, current_square, dir, -2, 1, 2, capture);
            // right-backward L
            Move.moveAttempt(moves, current_square, dir, -1, 2, 2, regular);
            Move.moveAttempt(moves, current_square, dir, -1, 2, 2, capture);
            // backward-left L
            Move.moveAttempt(moves, current_square, dir, -2, -1, 2, regular);
            Move.moveAttempt(moves, current_square, dir, -2, -1, 2, capture);
            // left-backward L
            Move.moveAttempt(moves, current_square, dir, -1, -2, 2, regular);
            Move.moveAttempt(moves, current_square, dir, -1, -2, 2, capture);

            return moves;
        }
    }
}

