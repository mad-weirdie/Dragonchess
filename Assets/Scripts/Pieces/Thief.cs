using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    /* ----------- Thief ------------
     * 
     */
    public class Thief : Piece
    {
        public Thief() : base(PieceType.Thief) { }

        public override ArrayList GetMoves()
        {
            ArrayList moves = new ArrayList();
            Square current_square = this.location;
            Layer layer = current_square.layer;
            int dir = 1;

            // Bisop-esque moves ------------(CANNOT JUMP) --------------------

            // Forward-right diags
            int r = current_square.row + 1;
            int c = current_square.col + 1;
            while (r < Board.height && c < Board.width)
            {
                int row_diff = r - current_square.row;
                int col_diff = c - current_square.col;
                if (Move.IsBlocked(current_square, dir, row_diff, col_diff, 2))
                    break;
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
                if (Move.IsBlocked(current_square, dir, row_diff, col_diff, 2))
                    break;
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
                if (Move.IsBlocked(current_square, dir, row_diff, col_diff, 2))
                    break;
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
                if (Move.IsBlocked(current_square, dir, row_diff, col_diff, 2))
                    break;
                Move.moveAttempt(moves, current_square, dir, row_diff, col_diff, 2, regular);
                Move.moveAttempt(moves, current_square, dir, row_diff, col_diff, 2, capture);
                r--;
                c--;
            }

            return moves;
        }
    }
}

