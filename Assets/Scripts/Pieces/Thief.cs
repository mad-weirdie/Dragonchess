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

        public override List<Move> GetMoves()
        {
            List<Move> moves = new List<Move>();
            Square current_square = this.pos;
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

            return moves;
        }
    }
}

