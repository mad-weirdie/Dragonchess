using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
	using static Move;
	using static MoveDict;
	/* ----------- Griffon ------------
     * On level 3:
     *  i)  can move and capture by jumping in the following pattern:
     *      two steps diagonally followed by one step orthogonally outwards
     *  ii) can move and capture one step triagonally to level 2
     *  
     *  On level 2:
     *  i)  can move and capture one step diagonally
     *  ii) can move and capture one step triagonally to level 3
     */
	public class Griffon : Piece
    {
        public Griffon() : base(PieceType.Griffon) { nameChar = "G"; value = 20; }
        public override List<Move> GetMoves(Gamestate state)
        {
			List<Move> moves = new List<Move>();
			List<(int, int, int)> dictMoves;
			Square current = this.pos;

			if (current.board == 3)
			{
				dictMoves = MoveDictionary["TopGrif"][current.board, current.row, current.col];
				AddMoves(state, dictMoves, moves, current, move_cap);
			}
			else if (current.board == 2)
			{
				dictMoves = MoveDictionary["MidGrif"][current.board, current.row, current.col];
				AddMoves(state, dictMoves, moves, current, move_cap);
			}

			return moves;
		}
    }
}

