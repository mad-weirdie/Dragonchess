using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    /* ----------- Hero ------------
     * 
     */
    public class Hero : Piece
    {
        public Hero() : base(PieceType.Hero) { }
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
                // Middle-board diagonal moves (1 or 2 diags)
                // diag forward-right
                Move.moveAttempt(moves, current_square, dir, 1, 1, 2, regular);
                Move.moveAttempt(moves, current_square, dir, 2, 2, 2, capture);
                Move.moveAttempt(moves, current_square, dir, 1, 1, 2, regular);
                Move.moveAttempt(moves, current_square, dir, 2, 2, 2, capture);

                // diag forward-left
                Move.moveAttempt(moves, current_square, dir, 1, -1, 2, regular);
                Move.moveAttempt(moves, current_square, dir, 2, -2, 2, capture);
                Move.moveAttempt(moves, current_square, dir, 1, -1, 2, regular);
                Move.moveAttempt(moves, current_square, dir, 2, -2, 2, capture);

                // diag backward-right 
                Move.moveAttempt(moves, current_square, dir, -1, 1, 2, regular);
                Move.moveAttempt(moves, current_square, dir, -2, 2, 2, capture);
                Move.moveAttempt(moves, current_square, dir, -1, 1, 2, regular);
                Move.moveAttempt(moves, current_square, dir, -2, 2, 2, capture);

                // diag backward-left
                Move.moveAttempt(moves, current_square, dir, -1, -1, 2, regular);
                Move.moveAttempt(moves, current_square, dir, -2, -2, 2, capture);
                Move.moveAttempt(moves, current_square, dir, -1, -1, 2, regular);
                Move.moveAttempt(moves, current_square, dir, -2, -2, 2, capture);

                // Inter-board triagonal moves
                // triag forward-right and down
                Move.moveAttempt(moves, current_square, dir, 1, 1, 1, regular);
                Move.moveAttempt(moves, current_square, dir, 1, 1, 1, capture);

                // triag forward-left and down 
                Move.moveAttempt(moves, current_square, dir, 1, -1, 1, regular);
                Move.moveAttempt(moves, current_square, dir, 1, -1, 1, capture);

                // triag backward-right and down 
                Move.moveAttempt(moves, current_square, dir, -1, 1, 1, regular);
                Move.moveAttempt(moves, current_square, dir, -1, 1, 1, capture);

                // triag backward-left and down
                Move.moveAttempt(moves, current_square, dir, -1, -1, 1, regular);
                Move.moveAttempt(moves, current_square, dir, -1, -1, 1, capture);

                // triag forward-right and up
                Move.moveAttempt(moves, current_square, dir, 1, 1, 3, regular);
                Move.moveAttempt(moves, current_square, dir, 1, 1, 3, capture);

                // triag forward-left and up 
                Move.moveAttempt(moves, current_square, dir, 1, -1, 3, regular);
                Move.moveAttempt(moves, current_square, dir, 1, -1, 3, capture);

                // triag backward-right and up 
                Move.moveAttempt(moves, current_square, dir, -1, 1, 3, regular);
                Move.moveAttempt(moves, current_square, dir, -1, 1, 3, capture);

                // triag backward-left and up
                Move.moveAttempt(moves, current_square, dir, -1, -1, 3, regular);
                Move.moveAttempt(moves, current_square, dir, -1, -1, 3, capture);
            }

            // Level 1 or 3 moves
            if (layer == Layer.Upper || layer == Layer.Lower)
            {
                // triag forward-right
                Move.moveAttempt(moves, current_square, dir, 1, 1, 2, regular);
                Move.moveAttempt(moves, current_square, dir, 1, 1, 2, capture);

                // triag forward-left
                Move.moveAttempt(moves, current_square, dir, 1, -1, 2, regular);
                Move.moveAttempt(moves, current_square, dir, 1, -1, 2, capture);

                // triag backward-right
                Move.moveAttempt(moves, current_square, dir, -1, 1, 2, regular);
                Move.moveAttempt(moves, current_square, dir, -1, 1, 2, capture);

                // triag backward-left
                Move.moveAttempt(moves, current_square, dir, -1, -1, 2, regular);
                Move.moveAttempt(moves, current_square, dir, -1, -1, 2, capture);
            }
            return moves;
        }
    }
}

