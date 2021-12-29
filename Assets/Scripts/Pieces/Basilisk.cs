using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    /* ----------- Basilisk ------------*/
    public class Basilisk : Piece
    {
        public Square danger_square;
        public Piece frozen;
        public Basilisk() : base(PieceType.Basilisk) { }
        public Material frozen_mat;

        public void FreezeSquare(Square s)
        {
            if (s.occupied)
            {
                GameObject p = s.piece.pieceGameObject;
                if (p.GetComponent<Piece>().color != this.color)
                    p.GetComponent<Renderer>().material = frozen_mat;
            }
        }

        public override void MoveTo(Square s)
        {
            // Set new position of the piece's GameObject on the board
            Vector3 pos = s.cubeObject.transform.position;
            pos.y += 1.0f / Board.square_scale;
            this.pieceGameObject.transform.position = pos;
            GameObject sObj = this.location.cubeObject;
            sObj.GetComponent<Renderer>().material = this.location.properMaterial;

            // Link piece and square to each other
            print(this.location.row + " " + this.location.col);
            this.location.occupied = false;
            this.location = s;
            this.location.occupied = true;
            s.piece = this;

            // Set proper rendering layer (for the overhead cameras)
            this.pieceGameObject.layer = s.board.m_layer + 3;
            foreach (Transform child in this.transform)
                child.gameObject.layer = s.board.m_layer + 3;

            danger_square = GameController.getMiddleBoard().squares[s.row, s.col];
            FreezeSquare(danger_square);
        }

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

            /* Basilisk moves
             * - can move and capture one step diagonally forward or straight
             *   forward on level 1, or move one step straight backward
             * 
             * - automatically freezes(immobilizes) an enemy piece on the
             *   squaredirectly above on level 2, whether the Basilisk moves
             *   to the space below or the enemy moves to the space above, and
             *   until the Basilisk moves away or is captured.
             */

            // One step forward (move or capture)
            Move.moveAttempt(moves, current_square, dir, 1, 0, 1, regular);
            Move.moveAttempt(moves, current_square, dir, 1, 0, 1, capture);

            // One step backwards (move only)
            Move.moveAttempt(moves, current_square, dir, -1, 0, 1, regular);

            // Right forward diagonal (move or capture)
            Move.moveAttempt(moves, current_square, dir, 1, 1, 1, regular);
            Move.moveAttempt(moves, current_square, dir, 1, 1, 1, capture);

            // Left forward diagonal (move or capture)
            Move.moveAttempt(moves, current_square, dir, 1, -1, 1, regular);
            Move.moveAttempt(moves, current_square, dir, 1, -1, 1, capture);

            return moves;
        }
    }
}

