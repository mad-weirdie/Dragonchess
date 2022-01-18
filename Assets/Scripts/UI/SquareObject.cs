using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Dragonchess
{
    public class SquareObject : MonoBehaviour
    {
        public Material properMaterial;
        public PieceObject piece;
        public GameObject dot;
        public Board board;
        public int row;
        public int col;
    }
}
