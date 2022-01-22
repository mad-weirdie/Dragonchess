using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
	using static Move;
	using static MoveDict;
	/* ----------- Cleric ------------
     * 
     */
	public class Cleric : Piece
    {
        public Cleric() : base(PieceType.Cleric) { nameChar = "C"; value = 30; }
        public override List<Move> GetMoves(Gamestate state)
        {
			List<Move> moves = new List<Move>();
			List<(int, int, int)> dictMoves;
			Square current = this.pos;

			if (current.board == 3)
			{
				dictMoves = MoveDictionary["TopCleric"][current.board, current.row, current.col];
				AddMoves(state, dictMoves, moves, current, move_cap);
			}
			else if (current.board == 2)
			{
				dictMoves = MoveDictionary["MidCleric"][current.board, current.row, current.col];
				AddMoves(state, dictMoves, moves, current, move_cap);
			}
			else
			{
				dictMoves = MoveDictionary["BotCleric"][current.board, current.row, current.col];
				AddMoves(state, dictMoves, moves, current, move_cap);
			}
			return moves;
		}
    }
}

