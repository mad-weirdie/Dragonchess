using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    /* ----------- Mage ------------
     * 
     */
    public class Mage : Piece
    {
        public Mage() : base(PieceType.Mage) { }
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
                dir = 1;

            if (layer == Layer.Upper)
            {
                // Move to either layer straight below
                Move.moveAttempt(moves, current_square, dir, 0, 0, 2, regular);
                Move.moveAttempt(moves, current_square, dir, 0, 0, 1, regular);

                Move.moveAttempt(moves, current_square, dir, 0, 0, 2, capture);
                Move.moveAttempt(moves, current_square, dir, 0, 0, 1, capture);

                // Plus-shaped moves
                Move.moveAttempt(moves, current_square, dir, 1, 0, 3, regular);
                Move.moveAttempt(moves, current_square, dir, -1, 0, 3, regular);
                Move.moveAttempt(moves, current_square, dir, 0, 1, 3, regular);
                Move.moveAttempt(moves, current_square, dir, 0, -1, 3, regular);
            }
            else if (layer == Layer.Lower)
            {
                // Move to either layer straight above
                Move.moveAttempt(moves, current_square, dir, 0, 0, 2, regular);
                Move.moveAttempt(moves, current_square, dir, 0, 0, 3, regular);
                Move.moveAttempt(moves, current_square, dir, 0, 0, 2, capture);
                Move.moveAttempt(moves, current_square, dir, 0, 0, 3, capture);

                // Plus-shaped moves
                Move.moveAttempt(moves, current_square, dir, 1, 0, 1, regular);
                Move.moveAttempt(moves, current_square, dir, -1, 0, 1, regular);
                Move.moveAttempt(moves, current_square, dir, 0, 1, 1, regular);
                Move.moveAttempt(moves, current_square, dir, 0, -1, 1, regular);
            }
            // Level 2 moves
            else
            {
                // Straight up or down
                Move.moveAttempt(moves, current_square, dir, 0, 0, 1, regular);
                Move.moveAttempt(moves, current_square, dir, 0, 0, 3, regular);
                Move.moveAttempt(moves, current_square, dir, 0, 0, 1, capture);
                Move.moveAttempt(moves, current_square, dir, 0, 0, 3, capture);

                // Diagonal movements -----------------------------------------
                // Forward-right diags
                int r = current_square.row + 1;
                int c = current_square.col + 1;
                while (r < Board.height && c < Board.width)
                {
                    int row_diff = r - current_square.row;
                    int col_diff = c - current_square.col;
                    Move.moveAttempt(moves, current_square, dir, row_diff, col_diff, 2, regular);
                    Move.moveAttempt(moves, current_square, dir, row_diff, col_diff, 2, capture);
                    r++;
                    c++;
                }

                // Forward-left diags
                r = current_square.row + 1;
                c = current_square.col - 1;
                while (r < Board.height && c >= 0)
                {
                    int row_diff = r - current_square.row;
                    int col_diff = c - current_square.col;
                    Move.moveAttempt(moves, current_square, dir, row_diff, col_diff, 2, regular);
                    Move.moveAttempt(moves, current_square, dir, row_diff, col_diff, 2, capture);
                    r++;
                    c--;
                }

                // Backward-right diags
                r = current_square.row - 1;
                c = current_square.col + 1;
                while (r >= 0 && c < Board.width)
                {
                    int row_diff = r - current_square.row;
                    int col_diff = c - current_square.col;
                    Move.moveAttempt(moves, current_square, dir, row_diff, col_diff, 2, regular);
                    Move.moveAttempt(moves, current_square, dir, row_diff, col_diff, 2, capture);
                    r--;
                    c++;
                }
                // backward-left diags
                r = current_square.row - 1;
                c = current_square.col - 1;
                while (r >= 0 && c >= 0)
                {
                    int row_diff = r - current_square.row;
                    int col_diff = c - current_square.col;
                    Move.moveAttempt(moves, current_square, dir, row_diff, col_diff, 2, regular);
                    Move.moveAttempt(moves, current_square, dir, row_diff, col_diff, 2, capture);
                    r--;
                    c--;
                }
                // Orthogonal movements ---------------------------------------
                // Forward moves
                for (r = current_square.row; r < Board.height; r++)
                {
                    int row_diff = r - current_square.row;
                    Move.moveAttempt(moves, current_square, dir, row_diff, 0, 2, regular);
                    Move.moveAttempt(moves, current_square, dir, row_diff, 0, 2, capture);
                }
                // Backward moves
                for (r = current_square.row; r >= 0; r--)
                {
                    int row_diff = r - current_square.row;
                    Move.moveAttempt(moves, current_square, dir, row_diff, 0, 2, regular);
                    Move.moveAttempt(moves, current_square, dir, row_diff, 0, 2, capture);
                }
                // Right moves
                for (c = current_square.col; c < Board.width; c++)
                {
                    int col_diff = c - current_square.col;
                    Move.moveAttempt(moves, current_square, dir, 0, col_diff, 2, regular);
                    Move.moveAttempt(moves, current_square, dir, 0, col_diff, 2, capture);
                }
                // Left moves
                for (c = current_square.col; c >= 0; c--)
                {
                    int col_diff = c - current_square.col;
                    Move.moveAttempt(moves, current_square, dir, 0, col_diff, 2, regular);
                    Move.moveAttempt(moves, current_square, dir, 0, col_diff, 2, capture);
                }
            }
            return moves;
        }
    }
}

