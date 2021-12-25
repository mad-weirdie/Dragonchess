using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    /* ----------- Hero ------------
     * 
     */
    public class Hero : Piece
    {
        public Hero() : base(PieceType.Hero) { }
        public override ArrayList GetMoves()
        {
            ArrayList moves = new ArrayList();


            return moves;
        }
    }
}

