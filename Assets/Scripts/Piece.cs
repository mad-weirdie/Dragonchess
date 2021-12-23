using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Dragonchess
{
    public class Piece
    {
        Color m_color;

        PieceType m_type;
        bool isActive;
        Square m_location;

        public enum PieceType { Sylph, Griffon, Dragon, Warrior, Oliphant,
                                Unicorn, Hero, Thief, Cleric, Mage, King,
                                Paladin, Dwarf, Basilisk, Elemental };

        public Piece ()
        {
            isActive = true;
        }

        public Piece(PieceType t)
        {
            m_type = t;
        }

        virtual public ArrayList GetMoves()
        {
            return null;
        }

        public Square location
        {
            get
            {
                return m_location;
            }
            set
            {
                m_location = value;
            }
        }

        public Color color
        {
            get
            {
                return m_color;
            }
            set
            {
                m_color = value;
            }
        }

        public bool active
        {
            get
            {
                return isActive;
            }
            set
            {
                isActive = value;
            }
        }
    }

    /* ----------- Sylph ------------
     * On level 3:
     *  i)  can move one step diagonally forward or capture one step
     *      straight forward
     *  ii) can capture on the square directly below the sylph (on level 2)
     *  
     *  On level 2:
     *  i)  can move to the square directly above (on level 3) or to one of the player's
     *      six Sylph starting squares
     */
    public class Slyph : Piece 
    {
        public override ArrayList GetMoves()
        {
            ArrayList moves = new ArrayList();

            // Get current piece data
            Square current_square = this.location;
            Square new_square;
            Layer layer = current_square.layer;
            int direction;

            // Definition of "forward, left, right" changes based on piece color
            if (this.color == Color.White)
                direction = 1;
            else
                direction = -1;

            // Level 3 moves
            if (layer == Layer.Upper)
            {
                // Move only: left forward diagonal
                int new_row = current_square.row + direction;
                int new_col = current_square.col - direction;
                new_square = new Square(new_row, new_col, Layer.Upper);
                Move next_move = new Move(current_square, new_square, Move.MoveType.Regular);
                if (next_move.IsValidMove())
                {
                    moves.Add(next_move);
                }

                // Move only: right forward diagonal
                new_row = current_square.row + direction;
                new_square = new Square(new_row, new_col, Layer.Upper);
                next_move = new Move(current_square, new_square, Move.MoveType.Regular);
                if (next_move.IsValidMove())
                {
                    moves.Add(next_move);
                }

                // Capture: one forward
                new_col = current_square.col;
                new_square = new Square(new_row, new_col, Layer.Upper);
                next_move = new Move(current_square, new_square, Move.MoveType.Capture);
                if (next_move.IsValidMove())
                {
                    moves.Add(next_move);
                }
            }




            return moves;
        }
    }

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
        public override ArrayList GetMoves()
        {
            return base.GetMoves();
        }
    }

    public class Dragon : Piece
    {
        public override ArrayList GetMoves()
        {
            return base.GetMoves();
        }
    }

    public class Warrior : Piece
    {
        public override ArrayList GetMoves()
        {
            return base.GetMoves();
        }
    }

    public class Oliphant : Piece
    {
        public override ArrayList GetMoves()
        {
            return base.GetMoves();
        }
    }

    public class Unicorn : Piece
    {
        public override ArrayList GetMoves()
        {
            return base.GetMoves();
        }
    }

    public class Hero : Piece
    {
        public override ArrayList GetMoves()
        {
            return base.GetMoves();
        }
    }

    public class Thief : Piece
    {
        public override ArrayList GetMoves()
        {
            return base.GetMoves();
        }
    }

    public class Cleric : Piece
    {
        public override ArrayList GetMoves()
        {
            return base.GetMoves();
        }
    }

    public class Mage : Piece
    {
        public override ArrayList GetMoves()
        {
            return base.GetMoves();
        }
    }

    public class King : Piece
    {
        public override ArrayList GetMoves()
        {
            return base.GetMoves();
        }
    }

    public class Paladin : Piece
    {
        public override ArrayList GetMoves()
        {
            return base.GetMoves();
        }
    }

    public class Dwarf : Piece
    {
        public override ArrayList GetMoves()
        {
            return base.GetMoves();
        }
    }

    public class Basilisk : Piece
    {
        public override ArrayList GetMoves()
        {
            return base.GetMoves();
        }
    }

    public class Elemental : Piece
    {
        public override ArrayList GetMoves()
        {
            return base.GetMoves();
        }
    }

}
