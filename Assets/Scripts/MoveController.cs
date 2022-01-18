using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace Dragonchess
{
	using GC = GameController;
	using static Gamestate;
	using static GameController;
	using static GameUI;
	using static Piece;

	public class MoveController : MonoBehaviour
	{
		public GameObject selectedObj;
		public GameController G;
		public static List<(Square, MoveType)> highlightedSquares;

		public Material frozen;

		public void Start()
		{
			highlightedSquares = new List<(Square, MoveType)>();
		}

		public static void PromoteWarrior(Gamestate state, Square s, Piece piece, bool updateUI = false)
		{
			bool promote = false;
			if (piece.color == Color.White && s.row == 7)
				promote = true;
			else if (piece.color == Color.Black && s.row == 0)
				promote = true;
			else

			if (promote)
			{
				// Get references to the associated GameObjects
				SquareObject square = GetSquareAt(s.board, s.row, s.col);
				Player player = piece.player;
				PieceObject warrior = square.piece;
				Destroy(warrior.gameObject);
				int level = s.board.layer_int_val;

				// Add the new hero piece, linking it to a piece GameObject as well
				Piece hero = NewPiece(state, (int)PieceType.Hero, player, level, s.row, s.col);
				AddPieceUI(hero);
			}
		}

		public static void RemoveIllegal(Gamestate state, Player p, ref List<Move> moves)
		{
			List<Move> illegal = new List<Move>();

			foreach (Move m in moves)
			{
				if (m.type == MoveType.Capture)
				{
					// Pretend to move attacking piece
					Piece attacking_piece = m.start.piece;
					Piece captured_piece = m.end.piece;
					m.start.occupied = false;

					// Pretend to capture attacked piece
					attacking_piece.pos = m.end;
					m.end.occupied = true;
					m.end.piece = attacking_piece;

					// Remove captured piece from enemy's list of pieces
					GetEnemy(state, p).pieces.Remove(captured_piece);

					if (IsCheck(state, p))
					{
						illegal.Add(m);
					}

					// Move attacking piece back to original location
					m.start.occupied = true;
					m.start.piece = attacking_piece;
					m.end.piece = captured_piece;
					attacking_piece.pos = m.start;
					captured_piece.pos = m.end;

					// Re-add captured piece back to enemy's list of pieces
					GetEnemy(state, p).pieces.Add(captured_piece);
				}
				else if (m.type == MoveType.Swoop)
				{
					// Pretend to capture the attacked piece
					Piece attacking_piece = m.start.piece;
					Piece captured_piece = m.end.piece;
					m.end.occupied = false;

					// Remove captured piece from enemy's list of pieces
					GetEnemy(state, p).pieces.Remove(captured_piece);

					if (IsCheck(state, p))
					{
						illegal.Add(m);
					}
					// Un-capture the attacked piece
					m.end.occupied = true;
					m.end.piece = captured_piece;

					// Re-add captured piece back to enemy's list of pieces
					GetEnemy(state, p).pieces.Add(captured_piece);
				}
				else if (m.type == MoveType.Regular)
				{
					Piece moving_piece = m.start.piece;
					m.start.occupied = false;
					m.end.occupied = true;
					moving_piece.pos = m.end;
					m.end.piece = moving_piece;

					if (IsCheck(state, p))
					{
						illegal.Add(m);
					}

					m.start.occupied = true;
					m.end.occupied = false;
					m.start.piece = moving_piece;
					moving_piece.pos = m.start;
					m.end.piece = null;
				}
				else
				{
					print("ERROR: Unknown move type.");
				}
			}
			foreach (Move illegal_move in illegal)
				moves.Remove(illegal_move);
		}

		public bool WouldCheckmate(Gamestate state, Move m)
		{
			DoMove(state, m, false);
			bool isMate = IsCheckmate(state, GetEnemy(state, m.piece.player));
			UndoMove(state, m, false);
			return isMate;
		}

		public static void DoMove(Gamestate state, Move move, bool updateUI)
		{
			Piece p = move.piece;
			Piece captured = move.captured;

			// Swoop move: capture enemy piece but stay in place
			if (move.IsType(MoveType.Swoop))
			{
				p.RemoteCapture(state, captured);
			}
			// Capture move: capture and replace enemy piece
			else if (move.IsType(MoveType.Capture))
			{
				p.Capture(state, captured);
				p.MoveTo(state, move.end);
			}
			// Regular move: just change positions
			else
			{
				p.MoveTo(state, move.end);
			}

			// Update the Game UI if enabled
			if (updateUI)
				GameUI.OnMoveUpdateUI(move);

			// Check for warrior piece promotion
			if (p.type == PieceType.Warrior)
				PromoteWarrior(state, move.end, p, updateUI);
		}

		public void UndoMove(Gamestate state, Move m, bool updateUI = false)
		{
			Piece p = m.piece;

			if (m.IsType(MoveType.Capture) || m.IsType(MoveType.Swoop))
			{
				// Add the captured piece back to the enemy player's list of pieces
				Piece cap = m.captured;
				print("captured: " + cap.type + ": " + cap.pos.SquareName());
				cap.player.pieces.Add(cap);

				// Move the capturing piece back
				if (!m.IsType(MoveType.Swoop))
				{
					Move undoMove = new Move(m.piece, m.end, m.start, MoveType.Regular);
					DoMove(state, undoMove, updateUI);
				}

				// Move the captured piece back
				Move undoCap = new Move(cap, m.end, m.end, MoveType.Regular);

				if (updateUI)
				{
					List<PieceObject> pieces;
					if (cap.player.color == Color.White)
						pieces = GameUI.P1_Pieces;
					else
						pieces = GameUI.P2_Pieces;
					AddPieceUI(cap);
				}

				DoMove(state, undoCap, updateUI);
			}
			else
			{
				Move undoMove = new Move(m.piece, m.end, m.start, MoveType.Regular);
				DoMove(state, undoMove, updateUI);
			}
		}
	}
}
