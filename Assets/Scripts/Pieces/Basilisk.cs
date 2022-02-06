using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
	using static Move;
	using static MoveDict;
	/* ----------- Basilisk ------------*/
	public class Basilisk : Piece
    {
        public Square danger_square;
        public Basilisk() : base(PieceType.Basilisk) { nameChar = "B"; value = 3; }
        public Material frozen_mat;
        Material original_mat;

		public static bool HasFrozen(Game state, Piece basilisk, Square s)
		{
			if (basilisk == null)
				return false;
			if (state.boards[2].squares[s.row, s.col].occupied)
			{
				Piece p = state.boards[2].squares[s.row, s.col].piece;
				if (basilisk.color != p.color)
					return true;
			}
			return false;
		}

        public override List<Move> GetMoves(Game state)
        {
			List<Move> moves = new List<Move>();
			List<(int, int, int)> dictMoves;
			Square current = this.pos;

			if (this.color == Color.White)
			{
				dictMoves = MoveDictionary["WBasilisk"][current.board, current.row, current.col];
				AddMoves(state, dictMoves, moves, current, move_cap);
				dictMoves = MoveDictionary["WBackwardsBasilisk"][current.board, current.row, current.col];
				AddMoves(state, dictMoves, moves, current, regular);
			}
			else
			{
			
				dictMoves = MoveDictionary["BBasilisk"][current.board, current.row, current.col];
				AddMoves(state, dictMoves, moves, current, move_cap);
				dictMoves = MoveDictionary["BBackwardsBasilisk"][current.board, current.row, current.col];
				AddMoves(state, dictMoves, moves, current, regular);
			}

			return moves;
		}
    }
}

