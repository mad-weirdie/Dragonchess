using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
	using static Move;
	using static MoveDict;
	/* ----------- Dwarf ------------
     * 
     */
	public class Dwarf : Piece
    {
        public Dwarf() : base(PieceType.Dwarf) { nameChar = "D"; value = 2; }
        public override List<Move> GetMoves(Game state)
        {
			List<Move> moves = new List<Move>();
			List<(int, int, int)> dictMoves;
			Square current = this.pos;

			if (this.color == Color.White && current.board == 2)
			{
				dictMoves = MoveDictionary["MidWDwarf"][current.board, current.row, current.col];
				AddMoves(state, dictMoves, moves, current, regular);
				dictMoves = MoveDictionary["MidWDwarfTake"][current.board, current.row, current.col];
				AddMoves(state, dictMoves, moves, current, capture);
			}
			else if (this.color == Color.Black && current.board == 2)
			{
				dictMoves = MoveDictionary["MidBDwarf"][current.board, current.row, current.col];
				AddMoves(state, dictMoves, moves, current, regular);
				dictMoves = MoveDictionary["MidBDwarfTake"][current.board, current.row, current.col];
				AddMoves(state, dictMoves, moves, current, capture);
			}
			else if (this.color == Color.White && current.board == 1)
			{
				dictMoves = MoveDictionary["BotWDwarf"][current.board, current.row, current.col];
				AddMoves(state, dictMoves, moves, current, regular);
				dictMoves = MoveDictionary["BotWDwarfTake"][current.board, current.row, current.col];
				AddMoves(state, dictMoves, moves, current, capture);
			}
			else if (this.color == Color.Black && current.board == 1)
			{
				dictMoves = MoveDictionary["BotBDwarf"][current.board, current.row, current.col];
				AddMoves(state, dictMoves, moves, current, regular);
				dictMoves = MoveDictionary["BotBDwarfTake"][current.board, current.row, current.col];
				AddMoves(state, dictMoves, moves, current, capture);
			}

			return moves;
		}
    }
}

