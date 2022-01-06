using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    /* ----------- Dwarf ------------
     * 
     */
    public class Dwarf : Piece
    {
        public Dwarf() : base(PieceType.Dwarf) { nameChar = "D"; value = 2; }
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

            int layer_num;
            if (layer == Layer.Upper)
                layer_num = 3;
            else if (layer == Layer.Middle)
                layer_num = 2;
            else
                layer_num = 1;

            // Can move one step straight forward or sideways (layers 1 and 2)
            // Can CAPTURE one step diagonally forward (layers 1 and 2)
            if (layer == Layer.Lower || layer == Layer.Middle)
            {
                Move.moveAttempt(moves, current_square, dir, 1, 0, layer_num, regular);
                Move.moveAttempt(moves, current_square, dir, 0, 1, layer_num, regular);
                Move.moveAttempt(moves, current_square, dir, 0, -1, layer_num, regular);

                Move.moveAttempt(moves, current_square, dir, 1, 1, layer_num, capture);
                Move.moveAttempt(moves, current_square, dir, 1, -1, layer_num, capture);
            }
            // Level 1: - can CAPTURE on the square directly above on level 2.
            if (layer == Layer.Lower)
            {
                Move.moveAttempt(moves, current_square, dir, 0, 0, 2, capture);
            }
            // Level 2: - can MOVE TO the square directly below on level 1
            if (layer == Layer.Middle)
            {
                Move.moveAttempt(moves, current_square, dir, 0, 0, 1, regular);
            }
            return moves;
        }
    }
}

