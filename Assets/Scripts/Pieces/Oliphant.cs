using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
	using static Move;
	using static MoveDict;
	/* ----------- Oliphant ------------
     * Moves like a chess rook. Stays on level 2.
     */
	public class Oliphant : Piece
    {
        public Oliphant() : base(PieceType.Oliphant) { nameChar = "O"; value = 15; }
		public override List<Move> GetMoves(Gamestate state)
		{
			List<Move> moves = new List<Move>();
			List<(int, int, int)> dictMoves;
			Square current = this.pos;

			List<string> slideTypes = new List<string> { "forward", "backward", "left", "right" };

			dictMoves = MoveDictionary["Oliphant"][current.board, current.row, current.col];
			List<(int, int, int)> blocked = GetBlocked(state, dictMoves, slideTypes, current);
			AddMoves(state, dictMoves, moves, current, move_cap);
			RemoveBlocked(state, blocked, moves);
			return moves;
		}
    }
}