using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
	using static Move;
	using static MoveDict;
	/* ----------- Unicorn ------------
     * Moves and captures like a chess Knight - level 2 only
     */
	public class Unicorn : Piece
    {
        public Unicorn() : base(PieceType.Unicorn) { nameChar = "U"; value = 3; }

        public override List<Move> GetMoves(Gamestate state)
        {
			List<Move> moves = new List<Move>();
			List<(int, int, int)> dictMoves;
			Square current = this.pos;

			dictMoves = MoveDictionary["Unicorn"][current.board, current.row, current.col];
			AddMoves(state, dictMoves, moves, current, regular);

			return moves;
		}
    }
}

