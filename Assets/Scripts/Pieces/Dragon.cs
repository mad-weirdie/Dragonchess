using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
	using static Move;
	using static MoveDict;
	using static Gamestate;
	using static BitPiece;
	using static BitMove;
	using static BitMoveController;

	/* ----------- Dragon ------------*/
	public class Dragon : Piece
    {
        public Dragon() : base(PieceType.Dragon) { nameChar = "R"; value = 80; }
        public override List<Move> GetMoves(Game state)
        {
            List<Move> moves = new List<Move>();
            Square current = this.pos;

			List<string> slideTypes = new List<string> { "diagUR", "diagUL", "diagDR", "diagDL" };
			List<(int, int, int)> possibleMoves;
			possibleMoves = MoveDictionary["Dragon"][current.board, current.row, current.col];
			List<(int, int, int)> blocked = GetBlocked(state, possibleMoves, slideTypes, current);
			
			AddMoves(state, possibleMoves, moves, current, move_cap);
			RemoveBlocked(state, blocked, moves);

			possibleMoves = MoveDictionary["SwoopDragon"][current.board, current.row, current.col];
			AddMoves(state, possibleMoves, moves, current, swoop);

			Gamestate sState = new Gamestate(state);

			int index = current.row * 12 + current.col;
			List<int> dMoves = DragonBitMoves(sState, (int)this.color, current.board, index);
			return moves;
        }
    }
}

