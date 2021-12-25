using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    /* ----------- Oliphant ------------
     * Moves like a chess rook. Stays on level 2.
     */
    public class Oliphant : Piece
    {
        public Oliphant() : base(PieceType.Oliphant) { }
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

            // Forward moves
            for (int r = current_square.row; r < Board.height; r++)
            {
                int row_diff = r - current_square.row;
                Move.moveAttempt(moves, current_square, dir, row_diff, 0, 2, regular);
            }

            // Backward moves
            for (int r = current_square.row; r >= 0; r--)
            {
                int row_diff = r - current_square.row;
                Move.moveAttempt(moves, current_square, dir, row_diff, 0, 2, regular);
            }

            // Right moves
            for (int c = current_square.col; c < Board.width; c++)
            {
                int col_diff = c - current_square.col;
                Move.moveAttempt(moves, current_square, dir, 0, col_diff, 2, regular);
            }

            // Left moves
            for (int c = current_square.col; c >= 0; c--)
            {
                int col_diff = c - current_square.col;
                Move.moveAttempt(moves, current_square, dir, 0, col_diff, 2, regular);
            }

            return moves;
        }

        void Start()
        {

        }
    }
}

