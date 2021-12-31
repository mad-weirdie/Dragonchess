using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    /* ----------- Basilisk ------------*/
    public class Basilisk : Piece
    {
        public Square danger_square;
        public Basilisk() : base(PieceType.Basilisk) { }
        public Material frozen_mat;
        Material original_mat;

        public void FreezeSquare(Square s)
        {
            if (s.occupied)
            {
                GameObject p = s.piece.pieceGameObject;
                if (p.GetComponent<Piece>().color != this.color)
                {
                    p.GetComponent<Piece>().frozen = true;
                    original_mat = p.GetComponent<Renderer>().material;
                    p.GetComponent<Renderer>().material = frozen_mat;
                }
            }
        }

        public void UnfreezeSquare(Square s)
        {
            if (s.occupied)
            {
                GameObject p = s.piece.pieceGameObject;
                if (p.GetComponent<Piece>().color != this.color)
                {
                    p.GetComponent<Piece>().frozen = false;
                    p.GetComponent<Renderer>().material = original_mat;
                }
            }
        }

        public override void MoveTo(Square s)
        {
            // Set new position of the piece's GameObject on the board
            Vector3 pos = s.cubeObject.transform.position;
            pos.y += 1.0f / Board.square_scale;
            this.pieceGameObject.transform.position = pos;
            GameObject sObj = this.pos.cubeObject;
            sObj.GetComponent<Renderer>().material = this.pos.properMaterial;

            // Link piece and square to each other
            this.pos.occupied = false;
            this.pos = s;
            this.pos.occupied = true;
            s.piece = this;

            // Set proper rendering layer (for the overhead cameras)
            this.pieceGameObject.layer = s.board.m_layer + 3;
            foreach (Transform child in this.transform)
                child.gameObject.layer = s.board.m_layer + 3;

            UnfreezeSquare(danger_square);
            danger_square = GameController.getMiddleBoard().squares[s.row, s.col];
            FreezeSquare(danger_square);
        }

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

