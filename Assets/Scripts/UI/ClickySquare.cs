using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

namespace Dragonchess
{
    public class ClickySquare : MonoBehaviour
    {
        public MoveController mController;
        public GameController GC;
        public SquareObject square;

        int m_board;

        public void ClickSquare()
        {
            Game state = GameController.state;
            if (GameController.GameFromFileEnabled)
                return;

            if (state.ActivePlayer.type == PlayerType.Human)
            {
                GameObject selected = square.gameObject;
				Board board = state.boards[square.board];
                if (board.squares[square.row, square.col].occupied)
                {
                    selected = square.piece.gameObject;
                }
                ((Human)state.ActivePlayer).MoveSelect(state, selected);
            }
			EventSystem.current.SetSelectedGameObject(null);
		}

		public int board
        {
            get
            {
                return m_board;
            }
            set
            {
                m_board = value;
            }
        }
    }
}
