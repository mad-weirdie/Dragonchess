using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    /* ----------- Paladin ------------
     * 
     */
    public class Paladin : Piece
    {
        public Paladin() : base(PieceType.Paladin) { }
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


            // Any board - move_cap King moves
            Move.moveAttempt(moves, current_square, dir, 1, 0, layer_num, move_cap);
            Move.moveAttempt(moves, current_square, dir, 1, 1, layer_num, move_cap);
            Move.moveAttempt(moves, current_square, dir, 0, 1, layer_num, move_cap);
            Move.moveAttempt(moves, current_square, dir, -1, 1, layer_num, move_cap);
            Move.moveAttempt(moves, current_square, dir, -1, 0, layer_num, move_cap);
            Move.moveAttempt(moves, current_square, dir, -1, -1, layer_num, move_cap);
            Move.moveAttempt(moves, current_square, dir, 0, -1, layer_num, move_cap);
            Move.moveAttempt(moves, current_square, dir, 1, -1, layer_num, move_cap);

            // Between-board knight-esque moves
            int[] layer_shift = { 1, 2, -1, -2 };
            foreach (int shift in layer_shift)
            {
                int new_layer = layer_num - shift;
                if ((new_layer <= 3) && (new_layer >= 1))
                {
                    int lat_shift = 3 - Math.Abs(shift);
                    // forward
                    Move.moveAttempt(moves, current_square, dir, lat_shift, 0, new_layer, regular);
                    // backward
                    Move.moveAttempt(moves, current_square, dir, -lat_shift, 0, new_layer, regular);
                    // left
                    Move.moveAttempt(moves, current_square, dir, 0, lat_shift, new_layer, regular);
                    // right
                    Move.moveAttempt(moves, current_square, dir, 0, -lat_shift, new_layer, regular);
                }
            }
            // Level 2 moves
            if (layer == Layer.Middle)
            {
                // Knight-esque moves -----------------------------------------
                // forward-right L
                Move.moveAttempt(moves, current_square, dir, 2, 1, 2, move_cap);
                // right-forward L
                Move.moveAttempt(moves, current_square, dir, 1, 2, 2, move_cap);
                // forward-left L
                Move.moveAttempt(moves, current_square, dir, 2, -1, 2, move_cap);
                // left-forward L
                Move.moveAttempt(moves, current_square, dir, 1, -2, 2, move_cap);
                // backward-right L
                Move.moveAttempt(moves, current_square, dir, -2, 1, 2, move_cap);
                // right-backward L
                Move.moveAttempt(moves, current_square, dir, -1, 2, 2, move_cap);
                // backward-left L
                Move.moveAttempt(moves, current_square, dir, -2, -1, 2, move_cap);
                // left-backward L
                Move.moveAttempt(moves, current_square, dir, -1, -2, 2, move_cap);
            }
            return moves;
        }
    }
}

