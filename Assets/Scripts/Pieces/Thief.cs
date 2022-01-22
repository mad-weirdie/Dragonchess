using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
	using static Move;
	using static MoveDict;
	/* ----------- Thief ------------
     * 
     */
	public class Thief : Piece
    {
        public Thief() : base(PieceType.Thief) { nameChar = "T"; value = 5; }

        public override List<Move> GetMoves(Gamestate state)
        {
			List<Move> moves = new List<Move>();
			List<(int, int, int)> dictMoves;
			Square current = this.pos;

			List<string> slideTypes = new List<string> { "diagUR", "diagUL", "diagDR", "diagDL" };

			dictMoves = MoveDictionary["Thief"][current.board, current.row, current.col];
			List<(int, int, int)> blocked = GetBlocked(state, dictMoves, slideTypes, current);
			AddMoves(state, dictMoves, moves, current, move_cap);
			RemoveBlocked(state, blocked, moves);
			return moves;
        }
    }
}

