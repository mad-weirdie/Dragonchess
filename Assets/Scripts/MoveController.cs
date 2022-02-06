using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace Dragonchess
{
	using GC = GameController;
	using static Game;
	using static GameController;
	using static GameUI;
	using static Gamestate;
	using static BitMoveController;
	using static BitMove;
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

		public static void PromoteWarrior(Game state, Square s, Piece piece, bool updateUI = false)
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
				int level = s.board;

				// Add the new hero piece, linking it to a piece GameObject as well
				Piece hero = NewPiece(state, (int)PieceType.Hero, player, level, s.row, s.col);
				AddPieceUI(hero);
			}
		}

		public static void RemoveIllegal(Game state, Player p, ref List<Move> moves)
		{
			List<Move> illegal = new List<Move>();
			List<int> threats;
			for (int i = 0; i < moves.Count; i++)
			{
				Move m = moves[i];
				Gamestate sState = new Gamestate(state);
				int move = MoveToBitMove(state, m);
				int start = GetStartPiece(sState, move);
				int end = GetEndPiece(sState, move);

				DoBitMove(ref sState, move);

				threats = ThreatsInRange(sState, (p.color == Color.White));
				if (threats.Count != 0)
				{
					if (IsCheck(sState, (p.color == Color.White)))
						illegal.Add(m);
				}
				UndoBitMove(ref sState, move, start, end);
			}
			for (int i = 0; i < illegal.Count; i++)
			{
				Move illegal_move = illegal[i];
				moves.Remove(illegal_move);
			}
		}

		public static void DoMove(Game state, Move move, bool updateUI)
		{
			Piece p = move.piece;
			Piece captured = move.captured;

			// Swoop move: capture enemy piece but stay in place
			if (move.IsType(MoveType.Swoop))
			{
				p.RemoteCapture(state, move.end.piece);
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
				GameUI.OnMoveUpdateUI(state, move);

			// Check for warrior piece promotion
			if (p.type == PieceType.Warrior)
				PromoteWarrior(state, move.end, p, updateUI);
		}

		public static void UndoMove(Game state, Move m, bool updateUI = false)
		{
			Piece p = m.piece;

			if (m.IsType(MoveType.Capture) || m.IsType(MoveType.Swoop))
			{
				// Add the captured piece back to the enemy player's list of pieces
				Piece cap = m.captured;
				if (!cap.player.pieces.Contains(cap))
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
