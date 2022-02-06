using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
	using static Move;
	using static MoveDict;
	/* ----------- King ------------
     * 
     */
	public class King : Piece
    {
        public King() : base(PieceType.King) { nameChar = "K"; value = 900; }
        public override List<Move> GetMoves(Game state)
        {
			List<Move> moves = new List<Move>();
			List<(int, int, int)> dictMoves;
			Square current = this.pos;

			if (current.board == 3)
			{
				dictMoves = MoveDictionary["TopKing"][current.board, current.row, current.col];
				AddMoves(state, dictMoves, moves, current, move_cap);
			}
			else if (current.board == 2)
			{
				dictMoves = MoveDictionary["MidKing"][current.board, current.row, current.col];
				AddMoves(state, dictMoves, moves, current, move_cap);
			}
			else
			{
				dictMoves = MoveDictionary["BotKing"][current.board, current.row, current.col];
				AddMoves(state, dictMoves, moves, current, move_cap);
			}
			return moves;
		}
    }
}

