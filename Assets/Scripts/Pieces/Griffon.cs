using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{

    /* ----------- Griffon ------------
     * On level 3:
     *  i)  can move and capture by jumping in the following pattern:
     *      two steps diagonally followed by one step orthogonally outwards
     *  ii) can move and capture one step triagonally to level 2
     *  
     *  On level 2:
     *  i)  can move and capture one step diagonally
     *  ii) can move and capture one step triagonally to level 3
     */
    public class Griffon : Piece
    {
        public Griffon() : base(PieceType.Griffon) { }
        public override ArrayList GetMoves()
        {
            ArrayList moves = new ArrayList();


            return moves;
        }
    }
}

