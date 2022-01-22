using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
	using static Move;
	using static MoveDict;
	/* ----------- Paladin ------------
     * 
     */
	public class Paladin : Piece
    {
        public Paladin() : base(PieceType.Paladin) { nameChar = "P"; value = 50; }
        public override List<Move> GetMoves(Gamestate state)
        {
			List<Move> moves = new List<Move>();
			List<(int, int, int)> dictMoves;
			Square current = this.pos;

			dictMoves = MoveDictionary["Paladin"][current.board, current.row, current.col];
			AddMoves(state, dictMoves, moves, current, move_cap);
			return moves;
		}
    }
}

