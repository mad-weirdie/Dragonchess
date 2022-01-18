using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    public class PieceObject : MonoBehaviour
    {
        public Piece piece;
        public SquareObject pos;
        public PieceType type;
        public int board;
        public int row;
        public int col;
    }
}
