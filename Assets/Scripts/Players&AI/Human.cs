using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System;
using System.IO;

namespace Dragonchess
{
    using UI = GameUI;
	using M = MonoBehaviour;
    using static GameUI;
    using static MoveController;
    using static Game;
	using static Gamestate;
	using static BitPiece;
	using static BitMove;
	using static BitMoveController;

	public class Human : Player
    {
        public static Move nextMove;

        // Initialize player with color c
        public Human(Color c, PlayerType t) : base(c, PlayerType.Human)
        {
            nextMove = null;
        }

        override public void GetMove(Game state)
        {
            nextMove = null;
        }

        override public void OnClick(Game state)
		{
            if (this == state.ActivePlayer)
            {
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                RaycastHit hit;
                // Check where we clicked
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, GameController.layerMask))
                {
                    // Locate GameObject of whatever we clicked on (piece or square)
                    GameObject hitGameObject = hit.transform.gameObject;
                    if (hitGameObject.GetComponent<ClickySquare>())
                    {
                        hitGameObject = hitGameObject.GetComponent<ClickySquare>().square.gameObject;
                    }
                    MoveSelect(state, hitGameObject);
                }
            }
        }

        public void MoveSelect(Game state, GameObject clickedObj)
        {
            Piece piece = null;

            // If we clicked on a piece belonging to us
            if (IsPlayerPiece(this, clickedObj, ref piece))
			{
                DeselectAll();
				//List<Move> possibleMoves = piece.GetMoves(state);
				//RemoveIllegal(state, this, ref possibleMoves);
				byte type = (byte)((int)(piece.type));
				byte board = (byte)(piece.pos.board);
				byte row = (byte)(piece.pos.row);
				byte col = (byte)(piece.pos.col);

				if (this.color == Color.Black)
					board = (byte)(0x8 | board);
				byte[] bytes = { type, board, row, col };

				int bPiece = BitConverter.ToInt32(bytes, 0);
				Gamestate sState = new Gamestate(state);
				List<int> possibleMoves = GetBitMoves(sState, bPiece);
				List<Move> moves = new List<Move>();

				for (int i = 0; i < possibleMoves.Count; i++)
				{
					int m = possibleMoves[i];
					Move move = BitMoveToMove(state, m);
					moves.Add(move);
				}
                HighlightMoves(piece, moves);
			}
            // If we already had something selected before this
            else if (PieceSelected())
            {
				Move move = null;
				if (IsAvailableMove(clickedObj, ref move))
                {
                    DeselectAll();
					//PrintState(new Gamestate(state), 99);
					M.print("Move received: " + move.MoveToString());
					GameController.OnMoveReceived(state, state.ActivePlayer, move);
					//PrintState(new Gamestate(state), 99);
                }
                else
                    DeselectAll();
            }
            else
            {
                DeselectAll();
            }
        }
    }
}
