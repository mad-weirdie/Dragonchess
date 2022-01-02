using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    /* ----------- Warrior ------------
     * 
     */
    public class Warrior : Piece
    {
        public Warrior() : base(PieceType.Warrior) { nameChar = "W"; }

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

            // Level 2 moves
            if (layer == Layer.Middle)
            {
                // Move only: forward
                Move.moveAttempt(moves, current_square, dir, 1, 0, 2, regular);
                // Capture only: forward-left
                Move.moveAttempt(moves, current_square, dir, 1, -1, 2, capture);
                // Capture only: forward-right
                Move.moveAttempt(moves, current_square, dir, 1, 1, 2, capture);
            }
            return moves;
        }
    }
}

