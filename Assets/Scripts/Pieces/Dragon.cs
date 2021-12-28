using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    /* ----------- Dragon ------------*/
    public class Dragon : Piece
    {
        public Dragon() : base(PieceType.Dragon) { }
        public override ArrayList GetMoves()
        {
            print("calling dragon moves");
            ArrayList moves = new ArrayList();
            Square current_square = this.location;
            Layer layer = current_square.layer;
            int dir;

            // Definition of "forward, left, right" changes based on piece color
            if (this.color == Color.White)
                dir = 1;
            else
                dir = 1;

            // Level 3 moves
            if (layer == Layer.Upper)
            {
                // King-esque moves -------------------------------------------
                Move.moveAttempt(moves, current_square, dir, 1, 0, 3, regular);
                Move.moveAttempt(moves, current_square, dir, 1, 1, 3, regular);
                Move.moveAttempt(moves, current_square, dir, 0, 1, 3, regular);
                Move.moveAttempt(moves, current_square, dir, -1, 1, 3, regular);
                Move.moveAttempt(moves, current_square, dir, -1, 0, 3, regular);
                Move.moveAttempt(moves, current_square, dir, -1, -1, 3, regular);
                Move.moveAttempt(moves, current_square, dir, 0, -1, 3, regular);
                Move.moveAttempt(moves, current_square, dir, 1, -1, 3, regular);

                Move.moveAttempt(moves, current_square, dir, 1, 0, 3, capture);
                Move.moveAttempt(moves, current_square, dir, 1, 1, 3, capture);
                Move.moveAttempt(moves, current_square, dir, 0, 1, 3, capture);
                Move.moveAttempt(moves, current_square, dir, -1, 1, 3, capture);
                Move.moveAttempt(moves, current_square, dir, -1, 0, 3, capture);
                Move.moveAttempt(moves, current_square, dir, -1, -1, 3, capture);
                Move.moveAttempt(moves, current_square, dir, 0, -1, 3, capture);
                Move.moveAttempt(moves, current_square, dir, 1, -1, 3, capture);

                // Bisop-esque moves ------------------------------------------

                // Forward-right diags
                int r = current_square.row + 1;
                int c = current_square.col + 1;
                while (r < Board.height && c < Board.width)
                {
                    int row_diff = r - current_square.row;
                    int col_diff = c - current_square.col;
                    Move.moveAttempt(moves, current_square, dir, row_diff, col_diff, 3, regular);
                    Move.moveAttempt(moves, current_square, dir, row_diff, col_diff, 3, capture);
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
                    Move.moveAttempt(moves, current_square, dir, row_diff, col_diff, 3, regular);
                    Move.moveAttempt(moves, current_square, dir, row_diff, col_diff, 3, capture);
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
                    Move.moveAttempt(moves, current_square, dir, row_diff, col_diff, 3, regular);
                    Move.moveAttempt(moves, current_square, dir, row_diff, col_diff, 3, capture);
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
                    Move.moveAttempt(moves, current_square, dir, row_diff, col_diff, 3, regular);
                    Move.moveAttempt(moves, current_square, dir, row_diff, col_diff, 3, capture);
                    r--;
                    c--;
                }

                // Remote capturing moves
                Move.moveAttempt(moves, current_square, dir, 0, 0, 2, swoop);
                Move.moveAttempt(moves, current_square, dir, 1, 0, 2, swoop);
                Move.moveAttempt(moves, current_square, dir, -1, 0, 2, swoop);
                Move.moveAttempt(moves, current_square, dir, 0, 1, 2, swoop);
                Move.moveAttempt(moves, current_square, dir, 0, -1, 2, swoop);
            }
            return moves;
        }
    }
}

