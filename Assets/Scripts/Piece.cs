using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Dragonchess
{
	using static MoveDict;
	public enum PieceType
	{
		Sylph, Griffon, Dragon, Warrior, Oliphant,
		Unicorn, Hero, Thief, Cleric, Mage, King,
		Paladin, Dwarf, Basilisk, Elemental, EMPTY
	};

	public class Piece
	{
		public static MoveType capture = MoveType.Capture;
		public static MoveType regular = MoveType.Regular;
		public static MoveType move_cap = MoveType.MoveOrCapture;
		public static MoveType swoop = MoveType.Swoop;


		public bool frozen;     // is frozen by the basilisk
		public int value;       // piece value (worth)

		public PieceType m_type;
		public string nameChar;
		public Player player;

		Square m_pos;
		Color m_color;

		public Piece() { }

		public Piece(PieceType type)
		{
			m_type = type;
		}

		public static Piece NewPiece(Gamestate state, int t, Player p, int b, int r, int c)
		{
			Piece piece;
			if (t == 0)
				piece = new Sylph();
			else if (t == 1)
				piece = new Griffon();
			else if (t == 2)
				piece = new Dragon();
			else if (t == 3)
				piece = new Warrior();
			else if (t == 4)
				piece = new Oliphant();
			else if (t == 5)
				piece = new Unicorn();
			else if (t == 6)
				piece = new Hero();
			else if (t == 7)
				piece = new Thief();
			else if (t == 8)
				piece = new Cleric();
			else if (t == 9)
				piece = new Mage();
			else if (t == 10)
				piece = new King();
			else if (t == 11)
				piece = new Paladin();
			else if (t == 12)
				piece = new Dwarf();
			else if (t == 13)
				piece = new Basilisk();
			else
				piece = new Elemental();

			Board board;
			if (b == 3)
				board = state.upperBoard;
			else if (b == 2)
				board = state.middleBoard;
			else
				board = state.lowerBoard;

			piece.color = p.color;
			piece.player = p;
			piece.pos = board.squares[r, c];
			board.squares[r, c].occupied = true;
			board.squares[r, c].piece = piece;
			p.pieces.Add(piece);

			return piece;
		}

		// GetMoves to be overridden by child classes
		virtual public List<Move> GetMoves(Gamestate state) { return null; }

		virtual public void MoveTo(Gamestate state, Square s)
		{
			// Link piece and square to each other
			this.pos.occupied = false;
			this.pos = s;

			this.pos.occupied = true;
			s.piece = this;

			if (s.board == state.middleBoard.layer_int_val)
			{
				if (state.lowerBoard.squares[s.row, s.col].occupied)
				{
					Piece checkB = state.lowerBoard.squares[s.row, s.col].piece;
				}
			}
		}

		public bool Capture(Gamestate state, Piece enemy)
		{
			Square s = enemy.pos;
			if (enemy.type == PieceType.Basilisk)
			{
				if (state.middleBoard.squares[enemy.row, enemy.col].occupied)
				{
					Square basilisk = state.lowerBoard.squares[enemy.row, enemy.col];
					Square above = state.middleBoard.squares[enemy.row, enemy.col];
					above.piece.frozen = false;
				}
			}
			if (enemy.color == Color.White)
				state.P1.pieces.Remove(enemy);
			else
				state.P2.pieces.Remove(enemy);

			enemy.pos.piece = this;
			this.pos.occupied = false;
			this.pos = enemy.pos;

			return true;
		}

		public bool RemoteCapture(Gamestate state, Piece enemy)
		{
			if (enemy.color == Color.White)
				state.P1.pieces.Remove(enemy);
			else
				state.P2.pieces.Remove(enemy);

			enemy.pos.occupied = false;
			return true;
		}

		public Square pos
		{
			get
			{
				return m_pos;
			}
			set
			{
				m_pos = value;
			}
		}
		
		public int row
		{
			get
			{
				return m_pos.row;
			}
		}

		public int col
		{
			get
			{
				return m_pos.col;
			}
		}

		public Color color
		{
			get
			{
				return m_color;
			}
			set
			{
				m_color = value;
			}
		}

		public bool active { get; set; }

		public PieceType type
		{
			get
			{
				return m_type;
			}
			set
			{
				m_type = value;
			}
		}
	}
}
