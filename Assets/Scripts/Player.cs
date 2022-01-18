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
        public bool inCheck = false;
        public Move prevMove;

        // Initialize player with color c
        public Player(Color c, PlayerType t)
        {
            pieces = new List<Piece>();
            m_color = c;
            m_type = t;
        }

        public void AddPiece(Piece p, Square s)
        {
            pieces.Add(p);
            p.pos = s;
            s.piece = p;
        }

        void Update()
		{

		}

        virtual public void GetMove(Gamestate state) { return; }

        virtual public void OnClick(Gamestate state) { }

        public List<Move> GetPossibleMoves(Gamestate state)
		{
            List<Move> moves = new List<Move>();
            foreach (Piece p in pieces)
			{
                foreach (Move m in p.GetMoves(state))
				{
                    m.piece = p;
                    moves.Add(m);
				}
			}
			//MoveController.RemoveIllegal(state, this, ref moves);
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
