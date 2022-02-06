using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
	using static Move;
	using static MoveDict;
	/* ----------- Mage ------------
     * 
     */
	public class Mage : Piece
    {
        public Mage() : base(PieceType.Mage) { nameChar = "M"; value = 40; }
        public override List<Move> GetMoves(Game state)
        {
			List<Move> moves = new List<Move>();
			List<(int, int, int)> dictMoves;
			Square current = this.pos;

			List<string> slideTypes = new List<string> {
				"diagUR", "diagUL", "diagDR", "diagDL",
				"forward", "backward", "left", "right"};

			if (current.board == 3)
				dictMoves = MoveDictionary["TopMage"][current.board, current.row, current.col];
			else if (current.board == 2)
				dictMoves = MoveDictionary["MidMage"][current.board, current.row, current.col];
			else
				dictMoves = MoveDictionary["BotMage"][current.board, current.row, current.col];
			List<(int, int, int)> blocked = GetBlocked(state, dictMoves, slideTypes, current);
			AddMoves(state, dictMoves, moves, current, move_cap);
			RemoveBlocked(state, blocked, moves);
			return moves;
		}
    }
}

