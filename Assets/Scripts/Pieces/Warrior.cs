using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
	using static Move;
	using static MoveDict;
	/* ----------- Warrior ------------
     * 
     */
	public class Warrior : Piece
    {
        public Warrior() : base(PieceType.Warrior) { nameChar = "W"; value = 1; }

        public override List<Move> GetMoves(Gamestate state)
        {
			List<Move> moves = new List<Move>();
			List<(int, int, int)> dictMoves;
			Square current = this.pos;

			if (this.color == Color.White)
			{
				dictMoves = MoveDictionary["WWarrior"][current.board, current.row, current.col];
				AddMoves(state, dictMoves, moves, current, regular);
				dictMoves = MoveDictionary["WWarriorTake"][current.board, current.row, current.col];
				AddMoves(state, dictMoves, moves, current, capture);
			}
			else
			{
				dictMoves = MoveDictionary["BWarrior"][current.board, current.row, current.col];
				AddMoves(state, dictMoves, moves, current, regular);
				dictMoves = MoveDictionary["BWarriorTake"][current.board, current.row, current.col];
				AddMoves(state, dictMoves, moves, current, capture);
			}
			return moves;
		}
    }
}

