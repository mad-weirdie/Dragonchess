using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{

    /* ----------- Griffon ------------
     * On level 3:
     *  i)  can move and capture by jumping in the following pattern:
     *      two steps diagonally followed by one step orthogonally outwards
     *  ii) can move and capture one step triagonally to level 2
     *  
     *  On level 2:
     *  i)  can move and capture one step diagonally
     *  ii) can move and capture one step triagonally to level 3
     */
    public class Griffon : Piece
    {
        public Griffon() : base(PieceType.Griffon) { nameChar = "G"; value = 20; }
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
                // ------------------------------------------------------------
                // Move or capture: two steps diagonally + one step OUTWARDS
                // ------------------------------------------------------------
                // diag forward-right x2, then forward
                Move.moveAttempt(moves, current_square, dir, 3, 2, 3, regular);
                Move.moveAttempt(moves, current_square, dir, 3, 2, 3, capture);

                // diag forward-right x2, then right
                Move.moveAttempt(moves, current_square, dir, 2, 3, 3, regular);
                Move.moveAttempt(moves, current_square, dir, 2, 3, 3, capture);

                // diag forward-left x2, then forward
                Move.moveAttempt(moves, current_square, dir, 3, -2, 3, regular);
                Move.moveAttempt(moves, current_square, dir, 3, -2, 3, capture);

                // diag forward-left x2, then left
                Move.moveAttempt(moves, current_square, dir, 2, -3, 3, regular);
                Move.moveAttempt(moves, current_square, dir, 2, -3, 3, capture);

                // diag backward-right x2, then backward
                Move.moveAttempt(moves, current_square, dir, -3, 2, 3, regular);
                Move.moveAttempt(moves, current_square, dir, -3, 2, 3, capture);

                // diag backward-right x2, then right
                Move.moveAttempt(moves, current_square, dir, -2, 3, 3, regular);
                Move.moveAttempt(moves, current_square, dir, -2, 3, 3, capture);

                // diag backward-left x2, then backward
                Move.moveAttempt(moves, current_square, dir, -3, -2, 3, regular);
                Move.moveAttempt(moves, current_square, dir, -3, -2, 3, capture);

                // diag backward-left x2, then left
                Move.moveAttempt(moves, current_square, dir, -2, -3, 3, regular);
                Move.moveAttempt(moves, current_square, dir, -2, -3, 3, capture);

                // ------------------------------------------------------------
                // Move or capture one square triagonally
                // ------------------------------------------------------------
                // triag forward-right
                Move.moveAttempt(moves, current_square, dir, 1, 1, 2, regular);
                Move.moveAttempt(moves, current_square, dir, 1, 1, 2, capture);

                // triag forward-left
                Move.moveAttempt(moves, current_square, dir, 1, -1, 2, regular);
                Move.moveAttempt(moves, current_square, dir, 1, -1, 2, capture);

                // triag backward-right
                Move.moveAttempt(moves, current_square, dir, -1, 1, 2, regular);
                Move.moveAttempt(moves, current_square, dir, -1, 1, 2, capture);

                // triag backward-left
                Move.moveAttempt(moves, current_square, dir, -1, -1, 2, regular);
                Move.moveAttempt(moves, current_square, dir, -1, -1, 2, capture);

            }
            // Level 2 moves
            else if (layer == Layer.Middle)
            {
                // can move and capture one step diagonally;
                // diag forward-right
                Move.moveAttempt(moves, current_square, dir, 1, 1, 2, regular);
                Move.moveAttempt(moves, current_square, dir, 1, 1, 2, capture);

                // diag forward-left
                Move.moveAttempt(moves, current_square, dir, 1, -1, 2, regular);
                Move.moveAttempt(moves, current_square, dir, 1, -1, 2, capture);

                // diag backward-right
                Move.moveAttempt(moves, current_square, dir, -1, 1, 2, regular);
                Move.moveAttempt(moves, current_square, dir, -1, 1, 2, capture);

                // diag backward-left
                Move.moveAttempt(moves, current_square, dir, -1, -1, 2, regular);
                Move.moveAttempt(moves, current_square, dir, -1, -1, 2, capture);

                // can move and capture one step triagonally to level 3.
                // triag forward-right
                Move.moveAttempt(moves, current_square, dir, 1, 1, 3, regular);
                Move.moveAttempt(moves, current_square, dir, 1, 1, 3, capture);

                // triag forward-left
                Move.moveAttempt(moves, current_square, dir, 1, -1, 3, regular);
                Move.moveAttempt(moves, current_square, dir, 1, -1, 3, capture);

                // triag backward-right
                Move.moveAttempt(moves, current_square, dir, -1, 1, 3, regular);
                Move.moveAttempt(moves, current_square, dir, -1, 1, 3, capture);

                // triag backward-left
                Move.moveAttempt(moves, current_square, dir, -1, -1, 3, regular);
                Move.moveAttempt(moves, current_square, dir, -1, -1, 3, capture);
            }

            return moves;
        }
    }
}

