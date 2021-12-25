using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    /* ----------- Sylph ------------
         * On level 3:
         *  i)  can move one step diagonally forward or capture one step
         *      straight forward
         *  ii) can capture on the square directly below the sylph (on level 2)
         *  
         *  On level 2:
         *  i)  can move to the square directly above (on level 3) or to one of the player's
         *      six Sylph starting squares
         */
    public class Sylph : Piece
    {
        public Sylph() : base(PieceType.Sylph) { }

        public override ArrayList GetMoves()
        {
            ArrayList moves = new ArrayList();
            // Get current piece data
            Square current_square = this.location;
            Square end;
            Move next_move;
            print("row: " + current_square.row);
            print("col: " + current_square.col);
            Layer layer = current_square.layer;
            int direction;

            // Definition of "forward, left, right" changes based on piece color
            if (this.color == Color.White)
                direction = 1;
            else
                direction = -1;

            // Level 3 moves
            if (layer == Layer.Upper)
            {
                // Move only: left forward diagonal
                int new_row = current_square.row + direction;
                int new_col = current_square.col - direction;
                if (Square.IsValidSquare(new_row, new_col))
                {
                    end = board.squares[new_row, new_col];
                    next_move = new Move(location, end, Move.MoveType.Regular);
                    if (next_move.IsValidMove())
                    {
                        moves.Add(next_move);
                    }
                }

                // Move only: right forward diagonal
                new_row = current_square.row + direction;
                new_col = current_square.col + direction;
                if (Square.IsValidSquare(new_row, new_col))
                {
                    end = board.squares[new_row, new_col];
                    next_move = new Move(location, end, Move.MoveType.Regular);
                    if (next_move.IsValidMove())
                    {
                        moves.Add(next_move);
                    }
                }

                // Capture: one forward
                new_col = current_square.col;
                if (Square.IsValidSquare(new_row, new_col))
                {
                    end = board.squares[new_row, new_col];
                    next_move = new Move(location, end, Move.MoveType.Capture);
                    if (next_move.IsValidMove())
                    {
                        moves.Add(next_move);
                    }
                }

                // Capture: one down
                Board middleBoard = GameController.getMiddleBoard();
                end = middleBoard.squares[current_square.row, current_square.col];
                next_move = new Move(location, end, Move.MoveType.Capture);
                if (next_move.IsValidMove())
                {
                    moves.Add(next_move);
                }
            }

            else if (layer == Layer.Middle)
            {
                // Move directly up
                Board upperBoard = GameController.getUpperBoard();
                end = upperBoard.squares[current_square.row, current_square.col];
                next_move = new Move(location, end, Move.MoveType.Regular);
                moves.Add(next_move);

                // Move to any of the starting Sylph locations
                for (int i = 0; i < Board.width; i+=2)
                {
                    if (this.color == Color.White)
                        end = upperBoard.squares[1, i];
                    else
                        end = upperBoard.squares[7, i];
                    next_move = new Move(location, end, Move.MoveType.Regular);
                    moves.Add(next_move);

                }
            }

            return moves;
        }
    }
}

