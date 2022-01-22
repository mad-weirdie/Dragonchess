using System.Collections;
using System.Collections.Generic;
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
            Gamestate state = GameController.state;
            if (GC.GameFromFileEnabled)
                return;

            if (state.ActivePlayer.type == PlayerType.Human)
            {
                GameObject selected = square.gameObject;
				Board board;
				if (square.board == 3)
					board = state.upperBoard;
				else if (square.board == 2)
					board = state.middleBoard;
				else
					board = state.lowerBoard;

                if (board.squares[square.row, square.col].occupied)
                {
                    selected = square.piece.gameObject;
                }
                ((Human)state.ActivePlayer).MoveSelect(state, selected);
            }
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
