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

            if (layer == Layer.Upper)
            {
                // Plus-shaped moves
                Move.moveAttempt(moves, current_square, dir, 1, 0, 3, regular);
                Move.moveAttempt(moves, current_square, dir, -1, 0, 3, regular);
                Move.moveAttempt(moves, current_square, dir, 0, 1, 3, regular);
                Move.moveAttempt(moves, current_square, dir, 0, -1, 3, regular);

                // Move to either layer straight below
                Move.moveAttempt(moves, current_square, dir, 0, 0, 2, capture);
                if (!Move.IsBlocked(current_square, dir, 0, 0, 2, this.color))
                {
                    Move.moveAttempt(moves, current_square, dir, 0, 0, 2, regular);
                    Move.moveAttempt(moves, current_square, dir, 0, 0, 1, regular);
                    Move.moveAttempt(moves, current_square, dir, 0, 0, 1, capture);
                }
            }
            else if (layer == Layer.Lower)
            {
                // Plus-shaped moves
                Move.moveAttempt(moves, current_square, dir, 1, 0, 1, regular);
                Move.moveAttempt(moves, current_square, dir, -1, 0, 1, regular);
                Move.moveAttempt(moves, current_square, dir, 0, 1, 1, regular);
                Move.moveAttempt(moves, current_square, dir, 0, -1, 1, regular);

                // Move to either layer straight above
                Move.moveAttempt(moves, current_square, dir, 0, 0, 2, capture);
                if (!Move.IsBlocked(current_square, dir, 0, 0, 2, this.color))
                {
                    Move.moveAttempt(moves, current_square, dir, 0, 0, 2, regular);
                    Move.moveAttempt(moves, current_square, dir, 0, 0, 3, regular);
                    Move.moveAttempt(moves, current_square, dir, 0, 0, 3, capture);
                }
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
                    Move.moveAttempt(moves, current_square, dir, row_diff, col_diff, 2, capture);
                    if (Move.IsBlocked(current_square, dir, row_diff, col_diff, 2, this.color))
                        break;
                    Move.moveAttempt(moves, current_square, dir, row_diff, col_diff, 2, regular);
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
                    Move.moveAttempt(moves, current_square, dir, row_diff, col_diff, 2, capture);
                    if (Move.IsBlocked(current_square, dir, row_diff, col_diff, 2, this.color))
                        break;
                    Move.moveAttempt(moves, current_square, dir, row_diff, col_diff, 2, regular);
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
                    Move.moveAttempt(moves, current_square, dir, row_diff, col_diff, 2, capture);
                    if (Move.IsBlocked(current_square, dir, row_diff, col_diff, 2, this.color))
                        break;
                    Move.moveAttempt(moves, current_square, dir, row_diff, col_diff, 2, regular);
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
                    Move.moveAttempt(moves, current_square, dir, row_diff, col_diff, 2, capture);
                    if (Move.IsBlocked(current_square, dir, row_diff, col_diff, 2, this.color))
                        break;
                    Move.moveAttempt(moves, current_square, dir, row_diff, col_diff, 2, regular);
                    r--;
                    c--;
                }
                // Orthogonal movements ---------------------------------------
                for (r = current_square.row + 1; r < Board.height; r++)
                {
                    int row_diff = r - current_square.row;
                    Move.moveAttempt(moves, current_square, dir, row_diff, 0, 2, capture);
                    if (Move.IsBlocked(current_square, dir, row_diff, 0, 2, this.color))
                        break;
                    Move.moveAttempt(moves, current_square, dir, row_diff, 0, 2, regular);
                }
                // Backward moves
                for (r = current_square.row - 1; r >= 0; r--)
                {
                    int row_diff = r - current_square.row;
                    Move.moveAttempt(moves, current_square, dir, row_diff, 0, 2, capture);
                    if (Move.IsBlocked(current_square, dir, row_diff, 0, 2, this.color))
                        break;
                    Move.moveAttempt(moves, current_square, dir, row_diff, 0, 2, regular);
                }
                // Right moves
                for (c = current_square.col + 1; c < Board.width; c++)
                {
                    int col_diff = c - current_square.col;
                    Move.moveAttempt(moves, current_square, dir, 0, col_diff, 2, capture);
                    if (Move.IsBlocked(current_square, dir, 0, col_diff, 2, this.color))
                        break;
                    Move.moveAttempt(moves, current_square, dir, 0, col_diff, 2, regular);
                }
                // Left moves
                for (c = current_square.col - 1; c >= 0; c--)
                {
                    int col_diff = c - current_square.col;
                    Move.moveAttempt(moves, current_square, dir, 0, col_diff, 2, capture);
                    if (Move.IsBlocked(current_square, dir, 0, col_diff, 2, this.color))
                        break;
                    Move.moveAttempt(moves, current_square, dir, 0, col_diff, 2, regular);
                }
            }
            return moves;
        }
    }
}

