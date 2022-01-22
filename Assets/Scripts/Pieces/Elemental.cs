using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
	using static Move;
	using static MoveDict;
	/* ----------- Elemental ------------
     * On level 1:
        - can move and capture one or two steps orthogonally
        - can move one step diagonally;
        - can capture in the following pattern: one step orthogonally followed
          by the square directly above on level 2.

       On level 2:
        - can move and capture in the following pattern: the square directly
          below on level 1 followed by one step orthogonally.
     */
	public class Elemental : Piece
    {
        public Elemental() : base(PieceType.Elemental) { nameChar = "E"; value = 4; }
        public override List<Move> GetMoves(Gamestate state)
        {
			List<Move> moves = new List<Move>();
			List<(int, int, int)> dictMoves;
			Square current = this.pos;

			if (current.board == 1)
			{
				List<string> slideTypes = new List<string> { "forward", "backward", "left", "right" };

				dictMoves = MoveDictionary["BotElemental"][current.board, current.row, current.col];
				List<(int, int, int)> blocked = GetBlocked(state, dictMoves, slideTypes, current);
				AddMoves(state, dictMoves, moves, current, move_cap);
				RemoveBlocked(state, blocked, moves);

				dictMoves = MoveDictionary["BotElementalSlide"][current.board, current.row, current.col];
				AddMoves(state, dictMoves, moves, current, regular);
			}
			else if (current.board == 2)
			{
				dictMoves = MoveDictionary["MidElemental"][current.board, current.row, current.col];
				if (!state.lowerBoard.squares[current.row, current.col].occupied)
					AddMoves(state, dictMoves, moves, current, move_cap);
			}

			return moves;
		}
    }
}

