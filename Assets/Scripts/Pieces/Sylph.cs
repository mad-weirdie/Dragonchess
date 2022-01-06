using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    /* ----------- Sylph ------------
         * On level 3:
         *  i)  can move one step diagonally forward or capture one step
         *      straight forward
         *  ii) can capture on the square directly below the sylph (on level 2)
         *  
         *  On level 2:
         *  i)  can move to the square directly above (on level 3) or to one of the player's
         *      six Sylph starting squares
         */
    public class Sylph : Piece
    {
        public Sylph() : base(PieceType.Sylph) { nameChar = "S"; value = 1; }

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

            // Level 3 moves
            if (layer == Layer.Upper)
            {
                // Move only: left forward diagonal
                Move.moveAttempt(moves, current_square, dir, 1, -1, 3, regular);
                // Move only: right forward diagonal
                Move.moveAttempt(moves, current_square, dir, 1, 1, 3, regular);
                // Capture: one forward
                Move.moveAttempt(moves, current_square, dir, 1, 0, 3, capture);
                // Capture: one down
                Move.moveAttempt(moves, current_square, dir, 0, 0, 2, capture);
            }
            else if (layer == Layer.Middle)
            {
                // Move directly up
                Move.moveAttempt(moves, current_square, dir, 0, 0, 3, regular);

                // Move to any of the starting Sylph locations
                for (int i = 0; i < Board.width; i += 2)
                {
                    int col_diff = i - current_square.col;
                    int row_diff;
                    if (this.color == Color.White)
                        row_diff = 1 - current_square.row;
                    else
                        row_diff = 7 - current_square.row;
                    Move.moveAttempt(moves, current_square, 1, row_diff, col_diff, 3, regular);
                }
            }

            return moves;
        }
    }
}

