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
        public Hero() : base(PieceType.Hero) { nameChar = "H"; value = 10; }
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
                dir = 1;

            // Level 2 moves
            if (layer == Layer.Middle)
            {
                // Middle-board diagonal moves (1 or 2 diags)
                // diag forward-right
                Move.moveAttempt(moves, current_square, dir, 1, 1, 2, move_cap);
                Move.moveAttempt(moves, current_square, dir, 2, 2, 2, move_cap);

                // diag forward-left
                Move.moveAttempt(moves, current_square, dir, 1, -1, 2, move_cap);
                Move.moveAttempt(moves, current_square, dir, 2, -2, 2, move_cap);

                // diag backward-right 
                Move.moveAttempt(moves, current_square, dir, -1, 1, 2, move_cap);
                Move.moveAttempt(moves, current_square, dir, -2, 2, 2, move_cap);

                // diag backward-left
                Move.moveAttempt(moves, current_square, dir, -1, -1, 2, move_cap);
                Move.moveAttempt(moves, current_square, dir, -2, -2, 2, move_cap);

                // Inter-board triagonal moves
                // triag forward-right and down
                Move.moveAttempt(moves, current_square, dir, 1, 1, 1, move_cap);
                // triag forward-left and down 
                Move.moveAttempt(moves, current_square, dir, 1, -1, 1, move_cap);
                // triag backward-right and down 
                Move.moveAttempt(moves, current_square, dir, -1, 1, 1, move_cap);
                // triag backward-left and down
                Move.moveAttempt(moves, current_square, dir, -1, -1, 1, move_cap);

                // triag forward-right and up
                Move.moveAttempt(moves, current_square, dir, 1, 1, 3, move_cap);
                // triag forward-left and up 
                Move.moveAttempt(moves, current_square, dir, 1, -1, 3, move_cap);
                // triag backward-right and up 
                Move.moveAttempt(moves, current_square, dir, -1, 1, 3, move_cap);
                // triag backward-left and up
                Move.moveAttempt(moves, current_square, dir, -1, -1, 3, move_cap);
            }

            // Level 1 or 3 moves
            if (layer == Layer.Upper || layer == Layer.Lower)
            {
                // triag forward-right
                Move.moveAttempt(moves, current_square, dir, 1, 1, 2, move_cap);
                // triag forward-left
                Move.moveAttempt(moves, current_square, dir, 1, -1, 2, move_cap);
                // triag backward-right
                Move.moveAttempt(moves, current_square, dir, -1, 1, 2, move_cap);
                // triag backward-left
                Move.moveAttempt(moves, current_square, dir, -1, -1, 2, move_cap);
            }
            return moves;
        }
    }
}

