using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    public class ClickySquare : MonoBehaviour
    {
        public MoveController mController;
        public GameController GC;
        Board m_board;
        Square m_square;

        public void ClickSquare()
        {
            if (GC.GameFromFileEnabled)
                return;
            m_square = square.board.GetSquare(square.row, square.col);

            if (m_square.IsOccupied())
                mController.MoveSelect(m_square.piece.pieceGameObject);
            else
            {
                mController.MoveSelect(m_square.cubeObject);
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

        public Square square
        {
            get
            {
                return m_square;
            }
            set
            {
                m_square = value;
            }
        }
    }
}
