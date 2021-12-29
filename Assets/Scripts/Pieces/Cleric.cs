using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    /* ----------- Cleric ------------
     * 
     */
    public class Cleric : Piece
    {
        public Cleric() : base(PieceType.Cleric) { }
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

            int layer_num;
            if (layer == Layer.Upper)
                layer_num = 3;
            else if (layer == Layer.Middle)
                layer_num = 2;
            else
                layer_num = 1;

            // ANY LEVEL: King-esque moves
            Move.moveAttempt(moves, current_square, dir, 1, 0, layer_num, move_cap);
            Move.moveAttempt(moves, current_square, dir, 1, 1, layer_num, move_cap);
            Move.moveAttempt(moves, current_square, dir, 0, 1, layer_num, move_cap);
            Move.moveAttempt(moves, current_square, dir, -1, 1, layer_num, move_cap);
            Move.moveAttempt(moves, current_square, dir, -1, 0, layer_num, move_cap);
            Move.moveAttempt(moves, current_square, dir, -1, -1, layer_num, move_cap);
            Move.moveAttempt(moves, current_square, dir, 0, -1, layer_num, move_cap);
            Move.moveAttempt(moves, current_square, dir, 1, -1, layer_num, move_cap);

            if (layer == Layer.Middle)
            {
                // Move up
                Move.moveAttempt(moves, current_square, dir, 0, 0, 3, move_cap);
                // Move down
                Move.moveAttempt(moves, current_square, dir, 0, 0, 1, move_cap);
            }
            else
            {
                // Move back to middle layer
                Move.moveAttempt(moves, current_square, dir, 0, 0, 2, move_cap);
            }

            return moves;
        }
    }
}

