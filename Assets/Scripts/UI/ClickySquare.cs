using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    public class ClickySquare : MonoBehaviour
    {
        public MoveController mController;
        Board m_board;
        Square m_square;

        public void ClickSquare()
        {
            m_square = square.board.GetSquare(square.row, square.col);

            if (m_square.IsOccupied())
                mController.DisplayMoves(m_square.piece.pieceGameObject);
            else
            {
                mController.DisplayMoves(m_square.cubeObject);
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
