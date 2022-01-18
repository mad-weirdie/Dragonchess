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

        Board m_board;

        public void ClickSquare()
        {
            Gamestate state = GameController.state;
            if (GC.GameFromFileEnabled)
                return;

            if (state.ActivePlayer.type == PlayerType.Human)
            {
                GameObject selected = square.gameObject;
                if (square.board.squares[square.row, square.col].occupied)
                {
                    selected = square.piece.gameObject;
                }
                ((Human)state.ActivePlayer).MoveSelect(state, selected);
            }
        }

        public Board board
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
