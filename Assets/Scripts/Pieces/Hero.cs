using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
	using static Move;
	using static MoveDict;
	/* ----------- Hero ------------
     * 
     */
	public class Hero : Piece
    {
        public Hero() : base(PieceType.Hero) { nameChar = "H"; value = 10; }
        public override List<Move> GetMoves(Game state)
        {
			List<Move> moves = new List<Move>();
			List<(int, int, int)> dictMoves;
			Square current = this.pos;

			if (current.board == 3)
			{
				dictMoves = MoveDictionary["TopHero"][current.board, current.row, current.col];
				AddMoves(state, dictMoves, moves, current, move_cap);
			}
			else if (current.board == 2)
			{
				dictMoves = MoveDictionary["MidHero"][current.board, current.row, current.col];
				AddMoves(state, dictMoves, moves, current, move_cap);
			}
			else
			{
				dictMoves = MoveDictionary["BotHero"][current.board, current.row, current.col];
				AddMoves(state, dictMoves, moves, current, move_cap);
			}
			return moves;
		}
    }
}

