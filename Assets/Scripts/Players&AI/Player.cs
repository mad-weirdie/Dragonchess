using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dragonchess
{
    public class Player
    {
        Color m_color;
        PlayerType m_type;
        public List<Piece> pieces;
        public Move prevMove;

        // Initialize player with color c
        public Player(Color c, PlayerType t)
        {
            pieces = new List<Piece>();
            m_color = c;
            m_type = t;
        }

        virtual public void GetMove(Game state) { return; }

        virtual public void OnClick(Game state) { }

        public List<Move> GetPossibleMoves(Game state)
		{
            List<Move> moves = new List<Move>();
			for (int i = 0; i < pieces.Count; i++)
			{
				Piece p = pieces[i];
				List<Move> pieceMoves = p.GetMoves(state);
				for (int j = 0; j < pieceMoves.Count; j++)
				{
					Move m = pieceMoves[j];
					m.piece = p;
					moves.Add(m);
				}
			}
			return moves;
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

        public PlayerType type
        {
            get
            {
                return m_type;
            }
            set
            {
                m_type = value;
            }
        }

        public bool isWhite()
        {
            return (this.color == Color.White);
        }

        public bool isBlack()
        {
            return (this.color == Color.Black);
        }
    }
}
