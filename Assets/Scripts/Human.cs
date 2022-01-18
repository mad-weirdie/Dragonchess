using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Dragonchess
{
    using UI = GameUI;
    using static GameUI;
    using static MoveController;
    using static Gamestate;
    public class Human : Player
    {
        public static Move nextMove;

        // Initialize player with color c
        public Human(Color c, PlayerType t) : base(c, PlayerType.Human)
        {
            nextMove = null;
        }

        override public void GetMove(Gamestate state)
        {
            nextMove = null;
        }

        override public void OnClick(Gamestate state)
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

        public void MoveSelect(Gamestate state, GameObject clickedObj)
        {
            Piece piece = null;
            Move move = null;

            // If we clicked on a piece belonging to us
            if (IsPlayerPiece(this, clickedObj, ref piece))
			{
                DeselectAll();

                List<Move> possibleMoves = piece.GetMoves(state);
				RemoveIllegal(state, this, ref possibleMoves);
                HighlightMoves(piece, possibleMoves);
			}
            // If we already had something selected before this
            else if (PieceSelected())
            {
                if (IsAvailableMove(clickedObj, ref move))
                {
                    DeselectAll();
                    GameController.OnMoveReceived(state, state.ActivePlayer, move);
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
