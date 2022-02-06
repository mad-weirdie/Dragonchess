using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
	using static Move;
	using static MoveDict;
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
        public Sylph() : base(PieceType.Sylph) { nameChar = "S"; value = 1; }

        public override List<Move> GetMoves(Game state)
        {
			List<Move> moves = new List<Move>();
			List<(int, int, int)> dictMoves;
			Square current = this.pos;

			if (this.color == Color.White && current.board == 3)
			{
				dictMoves = MoveDictionary["TopWSylph"][current.board, current.row, current.col];
				AddMoves(state, dictMoves, moves, current, regular);
				dictMoves = MoveDictionary["TopWSylphTake"][current.board, current.row, current.col];
				AddMoves(state, dictMoves, moves, current, capture);
			}
			else if (this.color == Color.Black && current.board == 3)
			{
				dictMoves = MoveDictionary["TopBSylph"][current.board, current.row, current.col];
				AddMoves(state, dictMoves, moves, current, regular);
				dictMoves = MoveDictionary["TopBSylphTake"][current.board, current.row, current.col];
				AddMoves(state, dictMoves, moves, current, capture);
			}
			else if (this.color == Color.White && current.board == 2)
			{
				dictMoves = MoveDictionary["MidWSylph"][current.board, current.row, current.col];
				AddMoves(state, dictMoves, moves, current, regular);
			}
			else if (this.color == Color.Black && current.board == 2)
			{
				dictMoves = MoveDictionary["MidBSylph"][current.board, current.row, current.col];
				AddMoves(state, dictMoves, moves, current, regular);
			}

			return moves;
		}
    }
}

